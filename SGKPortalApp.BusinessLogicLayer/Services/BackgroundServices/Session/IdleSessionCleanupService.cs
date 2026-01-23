using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServices.Session
{
    /// <summary>
    /// 30 dakika boyunca aktif olmayan (idle) session'ları otomatik logout yapan background service
    /// Her 5 dakikada bir çalışır
    /// </summary>
    public class IdleSessionCleanupService : BackgroundService, IManagedBackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IdleSessionCleanupService> _logger;
        private TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // 5 dakikada bir kontrol
        private readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(30); // 30 dakika idle timeout
        private bool _isRunning;

        // IManagedBackgroundService properties
        public string ServiceName => "IdleSessionCleanupService";
        public string DisplayName => "Idle Session Temizleme Servisi";
        public bool IsRunning => _isRunning;
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; private set; }
        public DateTime? NextRunTime { get; private set; }
        public TimeSpan Interval
        {
            get => _checkInterval;
            set => _checkInterval = value;
        }
        public string? LastError { get; private set; }
        public int SuccessCount { get; private set; }
        public int ErrorCount { get; private set; }

        public IdleSessionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<IdleSessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task TriggerAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 {ServiceName} manuel tetiklendi", ServiceName);
            await CleanupIdleSessionsAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 {ServiceName} başlatıldı - Interval: {Interval} dakika, Timeout: {Timeout} dakika",
                ServiceName, _checkInterval.TotalMinutes, _idleTimeout.TotalMinutes);

            try
            {
                // İlk çalışmadan önce 1 dakika bekle (startup sırasında yük olmaması için)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        NextRunTime = DateTime.Now.Add(_checkInterval);

                        if (IsPaused)
                        {
                            _logger.LogDebug("⏸️ {ServiceName} duraklatıldı, atlanıyor...", ServiceName);
                            await Task.Delay(_checkInterval, stoppingToken);
                            continue;
                        }

                        _isRunning = true;
                        await CleanupIdleSessionsAsync(stoppingToken);
                        LastRunTime = DateTime.Now;
                        LastError = null;
                        SuccessCount++;
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        _logger.LogError(ex, "❌ Idle session cleanup sırasında hata oluştu");
                        LastError = ex.Message;
                        ErrorCount++;
                    }
                    finally
                    {
                        _isRunning = false;
                    }

                    // Bir sonraki çalışmaya kadar bekle
                    await Task.Delay(_checkInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Uygulama kapanıyor, normal durum
                _logger.LogInformation("⏹️ {ServiceName} durduruldu (shutdown signal)", ServiceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {ServiceName} kritik hata", ServiceName);
                throw; // Kritik hatalar için rethrow
            }

            _logger.LogInformation("⏹️ {ServiceName} durduruldu", ServiceName);
        }

        private async Task CleanupIdleSessionsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var loginLogoutLogService = scope.ServiceProvider.GetRequiredService<ILoginLogoutLogService>();
            var hubConnectionService = scope.ServiceProvider.GetRequiredService<IHubConnectionBusinessService>();

            try
            {
                var userRepo = unitOfWork.GetRepository<IUserRepository>();

                // Tüm aktif kullanıcıları al (SessionID != null olan)
                var allUsers = await unitOfWork.Repository<SGKPortalApp.BusinessObjectLayer.Entities.Common.User>()
                    .GetAllAsync();

                var activeUsers = allUsers
                    .Where(u => !string.IsNullOrEmpty(u.SessionID) && u.AktifMi)
                    .ToList();

                if (activeUsers.Count == 0)
                {
                    _logger.LogDebug("ℹ️ Kontrol edilecek aktif kullanıcı yok");
                    return;
                }

                _logger.LogDebug("🔍 Idle session kontrolü başladı - {Count} aktif kullanıcı kontrol ediliyor", activeUsers.Count);

                var now = DateTime.Now;
                int logoutCount = 0;
                int bankoCleanupCount = 0;

                foreach (var user in activeUsers)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Son aktivite zamanı yoksa SonGirisTarihi'ni kullan (geriye uyumluluk)
                    var lastActivity = user.SonAktiviteZamani ?? user.SonGirisTarihi;

                    if (lastActivity == null)
                    {
                        // Hem SonAktiviteZamani hem SonGirisTarihi null ise, bu anormal bir durum
                        // Bu kullanıcıyı logout yapma, sadece log tut
                        _logger.LogWarning("⚠️ Kullanıcının son aktivite zamanı null - TC: {TcKimlikNo}, SessionID: {SessionID}",
                            user.TcKimlikNo, user.SessionID);
                        continue;
                    }

                    var idleDuration = now - lastActivity.Value;

                    // 30 dakikadan fazla idle ise logout yap
                    if (idleDuration > _idleTimeout)
                    {
                        _logger.LogInformation("⏰ Idle timeout tespit edildi - TC: {TcKimlikNo}, Son Aktivite: {LastActivity}, Idle Süre: {IdleDuration} dakika",
                            user.TcKimlikNo, lastActivity.Value, idleDuration.TotalMinutes);

                        try
                        {
                            // 1. Eğer banko modundaysa, önce banko modundan çık
                            if (user.BankoModuAktif)
                            {
                                var bankoResult = await hubConnectionService.DeactivateBankoConnectionAsync(user.TcKimlikNo);
                                if (bankoResult)
                                {
                                    bankoCleanupCount++;
                                    _logger.LogInformation("🏦 Idle timeout: Banko modu kapatıldı - TC: {TcKimlikNo}", user.TcKimlikNo);
                                }
                            }

                            // 2. LoginLogoutLog kaydının LogoutTime'ını güncelle
                            var sessionId = user.SessionID;
                            var result = await loginLogoutLogService.UpdateLogoutTimeBySessionIdAsync(sessionId);

                            if (result.Success && result.Data)
                            {
                                _logger.LogInformation("✅ Idle session logout edildi - TC: {TcKimlikNo}, SessionID: {SessionID}",
                                    user.TcKimlikNo, sessionId);
                                logoutCount++;
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ Idle session logout başarısız - TC: {TcKimlikNo}, SessionID: {SessionID}",
                                    user.TcKimlikNo, sessionId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Idle session logout hatası - TC: {TcKimlikNo}", user.TcKimlikNo);
                        }
                    }
                }

                if (logoutCount > 0)
                {
                    _logger.LogInformation("✅ Idle session cleanup tamamlandı - {Count} kullanıcı logout edildi, {BankoCount} banko modu kapatıldı",
                        logoutCount, bankoCleanupCount);
                }
                else
                {
                    _logger.LogDebug("ℹ️ Idle session cleanup tamamlandı - Logout edilecek kullanıcı yok");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Idle session cleanup sırasında genel hata");
                throw;
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// 30 dakika boyunca aktif olmayan (idle) session'ları otomatik logout yapan background service
    /// Her 5 dakikada bir çalışır
    /// </summary>
    public class IdleSessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IdleSessionCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // 5 dakikada bir kontrol
        private readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(30); // 30 dakika idle timeout

        public IdleSessionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<IdleSessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 IdleSessionCleanupService başlatıldı - Interval: {Interval} dakika, Timeout: {Timeout} dakika",
                _checkInterval.TotalMinutes, _idleTimeout.TotalMinutes);

            // İlk çalışmadan önce 1 dakika bekle (startup sırasında yük olmaması için)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupIdleSessionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Idle session cleanup sırasında hata oluştu");
                }

                // Bir sonraki çalışmaya kadar bekle
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("⏹️ IdleSessionCleanupService durduruldu");
        }

        private async Task CleanupIdleSessionsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var loginLogoutLogService = scope.ServiceProvider.GetRequiredService<ILoginLogoutLogService>();

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
                            // LoginLogoutLog kaydının LogoutTime'ını güncelle
                            var sessionId = user.SessionID;
                            var result = await loginLogoutLogService.UpdateLogoutTimeBySessionIdAsync(sessionId);

                            if (result.Success && result.Data)
                            {
                                _logger.LogInformation("✅ Idle session logout edildi - TC: {TcKimlikNo}, SessionID: {SessionID}",
                                    user.TcKimlikNo, sessionId);
                                logoutCount++;

                                // User'ın SessionID'sini temizle (opsiyonel - bu sayede bir sonraki kontrolde atlanır)
                                // user.SessionID = null;
                                // await unitOfWork.SaveChangesAsync();
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
                    _logger.LogInformation("✅ Idle session cleanup tamamlandı - {Count} kullanıcı logout edildi", logoutCount);
                }
                else
                {
                    _logger.LogDebug("ℹ️ Idle session cleanup tamamlandı - Logout edilecek kullanıcı yok");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Idle session cleanup sırasında genel hata");
            }
        }
    }
}

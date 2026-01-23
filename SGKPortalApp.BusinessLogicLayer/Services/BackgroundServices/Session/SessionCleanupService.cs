using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServices.Session
{
    /// <summary>
    /// Orphan/stale session ve connection'ları temizleyen background service
    /// Her 10 dakikada bir çalışır
    /// </summary>
    public class SessionCleanupService : BackgroundService, IManagedBackgroundService
    {
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);
        private bool _isRunning;

        // IManagedBackgroundService properties
        public string ServiceName => "SessionCleanupService";
        public string DisplayName => "Session Temizleme Servisi";
        public bool IsRunning => _isRunning;
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; private set; }
        public DateTime? NextRunTime { get; private set; }
        public TimeSpan Interval
        {
            get => _cleanupInterval;
            set => _cleanupInterval = value;
        }
        public string? LastError { get; private set; }
        public int SuccessCount { get; private set; }
        public int ErrorCount { get; private set; }

        public SessionCleanupService(
            ILogger<SessionCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task TriggerAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 {ServiceName} manuel tetiklendi", ServiceName);
            await CleanupOrphanSessionsAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🧹 {ServiceName} başlatıldı - Temizlik aralığı: {Interval}", ServiceName, _cleanupInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    NextRunTime = DateTime.Now.Add(_cleanupInterval);
                    await Task.Delay(_cleanupInterval, stoppingToken);

                    if (IsPaused)
                    {
                        _logger.LogDebug("⏸️ {ServiceName} duraklatıldı, atlanıyor...", ServiceName);
                        continue;
                    }

                    _isRunning = true;
                    await CleanupOrphanSessionsAsync(stoppingToken);
                    LastRunTime = DateTime.Now;
                    LastError = null;
                    SuccessCount++;
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("🛑 {ServiceName} durduruldu", ServiceName);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ {ServiceName} hatası", ServiceName);
                    LastError = ex.Message;
                    ErrorCount++;
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }

        private async Task CleanupOrphanSessionsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var loginLogoutService = scope.ServiceProvider.GetRequiredService<ILoginLogoutLogService>();

            try
            {
                var result = await loginLogoutService.CleanupOrphanSessionsAsync();

                if (result.Success && result.Data > 0)
                {
                    _logger.LogInformation("✅ Cleanup tamamlandı - {Count} kayıt temizlendi", result.Data);
                }
                else if (!result.Success)
                {
                    _logger.LogError("❌ Cleanup başarısız: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CleanupOrphanSessionsAsync hatası");
                throw;
            }
        }
    }
}

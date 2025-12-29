using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// Orphan/stale session ve connection'ları temizleyen background service
    /// Her 10 dakikada bir çalışır
    /// </summary>
    public class SessionCleanupService : BackgroundService
    {
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);

        public SessionCleanupService(
            ILogger<SessionCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🧹 SessionCleanupService başlatıldı - Temizlik aralığı: {Interval}", _cleanupInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    await CleanupOrphanSessionsAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("🛑 SessionCleanupService durduruldu");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ SessionCleanupService hatası");
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
            }
        }
    }
}

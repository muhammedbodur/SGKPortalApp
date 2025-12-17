using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;

namespace SGKPortalApp.PresentationLayer.Services.BackgroundServices
{
    /// <summary>
    /// Stale (eski/geçersiz) SignalR connection'larını periyodik olarak temizler.
    /// Sunucu restart'larında veya beklenmedik bağlantı kopmalarında
    /// "online" olarak kalan kayıtları "offline" yapar.
    /// </summary>
    public class StaleConnectionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StaleConnectionCleanupService> _logger;
        
        // Temizlik aralığı (5 dakikada bir kontrol)
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);
        
        // Stale kabul edilme süresi (10 dakika aktivite yoksa)
        private readonly TimeSpan _staleThreshold = TimeSpan.FromMinutes(10);

        public StaleConnectionCleanupService(
            IServiceProvider serviceProvider,
            ILogger<StaleConnectionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StaleConnectionCleanupService başlatıldı. Aralık: {Interval}, Threshold: {Threshold}",
                _cleanupInterval, _staleThreshold);

            // İlk başlangıçta tüm online connection'ları offline yap (sunucu restart)
            await CleanupAllOnStartupAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    await CleanupStaleConnectionsAsync();
                }
                catch (OperationCanceledException)
                {
                    // Uygulama kapanıyor, normal durum
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "StaleConnectionCleanupService hatası");
                }
            }

            _logger.LogInformation("StaleConnectionCleanupService durduruluyor...");
        }

        /// <summary>
        /// Uygulama başlangıcında tüm online connection'ları offline yap.
        /// Sunucu restart'larında eski kayıtlar temizlenir.
        /// </summary>
        private async Task CleanupAllOnStartupAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubConnectionRepo = unitOfWork.GetRepository<IHubConnectionRepository>();

                var onlineConnections = await hubConnectionRepo.GetActiveConnectionsAsync();
                var toCleanup = onlineConnections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online && !c.SilindiMi)
                    .ToList();

                if (!toCleanup.Any())
                {
                    _logger.LogInformation("Başlangıç temizliği: Temizlenecek online connection yok");
                    return;
                }

                foreach (var conn in toCleanup)
                {
                    conn.ConnectionStatus = ConnectionStatus.offline;
                    conn.DuzenlenmeTarihi = DateTime.Now;
                    hubConnectionRepo.Update(conn);
                }

                await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Başlangıç temizliği: {Count} connection offline yapıldı", toCleanup.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başlangıç temizliği hatası");
            }
        }

        /// <summary>
        /// Stale connection'ları temizle (LastActivityAt + threshold geçmişse)
        /// </summary>
        private async Task CleanupStaleConnectionsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubConnectionRepo = unitOfWork.GetRepository<IHubConnectionRepository>();

                var cutoffTime = DateTime.Now.Subtract(_staleThreshold);
                
                var onlineConnections = await hubConnectionRepo.GetActiveConnectionsAsync();
                var staleConnections = onlineConnections
                    .Where(c => c.ConnectionStatus == ConnectionStatus.online 
                             && !c.SilindiMi 
                             && c.LastActivityAt < cutoffTime)
                    .ToList();

                if (!staleConnections.Any())
                {
                    _logger.LogDebug("Stale connection temizliği: Temizlenecek kayıt yok");
                    return;
                }

                foreach (var conn in staleConnections)
                {
                    conn.ConnectionStatus = ConnectionStatus.offline;
                    conn.DuzenlenmeTarihi = DateTime.Now;
                    hubConnectionRepo.Update(conn);
                }

                await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Stale connection temizliği: {Count} connection offline yapıldı (Threshold: {Threshold})",
                    staleConnections.Count, _staleThreshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stale connection temizliği hatası");
            }
        }
    }
}

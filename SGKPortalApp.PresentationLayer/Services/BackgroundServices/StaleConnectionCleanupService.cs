using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.BackgroundServices
{
    /// <summary>
    /// Stale (eski/geçersiz) SignalR connection'larını periyodik olarak temizler.
    /// API endpoint'ler üzerinden temizlik yapar (Layered Architecture uyumlu)
    /// </summary>
    public class StaleConnectionCleanupService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StaleConnectionCleanupService> _logger;

        // Temizlik aralığı (5 dakikada bir kontrol)
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

        // Stale kabul edilme süresi (10 dakika aktivite yoksa)
        private readonly int _staleThresholdMinutes = 10;

        public StaleConnectionCleanupService(
            IHttpClientFactory httpClientFactory,
            ILogger<StaleConnectionCleanupService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StaleConnectionCleanupService başlatıldı. Aralık: {Interval}, Threshold: {Threshold} dakika",
                _cleanupInterval, _staleThresholdMinutes);

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
        /// API endpoint üzerinden çağrılır (Layered Architecture)
        /// </summary>
        private async Task CleanupAllOnStartupAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                var response = await httpClient.PostAsync("/api/hub-connections/cleanup/startup", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CleanupResponse>();
                    _logger.LogInformation("✅ Başlangıç temizliği: {Count} connection offline yapıldı",
                        result?.CleanedCount ?? 0);
                }
                else
                {
                    _logger.LogWarning("⚠️ Başlangıç temizliği API çağrısı başarısız: {StatusCode}",
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başlangıç temizliği hatası (API çağrısı)");
            }
        }

        /// <summary>
        /// Stale connection'ları temizle (LastActivityAt + threshold geçmişse)
        /// API endpoint üzerinden çağrılır (Layered Architecture)
        /// </summary>
        private async Task CleanupStaleConnectionsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                var response = await httpClient.PostAsync(
                    $"/api/hub-connections/cleanup/stale?staleThresholdMinutes={_staleThresholdMinutes}",
                    null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CleanupResponse>();
                    if (result?.CleanedCount > 0)
                    {
                        _logger.LogInformation("✅ Stale connection temizliği: {Count} connection offline yapıldı",
                            result.CleanedCount);
                    }
                    else
                    {
                        _logger.LogDebug("Stale connection temizliği: Temizlenecek kayıt yok");
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ Stale connection temizliği API çağrısı başarısız: {StatusCode}",
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stale connection temizliği hatası (API çağrısı)");
            }
        }

        /// <summary>
        /// API cleanup endpoint response DTO
        /// </summary>
        private class CleanupResponse
        {
            public int CleanedCount { get; set; }
            public string? Message { get; set; }
        }
    }
}

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

        // Ana loop aralığı (120 saniye = 2 dakika)
        private readonly TimeSpan _loopInterval = TimeSpan.FromSeconds(120);

        // Stale cleanup aralığı (5 dakika)
        private readonly TimeSpan _staleCleanupInterval = TimeSpan.FromMinutes(5);

        // Stale kabul edilme süresi (10 dakika aktivite yoksa)
        private readonly int _staleThresholdMinutes = 10;

        // Orphan cleanup aralığı (30 dakika) - nadiren gerekli, agresif olmamalı
        private readonly TimeSpan _orphanCleanupInterval = TimeSpan.FromMinutes(30);

        private DateTime _lastStaleCleanup = DateTime.MinValue;
        private DateTime _lastOrphanCleanup = DateTime.MinValue;

        public StaleConnectionCleanupService(
            IHttpClientFactory httpClientFactory,
            ILogger<StaleConnectionCleanupService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StaleConnectionCleanupService başlatıldı. Loop: {LoopInterval}, Stale: {StaleInterval}, Orphan: {OrphanInterval}",
                _loopInterval, _staleCleanupInterval, _orphanCleanupInterval);

            // İlk başlangıçta tüm online connection'ları offline yap (sunucu restart)
            await CleanupAllOnStartupAsync();

            _logger.LogInformation("Periyodik temizlik başlatılıyor...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Her 120 saniyede bir loop
                    await Task.Delay(_loopInterval, stoppingToken);

                    // Stale cleanup (5 dakikada bir - 10 dk aktivite yoksa offline yapar)
                    var timeSinceLastStaleCleanup = DateTime.Now - _lastStaleCleanup;
                    if (timeSinceLastStaleCleanup >= _staleCleanupInterval)
                    {
                        await CleanupStaleConnectionsAsync();
                        _lastStaleCleanup = DateTime.Now;
                    }

                    // Orphan cleanup (30 dakikada bir - offline olmuş banko connection'ları temizler)
                    var timeSinceLastOrphanCleanup = DateTime.Now - _lastOrphanCleanup;
                    if (timeSinceLastOrphanCleanup >= _orphanCleanupInterval)
                    {
                        await CleanupOrphanConnectionsAsync();
                        _lastOrphanCleanup = DateTime.Now;
                    }
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
                var httpClient = _httpClientFactory.CreateClient("CleanupClient");
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
                var httpClient = _httpClientFactory.CreateClient("CleanupClient");
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
        /// Orphan HubBankoConnection ve HubTvConnection kayıtlarını temizle
        /// API endpoint üzerinden çağrılır (Layered Architecture)
        /// </summary>
        private async Task CleanupOrphanConnectionsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("CleanupClient");

                // Orphan Banko Cleanup
                var bankoResponse = await httpClient.PostAsync("/api/hub-connections/cleanup/orphan-banko", null);
                if (bankoResponse.IsSuccessStatusCode)
                {
                    var bankoResult = await bankoResponse.Content.ReadFromJsonAsync<CleanupResponse>();
                    if (bankoResult?.CleanedCount > 0)
                    {
                        _logger.LogInformation("🧹 Orphan Banko temizliği: {Count} kayıt temizlendi", bankoResult.CleanedCount);
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ Orphan Banko temizliği API çağrısı başarısız: {StatusCode}", bankoResponse.StatusCode);
                }

                // Orphan TV Cleanup
                var tvResponse = await httpClient.PostAsync("/api/hub-connections/cleanup/orphan-tv", null);
                if (tvResponse.IsSuccessStatusCode)
                {
                    var tvResult = await tvResponse.Content.ReadFromJsonAsync<CleanupResponse>();
                    if (tvResult?.CleanedCount > 0)
                    {
                        _logger.LogInformation("🧹 Orphan TV temizliği: {Count} kayıt temizlendi", tvResult.CleanedCount);
                    }
                }
                else
                {
                    _logger.LogWarning("⚠️ Orphan TV temizliği API çağrısı başarısız: {StatusCode}", tvResponse.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Orphan connection temizliği hatası (API çağrısı)");
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

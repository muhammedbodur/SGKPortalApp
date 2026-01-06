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

        // Ana loop aralığı (5 dakika)
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        // Stale kabul edilme süresi (10 dakika aktivite yoksa)
        private readonly int _staleThresholdMinutes = 10;

        // Orphan cleanup aralığı (30 dakika) - nadiren gerekli, agresif olmamalı
        private readonly TimeSpan _orphanCleanupInterval = TimeSpan.FromMinutes(30);

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
            _logger.LogInformation("StaleConnectionCleanupService başlatıldı. Interval: {Interval} dakika, Stale Threshold: {StaleThreshold} dakika, Orphan Interval: {OrphanInterval} dakika",
                _checkInterval.TotalMinutes, _staleThresholdMinutes, _orphanCleanupInterval.TotalMinutes);

            // İlk başlangıçta tüm online connection'ları offline yap (sunucu restart)
            // NOT: ApiLayer henüz hazır olmayabilir, retry ile bekle
            await CleanupAllOnStartupAsync();

            _logger.LogInformation("Periyodik temizlik başlatılıyor (her {Interval} dakikada bir)...", _checkInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Her 5 dakikada bir çalış
                    await Task.Delay(_checkInterval, stoppingToken);

                    // Stale cleanup (her çalışmada - 10 dk aktivite yoksa offline yapar)
                    await CleanupStaleConnectionsAsync();

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
        /// NOT: ApiLayer henüz hazır olmayabilir, retry ile bekler (max 30 saniye)
        /// </summary>
        private async Task CleanupAllOnStartupAsync()
        {
            const int maxRetries = 6; // 6 deneme x 5 saniye = 30 saniye max bekleme
            const int retryDelaySeconds = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
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
                        return; // Başarılı, çık
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Başlangıç temizliği API çağrısı başarısız: {StatusCode}",
                            response.StatusCode);
                    }
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    // API henüz hazır değil, retry yap
                    _logger.LogWarning("⏳ ApiLayer henüz hazır değil (deneme {Attempt}/{MaxRetries}). {Delay} saniye sonra tekrar denenecek...",
                        attempt, maxRetries, retryDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
                catch (HttpRequestException ex) when (attempt == maxRetries)
                {
                    // Son deneme de başarısız, pes et
                    _logger.LogWarning(ex, "⚠️ Başlangıç temizliği yapılamadı: ApiLayer {MaxRetries} denemede hazır olmadı. Devam ediliyor...",
                        maxRetries);
                    return; // Critical değil, servis devam etsin
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Başlangıç temizliği hatası (API çağrısı)");
                    return; // Critical değil, servis devam etsin
                }
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

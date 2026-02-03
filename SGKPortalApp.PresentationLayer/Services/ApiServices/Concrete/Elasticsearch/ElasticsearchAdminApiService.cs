using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Elasticsearch
{
    public class ElasticsearchAdminApiService : IElasticsearchAdminApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ElasticsearchAdminApiService> _logger;

        public ElasticsearchAdminApiService(HttpClient httpClient, ILogger<ElasticsearchAdminApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<IndexStatusInfo>> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("personelsearch/status");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetStatusAsync failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    return ServiceResult<IndexStatusInfo>.Fail($"Durum bilgisi alınamadı: {response.StatusCode}");
                }

                var status = await response.Content.ReadFromJsonAsync<IndexStatusInfo>();
                if (status != null)
                {
                    return ServiceResult<IndexStatusInfo>.Ok(status, "Durum bilgisi alındı");
                }

                return ServiceResult<IndexStatusInfo>.Fail("Durum bilgisi okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatusAsync Exception");
                return ServiceResult<IndexStatusInfo>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> PingAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("personelsearch/ping");

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Ok(true, "Elasticsearch bağlantısı başarılı");
                }

                return ServiceResult<bool>.Fail("Elasticsearch bağlantısı kurulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PingAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CreateIndexAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("personelsearch/create-index", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateIndexResponse>();
                    return ServiceResult<bool>.Ok(true, result?.Message ?? "Index başarıyla oluşturuldu");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<CreateIndexResponse>();
                return ServiceResult<bool>.Fail(errorResult?.Message ?? "Index oluşturulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateIndexAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> FullReindexAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("personelsearch/full-reindex", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReindexResponse>();
                    var count = result?.IndexedCount ?? 0;
                    return ServiceResult<int>.Ok(count, result?.Message ?? $"{count} personel indexlendi");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<ReindexResponse>();
                return ServiceResult<int>.Fail(errorResult?.Message ?? "Reindex başarısız");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FullReindexAsync Exception");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> IncrementalSyncAsync(int sinceHours = 24)
        {
            try
            {
                var response = await _httpClient.PostAsync($"personelsearch/incremental-sync?sinceHours={sinceHours}", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReindexResponse>();
                    var count = result?.IndexedCount ?? 0;
                    return ServiceResult<int>.Ok(count, result?.Message ?? $"{count} personel güncellendi");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<ReindexResponse>();
                return ServiceResult<int>.Fail(errorResult?.Message ?? "Senkronizasyon başarısız");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IncrementalSyncAsync Exception");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<long>> GetDocumentCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("personelsearch/count");

                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadFromJsonAsync<long>();
                    return ServiceResult<long>.Ok(count, "Doküman sayısı alındı");
                }

                return ServiceResult<long>.Fail("Doküman sayısı alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDocumentCountAsync Exception");
                return ServiceResult<long>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteIndexAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("personelsearch/delete-index");

                if (response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Ok(true, "Index başarıyla silindi");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("DeleteIndexAsync failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return ServiceResult<bool>.Fail($"Index silinemedi: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteIndexAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

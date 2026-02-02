using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Elasticsearch
{
    /// <summary>
    /// Elasticsearch personel arama API servisi implementasyonu
    /// </summary>
    public class PersonelSearchApiService : IPersonelSearchApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PersonelSearchApiService> _logger;

        public PersonelSearchApiService(HttpClient httpClient, ILogger<PersonelSearchApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<PersonelElasticDto>>> SearchAsync(
            string query,
            string? departmanIds = null,
            bool sadeceAktif = true,
            int size = 20)
        {
            try
            {
                var url = $"personelsearch/search?q={Uri.EscapeDataString(query)}&sadeceAktif={sadeceAktif}&size={size}";

                if (!string.IsNullOrWhiteSpace(departmanIds))
                {
                    url += $"&departmanIds={Uri.EscapeDataString(departmanIds)}";
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Elasticsearch SearchAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelElasticDto>>.Fail("Arama yapılamadı.");
                }

                var results = await response.Content.ReadFromJsonAsync<List<PersonelElasticDto>>();

                if (results != null)
                {
                    return ServiceResult<List<PersonelElasticDto>>.Ok(results, "Arama başarılı");
                }

                return ServiceResult<List<PersonelElasticDto>>.Fail("Arama sonucu alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch SearchAsync Exception");
                return ServiceResult<List<PersonelElasticDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<PersonelElasticDto>>> AutocompleteAsync(
            string prefix,
            string? departmanIds = null,
            bool sadeceAktif = true,
            int size = 10)
        {
            try
            {
                var url = $"personelsearch/autocomplete?prefix={Uri.EscapeDataString(prefix)}&sadeceAktif={sadeceAktif}&size={size}";

                if (!string.IsNullOrWhiteSpace(departmanIds))
                {
                    url += $"&departmanIds={Uri.EscapeDataString(departmanIds)}";
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Elasticsearch AutocompleteAsync failed: {Error}", errorContent);
                    return ServiceResult<List<PersonelElasticDto>>.Fail("Autocomplete araması yapılamadı.");
                }

                var results = await response.Content.ReadFromJsonAsync<List<PersonelElasticDto>>();

                if (results != null)
                {
                    return ServiceResult<List<PersonelElasticDto>>.Ok(results, "Autocomplete başarılı");
                }

                return ServiceResult<List<PersonelElasticDto>>.Fail("Autocomplete sonucu alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch AutocompleteAsync Exception");
                return ServiceResult<List<PersonelElasticDto>>.Fail($"Hata: {ex.Message}");
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
                _logger.LogError(ex, "Elasticsearch PingAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

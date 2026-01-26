// ════════════════════════════════════════════════════════════════
// 📁 PresentationLayer/Services/ApiServices/Concrete/Common/BackgroundServiceApiService.cs
// ════════════════════════════════════════════════════════════════
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class BackgroundServiceApiService : IBackgroundServiceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BackgroundServiceApiService> _logger;

        public BackgroundServiceApiService(HttpClient httpClient, ILogger<BackgroundServiceApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<BackgroundServiceStatusDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("backgroundservice");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<BackgroundServiceStatusDto>>.Fail("Background servisler listesi alınamadı.");
                }

                var services = await response.Content.ReadFromJsonAsync<List<BackgroundServiceStatusDto>>();

                if (services != null)
                {
                    return ServiceResult<List<BackgroundServiceStatusDto>>.Ok(services, "İşlem başarılı");
                }

                return ServiceResult<List<BackgroundServiceStatusDto>>.Fail("Background servisler listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<BackgroundServiceStatusDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BackgroundServiceStatusDto>> GetAsync(string serviceName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"backgroundservice/{serviceName}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAsync failed for {ServiceName}: {Error}", serviceName, errorContent);
                    return ServiceResult<BackgroundServiceStatusDto>.Fail($"Servis bilgisi alınamadı: {serviceName}");
                }

                var service = await response.Content.ReadFromJsonAsync<BackgroundServiceStatusDto>();

                if (service != null)
                {
                    return ServiceResult<BackgroundServiceStatusDto>.Ok(service, "İşlem başarılı");
                }

                return ServiceResult<BackgroundServiceStatusDto>.Fail("Servis bilgisi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAsync exception for {ServiceName}", serviceName);
                return ServiceResult<BackgroundServiceStatusDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> TriggerAsync(string serviceName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"backgroundservice/{serviceName}/trigger", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("TriggerAsync failed for {ServiceName}: {Error}", serviceName, errorContent);
                    return ServiceResult<string>.Fail($"Servis tetiklenemedi: {serviceName}");
                }

                return ServiceResult<string>.Ok("", $"Servis tetiklendi: {serviceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TriggerAsync exception for {ServiceName}", serviceName);
                return ServiceResult<string>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> PauseAsync(string serviceName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"backgroundservice/{serviceName}/pause", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("PauseAsync failed for {ServiceName}: {Error}", serviceName, errorContent);
                    return ServiceResult<string>.Fail($"Servis duraklatılamadı: {serviceName}");
                }

                return ServiceResult<string>.Ok("", $"Servis duraklatıldı: {serviceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PauseAsync exception for {ServiceName}", serviceName);
                return ServiceResult<string>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> ResumeAsync(string serviceName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"backgroundservice/{serviceName}/resume", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ResumeAsync failed for {ServiceName}: {Error}", serviceName, errorContent);
                    return ServiceResult<string>.Fail($"Servis devam ettirilemedi: {serviceName}");
                }

                return ServiceResult<string>.Ok("", $"Servis devam ettiriliyor: {serviceName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResumeAsync exception for {ServiceName}", serviceName);
                return ServiceResult<string>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> SetIntervalAsync(string serviceName, int intervalMinutes)
        {
            try
            {
                var request = new { IntervalMinutes = intervalMinutes };
                var response = await _httpClient.PostAsJsonAsync($"backgroundservice/{serviceName}/interval", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SetIntervalAsync failed for {ServiceName}: {Error}", serviceName, errorContent);
                    return ServiceResult<string>.Fail($"Servis aralığı değiştirilemedi: {serviceName}");
                }

                return ServiceResult<string>.Ok("", $"Servis aralığı değiştirildi: {serviceName} -> {intervalMinutes} dakika");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetIntervalAsync exception for {ServiceName}", serviceName);
                return ServiceResult<string>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

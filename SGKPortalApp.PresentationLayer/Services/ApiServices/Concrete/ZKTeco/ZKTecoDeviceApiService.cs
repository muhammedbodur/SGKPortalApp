using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class ZKTecoDeviceApiService : IZKTecoDeviceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ZKTecoDeviceApiService> _logger;

        public ZKTecoDeviceApiService(HttpClient httpClient, ILogger<ZKTecoDeviceApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DeviceResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DeviceResponseDto>>.Fail("Cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DeviceResponseDto>>>();
                return result ?? ServiceResult<List<DeviceResponseDto>>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync failed");
                return ServiceResult<List<DeviceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DeviceResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device/active");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DeviceResponseDto>>.Fail("Aktif cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DeviceResponseDto>>>();
                return result ?? ServiceResult<List<DeviceResponseDto>>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync failed");
                return ServiceResult<List<DeviceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceResponseDto>>();
                return result ?? ServiceResult<DeviceResponseDto>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed");
                return ServiceResult<DeviceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceStatusDto>> GetStatusAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/status");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceStatusDto>.Fail("Cihaz durumu alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceStatusDto>>();
                return result ?? ServiceResult<DeviceStatusDto>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatusAsync failed");
                return ServiceResult<DeviceStatusDto>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

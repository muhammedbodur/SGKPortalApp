using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KioskMenuAtamaApiService : IKioskMenuAtamaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KioskMenuAtamaApiService> _logger;

        public KioskMenuAtamaApiService(HttpClient httpClient, ILogger<KioskMenuAtamaApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KioskMenuAtamaResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<List<KioskMenuAtamaResponseDto>>>("api/kiosk-menu-atama");
                return response ?? ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü atamaları getirilemedi");
                return ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KioskMenuAtamaResponseDto>>> GetByKioskAsync(int kioskId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<List<KioskMenuAtamaResponseDto>>>($"api/kiosk-menu-atama/kiosk/{kioskId}");
                return response ?? ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü atamaları getirilemedi. KioskId: {KioskId}", kioskId);
                return ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuAtamaResponseDto>> GetActiveByKioskAsync(int kioskId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>($"api/kiosk-menu-atama/kiosk/{kioskId}/active");
                return response ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif menü ataması getirilemedi. KioskId: {KioskId}", kioskId);
                return ServiceResult<KioskMenuAtamaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuAtamaResponseDto>> GetByIdAsync(int kioskMenuAtamaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>($"api/kiosk-menu-atama/{kioskMenuAtamaId}");
                return response ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması getirilemedi. Id: {Id}", kioskMenuAtamaId);
                return ServiceResult<KioskMenuAtamaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuAtamaResponseDto>> CreateAsync(KioskMenuAtamaCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/kiosk-menu-atama", request);
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>();
                return result ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması oluşturulamadı");
                return ServiceResult<KioskMenuAtamaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuAtamaResponseDto>> UpdateAsync(KioskMenuAtamaUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/kiosk-menu-atama", request);
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>();
                return result ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması güncellenemedi");
                return ServiceResult<KioskMenuAtamaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int kioskMenuAtamaId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/kiosk-menu-atama/{kioskMenuAtamaId}");
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması silinemedi. Id: {Id}", kioskMenuAtamaId);
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KioskMenuIslemApiService : IKioskMenuIslemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KioskMenuIslemApiService> _logger;

        public KioskMenuIslemApiService(HttpClient httpClient, ILogger<KioskMenuIslemApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KioskMenuIslemResponseDto>>> GetByKioskMenuAsync(int kioskMenuId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<List<KioskMenuIslemResponseDto>>>($"api/kiosk-menu-islem/menu/{kioskMenuId}");
                return response ?? ServiceResult<List<KioskMenuIslemResponseDto>>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemleri getirilemedi. KioskMenuId: {KioskMenuId}", kioskMenuId);
                return ServiceResult<List<KioskMenuIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuIslemResponseDto>> GetByIdAsync(int kioskMenuIslemId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResult<KioskMenuIslemResponseDto>>($"api/kiosk-menu-islem/{kioskMenuIslemId}");
                return response ?? ServiceResult<KioskMenuIslemResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemi getirilemedi. Id: {Id}", kioskMenuIslemId);
                return ServiceResult<KioskMenuIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuIslemResponseDto>> CreateAsync(KioskMenuIslemCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/kiosk-menu-islem", request);
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuIslemResponseDto>>();
                return result ?? ServiceResult<KioskMenuIslemResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemi oluşturulamadı");
                return ServiceResult<KioskMenuIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuIslemResponseDto>> UpdateAsync(KioskMenuIslemUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/kiosk-menu-islem", request);
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuIslemResponseDto>>();
                return result ?? ServiceResult<KioskMenuIslemResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemi güncellenemedi");
                return ServiceResult<KioskMenuIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int kioskMenuIslemId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/kiosk-menu-islem/{kioskMenuIslemId}");
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü işlemi silinemedi. Id: {Id}", kioskMenuIslemId);
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateSiraAsync(int kioskMenuIslemId, int yeniSira)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/kiosk-menu-islem/{kioskMenuIslemId}/sira/{yeniSira}", null);
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sıra güncellenemedi. Id: {Id}", kioskMenuIslemId);
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

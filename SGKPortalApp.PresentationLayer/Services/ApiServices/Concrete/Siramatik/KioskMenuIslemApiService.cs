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
                var response = await _httpClient.GetAsync($"kiosk-menu-islem/menu/{kioskMenuId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKioskMenuAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KioskMenuIslemResponseDto>>.Fail("Menü işlemleri alınamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<KioskMenuIslemResponseDto>>>();
                return result ?? ServiceResult<List<KioskMenuIslemResponseDto>>.Fail("Yanıt alınamadı");
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
                var response = await _httpClient.GetAsync($"kiosk-menu-islem/{kioskMenuIslemId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuIslemResponseDto>.Fail("Menü işlemi bulunamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuIslemResponseDto>>();
                return result ?? ServiceResult<KioskMenuIslemResponseDto>.Fail("Yanıt alınamadı");
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
                var response = await _httpClient.PostAsJsonAsync("kiosk-menu-islem", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuIslemResponseDto>.Fail("Menü işlemi oluşturulamadı.");
                }

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
                var response = await _httpClient.PutAsJsonAsync("kiosk-menu-islem", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuIslemResponseDto>.Fail("Menü işlemi güncellenemedi.");
                }

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
                var response = await _httpClient.DeleteAsync($"kiosk-menu-islem/{kioskMenuIslemId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Menü işlemi silinemedi.");
                }

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
                var response = await _httpClient.PutAsync($"kiosk-menu-islem/{kioskMenuIslemId}/sira/{yeniSira}", null);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateSiraAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Sıra güncellenemedi.");
                }

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

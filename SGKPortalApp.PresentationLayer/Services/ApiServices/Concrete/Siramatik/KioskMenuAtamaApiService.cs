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
                var response = await _httpClient.GetAsync("kiosk-menu-atama");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Menü atama listesi alınamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<KioskMenuAtamaResponseDto>>>();
                return result ?? ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü atamaları listelenemedi");
                return ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KioskMenuAtamaResponseDto>>> GetByKioskAsync(int kioskId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kiosk-menu-atama/kiosk/{kioskId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKioskAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Kiosk menü atamaları alınamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<KioskMenuAtamaResponseDto>>>();
                return result ?? ServiceResult<List<KioskMenuAtamaResponseDto>>.Fail("Yanıt alınamadı");
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
                var response = await _httpClient.GetAsync($"kiosk-menu-atama/kiosk/{kioskId}/active");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveByKioskAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuAtamaResponseDto>.Fail("Aktif menü ataması alınamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>();
                return result ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
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
                var response = await _httpClient.GetAsync($"kiosk-menu-atama/{kioskMenuAtamaId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuAtamaResponseDto>.Fail("Menü ataması bulunamadı.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>();
                return result ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
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
                var response = await _httpClient.PostAsJsonAsync("kiosk-menu-atama", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuAtamaResponseDto>.Fail("Menü ataması oluşturulamadı.");
                }

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
                var response = await _httpClient.PutAsJsonAsync($"kiosk-menu-atama/{request.KioskMenuAtamaId}", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<KioskMenuAtamaResponseDto>.Fail("Menü ataması güncellenemedi.");
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<KioskMenuAtamaResponseDto>>();
                return result ?? ServiceResult<KioskMenuAtamaResponseDto>.Fail("Yanıt alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menü ataması güncellenemedi. Id: {Id}", request.KioskMenuAtamaId);
                return ServiceResult<KioskMenuAtamaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int kioskMenuAtamaId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kiosk-menu-atama/{kioskMenuAtamaId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Menü ataması silinemedi.");
                }

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

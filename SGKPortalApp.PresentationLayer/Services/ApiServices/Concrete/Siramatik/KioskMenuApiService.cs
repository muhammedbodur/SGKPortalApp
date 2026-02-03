using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KioskMenuApiService : IKioskMenuApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KioskMenuApiService> _logger;

        public KioskMenuApiService(HttpClient httpClient, ILogger<KioskMenuApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KioskMenuResponseDto>>> GetAllAsync()
        {
            return await GetListAsync("kioskmenu");
        }

        public async Task<ServiceResult<List<KioskMenuResponseDto>>> GetActiveAsync()
        {
            return await GetListAsync("kioskmenu/active");
        }

        public async Task<ServiceResult<KioskMenuResponseDto>> GetByIdAsync(int kioskMenuId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kioskmenu/{kioskMenuId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", error);
                    return ServiceResult<KioskMenuResponseDto>.Fail("Kiosk menü bulunamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskMenuResponseDto>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KioskMenuResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<KioskMenuResponseDto>.Fail(apiResponse?.Message ?? "Kiosk menü bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<KioskMenuResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KioskMenuResponseDto>> CreateAsync(KioskMenuCreateRequestDto dto)
        {
            return await SendMutationAsync(() => _httpClient.PostAsJsonAsync("kioskmenu", dto));
        }

        public async Task<ServiceResult<KioskMenuResponseDto>> UpdateAsync(KioskMenuUpdateRequestDto dto)
        {
            return await SendMutationAsync(() => _httpClient.PutAsJsonAsync("kioskmenu", dto));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int kioskMenuId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kioskmenu/{kioskMenuId}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kiosk menü silinemedi";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }
                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "Kiosk menü silindi");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Kiosk menü silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        private async Task<ServiceResult<List<KioskMenuResponseDto>>> GetListAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{Endpoint} failed: {Error}", endpoint, error);
                    return ServiceResult<List<KioskMenuResponseDto>>.Fail("Kiosk menüler alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskMenuResponseDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskMenuResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskMenuResponseDto>>.Fail(apiResponse?.Message ?? "Kiosk menüler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetListAsync exception ({Endpoint})", endpoint);
                return ServiceResult<List<KioskMenuResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        private async Task<ServiceResult<KioskMenuResponseDto>> SendMutationAsync(Func<Task<HttpResponseMessage>> httpAction)
        {
            try
            {
                var response = await httpAction();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskMenuResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "İşlem gerçekleştirilemedi";
                    _logger.LogError("KioskMenu mutation failed: {Error}", errorMessage);
                    return ServiceResult<KioskMenuResponseDto>.Fail(errorMessage);
                }
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KioskMenuResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<KioskMenuResponseDto>.Fail(apiResponse?.Message ?? "İşlem gerçekleştirilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "KioskMenu mutation exception");
                return ServiceResult<KioskMenuResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

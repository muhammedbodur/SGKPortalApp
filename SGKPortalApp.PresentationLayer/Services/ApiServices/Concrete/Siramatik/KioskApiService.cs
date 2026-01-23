using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KioskApiService : IKioskApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KioskApiService> _logger;

        public KioskApiService(HttpClient httpClient, ILogger<KioskApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KioskResponseDto>>> GetAllAsync()
            => await GetListAsync("kiosk");

        public async Task<ServiceResult<List<KioskResponseDto>>> GetActiveAsync()
            => await GetListAsync("kiosk/aktif");

        public async Task<ServiceResult<List<KioskResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId)
            => await GetListAsync($"kiosk/departman-hizmet-binasi/{hizmetBinasiId}");

        public async Task<ServiceResult<KioskResponseDto>> GetByIdAsync(int kioskId)
            => await GetSingleAsync($"kiosk/{kioskId}");

        public async Task<ServiceResult<KioskResponseDto>> GetWithMenuAsync(int kioskId)
            => await GetSingleAsync($"kiosk/{kioskId}/with-menu");

        public async Task<ServiceResult<KioskResponseDto>> CreateAsync(KioskCreateRequestDto dto)
            => await SendMutationAsync(() => _httpClient.PostAsJsonAsync("kiosk", dto));

        public async Task<ServiceResult<KioskResponseDto>> UpdateAsync(KioskUpdateRequestDto dto)
            => await SendMutationAsync(() => _httpClient.PutAsJsonAsync("kiosk", dto));

        public async Task<ServiceResult<bool>> DeleteAsync(int kioskId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kiosk/{kioskId}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kiosk silinemedi";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }
                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "Kiosk silindi");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Kiosk silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        private async Task<ServiceResult<List<KioskResponseDto>>> GetListAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{Endpoint} failed: {Error}", endpoint, error);
                    return ServiceResult<List<KioskResponseDto>>.Fail("Kiosk verileri alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KioskResponseDto>>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KioskResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<KioskResponseDto>>.Fail(apiResponse?.Message ?? "Kiosk verileri alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetListAsync exception ({Endpoint})", endpoint);
                return ServiceResult<List<KioskResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        private async Task<ServiceResult<KioskResponseDto>> GetSingleAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{Endpoint} failed: {Error}", endpoint, error);
                    return ServiceResult<KioskResponseDto>.Fail("Kiosk bulunamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskResponseDto>>();
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KioskResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<KioskResponseDto>.Fail(apiResponse?.Message ?? "Kiosk bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSingleAsync exception ({Endpoint})", endpoint);
                return ServiceResult<KioskResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        private async Task<ServiceResult<KioskResponseDto>> SendMutationAsync(Func<Task<HttpResponseMessage>> httpAction)
        {
            try
            {
                var response = await httpAction();
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KioskResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "İşlem gerçekleştirilemedi";
                    _logger.LogError("Kiosk mutation failed: {Error}", errorMessage);
                    return ServiceResult<KioskResponseDto>.Fail(errorMessage);
                }
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KioskResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<KioskResponseDto>.Fail(apiResponse?.Message ?? "İşlem gerçekleştirilemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk mutation exception");
                return ServiceResult<KioskResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

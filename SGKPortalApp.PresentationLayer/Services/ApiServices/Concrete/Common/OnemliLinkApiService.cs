using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class OnemliLinkApiService : IOnemliLinkApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OnemliLinkApiService> _logger;

        public OnemliLinkApiService(HttpClient httpClient, ILogger<OnemliLinkApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<OnemliLinkResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("onemlilink");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<OnemliLinkResponseDto>>.Fail("Link listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<OnemliLinkResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<OnemliLinkResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<OnemliLinkResponseDto>>.Fail(apiResponse?.Message ?? "Link listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<OnemliLinkResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<OnemliLinkResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("onemlilink/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<OnemliLinkResponseDto>>.Fail("Aktif linkler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<OnemliLinkResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<OnemliLinkResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<OnemliLinkResponseDto>>.Fail(apiResponse?.Message ?? "Aktif linkler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync exception");
                return ServiceResult<List<OnemliLinkResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OnemliLinkResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"onemlilink/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<OnemliLinkResponseDto>.Fail("Link bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<OnemliLinkResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<OnemliLinkResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<OnemliLinkResponseDto>.Fail(apiResponse?.Message ?? "Link bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<OnemliLinkResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OnemliLinkResponseDto>> CreateAsync(OnemliLinkCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("onemlilink", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<OnemliLinkResponseDto>.Fail("Link oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<OnemliLinkResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<OnemliLinkResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<OnemliLinkResponseDto>.Fail(apiResponse?.Message ?? "Link oluşturulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync exception");
                return ServiceResult<OnemliLinkResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<OnemliLinkResponseDto>> UpdateAsync(int id, OnemliLinkUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"onemlilink/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<OnemliLinkResponseDto>.Fail("Link güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<OnemliLinkResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<OnemliLinkResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<OnemliLinkResponseDto>.Fail(apiResponse?.Message ?? "Link güncellenemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync exception");
                return ServiceResult<OnemliLinkResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"onemlilink/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Link silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Link silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>> GetPagedAsync(OnemliLinkFilterRequestDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("onemlilink/paged", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPagedAsync failed: {Error}", errorContent);
                    return ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>.Fail("Linkler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResponseDto<OnemliLinkResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>.Fail(apiResponse?.Message ?? "Linkler alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPagedAsync exception");
                return ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

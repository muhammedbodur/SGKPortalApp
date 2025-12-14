using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class YetkiApiService : IYetkiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<YetkiApiService> _logger;

        public YetkiApiService(HttpClient httpClient, ILogger<YetkiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<YetkiResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("yetki");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<YetkiResponseDto>>.Fail("Yetkiler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<YetkiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<YetkiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<YetkiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Yetkiler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<YetkiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<YetkiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"yetki/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<YetkiResponseDto>.Fail("Yetki bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<YetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<YetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<YetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Yetki bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<YetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<YetkiResponseDto>> CreateAsync(YetkiCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("yetki", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<YetkiResponseDto>.Fail("Yetki oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<YetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<YetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<YetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Yetki oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<YetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<YetkiResponseDto>> UpdateAsync(int id, YetkiUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"yetki/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<YetkiResponseDto>.Fail("Yetki güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<YetkiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<YetkiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<YetkiResponseDto>.Fail(
                    apiResponse?.Message ?? "Yetki güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<YetkiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"yetki/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Yetki silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Yetki silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<YetkiResponseDto>>> GetRootAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("yetki/root");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetRootAsync failed: {Error}", errorContent);
                    return ServiceResult<List<YetkiResponseDto>>.Fail("Kök yetkiler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<YetkiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<YetkiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<YetkiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kök yetkiler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRootAsync Exception");
                return ServiceResult<List<YetkiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<YetkiResponseDto>>> GetChildrenAsync(int ustYetkiId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"yetki/{ustYetkiId}/children");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetChildrenAsync failed: {Error}", errorContent);
                    return ServiceResult<List<YetkiResponseDto>>.Fail("Alt yetkiler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<YetkiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<YetkiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<YetkiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Alt yetkiler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetChildrenAsync Exception");
                return ServiceResult<List<YetkiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("yetki/dropdown");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDropdownAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DropdownItemDto>>.Fail("Dropdown listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DropdownItemDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DropdownItemDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DropdownItemDto>>.Fail(
                    apiResponse?.Message ?? "Dropdown listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownAsync Exception");
                return ServiceResult<List<DropdownItemDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

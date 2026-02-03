using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class ModulControllerApiService : IModulControllerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ModulControllerApiService> _logger;

        public ModulControllerApiService(HttpClient httpClient, ILogger<ModulControllerApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<ModulControllerResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modulcontroller");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", error);
                    return ServiceResult<List<ModulControllerResponseDto>>.Fail("Controller listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ModulControllerResponseDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<ModulControllerResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<ModulControllerResponseDto>>.Fail(apiResponse?.Message ?? "Controller listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<ModulControllerResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ModulControllerResponseDto>>> GetByModulIdAsync(int modulId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontroller/by-modul/{modulId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByModulIdAsync failed: {Error}", error);
                    return ServiceResult<List<ModulControllerResponseDto>>.Fail("Controller listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ModulControllerResponseDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<ModulControllerResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<ModulControllerResponseDto>>.Fail(apiResponse?.Message ?? "Controller listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByModulIdAsync Exception");
                return ServiceResult<List<ModulControllerResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontroller/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", error);
                    return ServiceResult<ModulControllerResponseDto>.Fail("Controller bulunamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerResponseDto>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<ModulControllerResponseDto>.Fail(apiResponse?.Message ?? "Controller bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<ModulControllerResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerResponseDto>> CreateAsync(ModulControllerCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("modulcontroller", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerResponseDto>>();

                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "Controller oluşturuldu")
                    : ServiceResult<ModulControllerResponseDto>.Fail(apiResponse?.Message ?? "Controller oluşturulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<ModulControllerResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerResponseDto>> UpdateAsync(int id, ModulControllerUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"modulcontroller/{id}", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerResponseDto>>();

                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "Controller güncellendi")
                    : ServiceResult<ModulControllerResponseDto>.Fail(apiResponse?.Message ?? "Controller güncellenemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<ModulControllerResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"modulcontroller/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                return apiResponse?.Success == true
                    ? ServiceResult<bool>.Ok(true, apiResponse.Message ?? "Controller silindi")
                    : ServiceResult<bool>.Fail(apiResponse?.Message ?? "Controller silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modulcontroller/dropdown");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DropdownItemDto>>.Fail("Dropdown listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DropdownItemDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<DropdownItemDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<DropdownItemDto>>.Fail(apiResponse?.Message ?? "Dropdown listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownAsync Exception");
                return ServiceResult<List<DropdownItemDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DropdownItemDto>>> GetDropdownByModulIdAsync(int modulId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontroller/dropdown/by-modul/{modulId}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DropdownItemDto>>.Fail("Dropdown listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DropdownItemDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<DropdownItemDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<DropdownItemDto>>.Fail(apiResponse?.Message ?? "Dropdown listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownByModulIdAsync Exception");
                return ServiceResult<List<DropdownItemDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class ModulControllerIslemApiService : IModulControllerIslemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ModulControllerIslemApiService> _logger;

        public ModulControllerIslemApiService(HttpClient httpClient, ILogger<ModulControllerIslemApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<ModulControllerIslemResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modulcontrollerislem");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", error);
                    return ServiceResult<List<ModulControllerIslemResponseDto>>.Fail("İşlem listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ModulControllerIslemResponseDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<ModulControllerIslemResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<ModulControllerIslemResponseDto>>.Fail(apiResponse?.Message ?? "İşlem listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<ModulControllerIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ModulControllerIslemResponseDto>>> GetByControllerIdAsync(int controllerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontrollerislem/by-controller/{controllerId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByControllerIdAsync failed: {Error}", error);
                    return ServiceResult<List<ModulControllerIslemResponseDto>>.Fail("İşlem listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ModulControllerIslemResponseDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<ModulControllerIslemResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<ModulControllerIslemResponseDto>>.Fail(apiResponse?.Message ?? "İşlem listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByControllerIdAsync Exception");
                return ServiceResult<List<ModulControllerIslemResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerIslemResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontrollerislem/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", error);
                    return ServiceResult<ModulControllerIslemResponseDto>.Fail("İşlem bulunamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerIslemResponseDto>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerIslemResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<ModulControllerIslemResponseDto>.Fail(apiResponse?.Message ?? "İşlem bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<ModulControllerIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerIslemResponseDto>> CreateAsync(ModulControllerIslemCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("modulcontrollerislem", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerIslemResponseDto>>();

                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerIslemResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem oluşturuldu")
                    : ServiceResult<ModulControllerIslemResponseDto>.Fail(apiResponse?.Message ?? "İşlem oluşturulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<ModulControllerIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulControllerIslemResponseDto>> UpdateAsync(int id, ModulControllerIslemUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"modulcontrollerislem/{id}", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulControllerIslemResponseDto>>();

                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<ModulControllerIslemResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem güncellendi")
                    : ServiceResult<ModulControllerIslemResponseDto>.Fail(apiResponse?.Message ?? "İşlem güncellenemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<ModulControllerIslemResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"modulcontrollerislem/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                return apiResponse?.Success == true
                    ? ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem silindi")
                    : ServiceResult<bool>.Fail(apiResponse?.Message ?? "İşlem silinemedi");
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
                var response = await _httpClient.GetAsync("modulcontrollerislem/dropdown");
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

        public async Task<ServiceResult<List<DropdownItemDto>>> GetDropdownByControllerIdAsync(int controllerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modulcontrollerislem/dropdown/by-controller/{controllerId}");
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
                _logger.LogError(ex, "GetDropdownByControllerIdAsync Exception");
                return ServiceResult<List<DropdownItemDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DropdownDto>>> GetActionTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modulcontrollerislem/action-types");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DropdownDto>>.Fail("ActionType listesi alınamadı");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DropdownDto>>>();
                return apiResponse?.Success == true && apiResponse.Data != null
                    ? ServiceResult<List<DropdownDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "Başarılı")
                    : ServiceResult<List<DropdownDto>>.Fail(apiResponse?.Message ?? "ActionType listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActionTypesAsync Exception");
                return ServiceResult<List<DropdownDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

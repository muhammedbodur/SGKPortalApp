using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class ModulApiService : IModulApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ModulApiService> _logger;

        public ModulApiService(HttpClient httpClient, ILogger<ModulApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<ModulResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modul");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<ModulResponseDto>>.Fail("Modül listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ModulResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<ModulResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<ModulResponseDto>>.Fail(
                    apiResponse?.Message ?? "Modül listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<ModulResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"modul/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<ModulResponseDto>.Fail("Modül bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<ModulResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<ModulResponseDto>.Fail(
                    apiResponse?.Message ?? "Modül bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<ModulResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulResponseDto>> CreateAsync(ModulCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("CreateAsync çağrıldı: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("modul", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Hatası: {Error}", responseContent);
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<ModulResponseDto>>(responseContent, jsonOptions);
                        var errorMessage = errorResponse?.Message ?? "Modül eklenemedi.";
                        return ServiceResult<ModulResponseDto>.Fail(errorMessage);
                    }
                    catch
                    {
                        return ServiceResult<ModulResponseDto>.Fail("Modül eklenemedi.");
                    }
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<ModulResponseDto>>(responseContent, jsonOptions);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Modül başarıyla oluşturuldu!";

                    _logger.LogInformation("Modül oluşturuldu: {Message}", successMessage);

                    return ServiceResult<ModulResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                var failMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Modül oluşturulamadı";

                return ServiceResult<ModulResponseDto>.Fail(failMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<ModulResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ModulResponseDto>> UpdateAsync(int id, ModulUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"modul/{id}", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ModulResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Modül güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<ModulResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Modül başarıyla güncellendi!";

                    return ServiceResult<ModulResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<ModulResponseDto>.Fail(
                    apiResponse?.Message ?? "Modül güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<ModulResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"modul/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Modül silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Modül başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Modül silinemedi"
                );
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
                var response = await _httpClient.GetAsync("modul/dropdown");

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

using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class UnvanApiService : IUnvanApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UnvanApiService> _logger;

        public UnvanApiService(HttpClient httpClient, ILogger<UnvanApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<UnvanResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("unvan");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<UnvanResponseDto>>.Fail("Unvan listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UnvanResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<UnvanResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<UnvanResponseDto>>.Fail(
                    apiResponse?.Message ?? "Unvan listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<UnvanResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UnvanResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"unvan/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<UnvanResponseDto>.Fail("Unvan bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<UnvanResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<UnvanResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<UnvanResponseDto>.Fail(
                    apiResponse?.Message ?? "Unvan bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<UnvanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UnvanResponseDto>> CreateAsync(UnvanCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔵 CreateAsync çağrıldı: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("unvan", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<UnvanResponseDto>>(responseContent, jsonOptions);
                        var errorMessage = errorResponse?.Message ?? "Unvan eklenemedi.";
                        return ServiceResult<UnvanResponseDto>.Fail(errorMessage);
                    }
                    catch
                    {
                        return ServiceResult<UnvanResponseDto>.Fail("Unvan eklenemedi.");
                    }
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<UnvanResponseDto>>(responseContent, jsonOptions);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Unvan başarıyla oluşturuldu!";

                    _logger.LogInformation("✅ Unvan oluşturuldu: {Message}", successMessage);

                    return ServiceResult<UnvanResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                var failMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Unvan oluşturulamadı";

                return ServiceResult<UnvanResponseDto>.Fail(failMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateAsync Exception");
                return ServiceResult<UnvanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<UnvanResponseDto>> UpdateAsync(int id, UnvanUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"unvan/{id}", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<UnvanResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Unvan güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<UnvanResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Unvan başarıyla güncellendi!";

                    return ServiceResult<UnvanResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<UnvanResponseDto>.Fail(
                    apiResponse?.Message ?? "Unvan güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<UnvanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"unvan/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Unvan silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Unvan başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Unvan silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<UnvanResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("unvan/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<UnvanResponseDto>>.Fail("Aktif unvanlar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UnvanResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<UnvanResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<UnvanResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif unvanlar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<UnvanResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetPersonelCountAsync(int unvanId)
        {
            try
            {
                _logger.LogInformation("🔵 GetPersonelCountAsync çağrıldı: Unvan ID = {UnvanId}", unvanId);

                var response = await _httpClient.GetAsync($"unvan/{unvanId}/personel-count");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("❌ GetPersonelCountAsync failed: {Error}", errorContent);
                    return ServiceResult<int>.Fail("Personel sayısı alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Personel sayısı başarıyla alındı!";

                    _logger.LogInformation("✅ Personel sayısı alındı: {Message}, Sonuç = {Count}", successMessage, apiResponse.Data);

                    return ServiceResult<int>.Ok(apiResponse.Data, successMessage);
                }

                return ServiceResult<int>.Fail(
                    apiResponse?.Message ?? "Personel sayısı alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetPersonelCountAsync Exception");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
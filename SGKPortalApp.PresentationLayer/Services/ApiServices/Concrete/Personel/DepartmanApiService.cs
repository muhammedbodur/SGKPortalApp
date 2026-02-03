using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class DepartmanApiService : IDepartmanApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmanApiService> _logger;

        public DepartmanApiService(HttpClient httpClient, ILogger<DepartmanApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("departman");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DepartmanResponseDto>>.Fail("Departman listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DepartmanResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DepartmanResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DepartmanResponseDto>>.Fail(
                    apiResponse?.Message ?? "Departman listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<DepartmanResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DepartmanResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"departman/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<DepartmanResponseDto>.Fail("Departman bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<DepartmanResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<DepartmanResponseDto>.Fail(
                    apiResponse?.Message ?? "Departman bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<DepartmanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔵 CreateAsync çağrıldı: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("departman", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<DepartmanResponseDto>>(responseContent, jsonOptions);
                        var errorMessage = errorResponse?.Message ?? "Departman eklenemedi.";
                        return ServiceResult<DepartmanResponseDto>.Fail(errorMessage);
                    }
                    catch
                    {
                        return ServiceResult<DepartmanResponseDto>.Fail("Departman eklenemedi.");
                    }
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponseDto<DepartmanResponseDto>>(responseContent, jsonOptions);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    // ✅ Başarılı - Mesajı mutlaka doldur
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Departman başarıyla oluşturuldu!";

                    _logger.LogInformation("✅ Departman oluşturuldu: {Message}", successMessage);

                    return ServiceResult<DepartmanResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                // ❌ API'den success=false geldi
                var failMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Departman oluşturulamadı";

                return ServiceResult<DepartmanResponseDto>.Fail(failMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateAsync Exception");
                return ServiceResult<DepartmanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"departman/{id}", request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Departman güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<DepartmanResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Departman başarıyla güncellendi!";

                    return ServiceResult<DepartmanResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<DepartmanResponseDto>.Fail(
                    apiResponse?.Message ?? "Departman güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<DepartmanResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"departman/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Departman silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Departman başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Departman silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("departman/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DepartmanResponseDto>>.Fail("Aktif departmanlar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DepartmanResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DepartmanResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DepartmanResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif departmanlar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<DepartmanResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetPersonelCountAsync(int departmanId)
        {
            try
            {
                _logger.LogInformation("🔵 GetPersonelCountAsync çağrıldı: Departman ID = {DepartmanId}", departmanId);

                var response = await _httpClient.GetAsync($"departman/{departmanId}/personel-count");

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
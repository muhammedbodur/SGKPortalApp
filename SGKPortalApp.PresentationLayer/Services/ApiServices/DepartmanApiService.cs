using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices
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
                var response = await _httpClient.GetAsync("api/departman");

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
                var response = await _httpClient.GetAsync($"api/departman/{id}");

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

                var response = await _httpClient.PostAsJsonAsync("api/departman", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    return ServiceResult<DepartmanResponseDto>.Fail(
                        $"Departman eklenemedi. Detay: {responseContent}"
                    );
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();

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
                var errorMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Departman oluşturulamadı";

                return ServiceResult<DepartmanResponseDto>.Fail(errorMessage);
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
                var response = await _httpClient.PutAsJsonAsync($"api/departman/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<DepartmanResponseDto>.Fail("Departman güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanResponseDto>>();

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
                var response = await _httpClient.DeleteAsync($"api/departman/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Departman silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

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
                var response = await _httpClient.GetAsync("api/departman/active");

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
    }
}
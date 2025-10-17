// ════════════════════════════════════════════════════════════════
// 📁 PresentationLayer/Services/ApiServices/Concrete/Common/HizmetBinasiApiService.cs
// ════════════════════════════════════════════════════════════════
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class HizmetBinasiApiService : IHizmetBinasiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HizmetBinasiApiService> _logger;

        public HizmetBinasiApiService(HttpClient httpClient, ILogger<HizmetBinasiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("hizmetbinasi");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Fail("Hizmet binaları listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<HizmetBinasiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Hizmet binaları listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<HizmetBinasiResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"hizmetbinasi/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<HizmetBinasiResponseDto>.Fail("Hizmet binası bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<HizmetBinasiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<HizmetBinasiResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<HizmetBinasiResponseDto>.Fail(
                    apiResponse?.Message ?? "Hizmet binası bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<HizmetBinasiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<HizmetBinasiDetailResponseDto>> GetDetailByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"hizmetbinasi/{id}/detail");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDetailByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<HizmetBinasiDetailResponseDto>.Fail("Hizmet binası detayı bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<HizmetBinasiDetailResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<HizmetBinasiDetailResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<HizmetBinasiDetailResponseDto>.Fail(
                    apiResponse?.Message ?? "Hizmet binası detayı bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDetailByIdAsync Exception");
                return ServiceResult<HizmetBinasiDetailResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<HizmetBinasiResponseDto>> CreateAsync(HizmetBinasiCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔵 CreateAsync çağrıldı: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("hizmetbinasi", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    return ServiceResult<HizmetBinasiResponseDto>.Fail(
                        $"Hizmet binası eklenemedi. Detay: {responseContent}"
                    );
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<HizmetBinasiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Hizmet binası başarıyla oluşturuldu!";

                    _logger.LogInformation("✅ Hizmet binası oluşturuldu: {Message}", successMessage);

                    return ServiceResult<HizmetBinasiResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                var errorMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Hizmet binası oluşturulamadı";

                return ServiceResult<HizmetBinasiResponseDto>.Fail(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateAsync Exception");
                return ServiceResult<HizmetBinasiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<HizmetBinasiResponseDto>> UpdateAsync(int id, HizmetBinasiUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"hizmetbinasi/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<HizmetBinasiResponseDto>.Fail("Hizmet binası güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<HizmetBinasiResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Hizmet binası başarıyla güncellendi!";

                    return ServiceResult<HizmetBinasiResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<HizmetBinasiResponseDto>.Fail(
                    apiResponse?.Message ?? "Hizmet binası güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<HizmetBinasiResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"hizmetbinasi/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Hizmet binası silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Hizmet binası başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Hizmet binası silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("hizmetbinasi/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Fail("Aktif hizmet binaları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<HizmetBinasiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif hizmet binaları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<HizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"hizmetbinasi/by-departman/{departmanId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByDepartmanAsync failed: {Error}", errorContent);
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Fail("Departmana ait hizmet binaları alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<HizmetBinasiResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<HizmetBinasiResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail(
                    apiResponse?.Message ?? "Departmana ait hizmet binaları alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByDepartmanAsync Exception");
                return ServiceResult<List<HizmetBinasiResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetPersonelCountAsync(int hizmetBinasiId)
        {
            try
            {
                _logger.LogInformation("🔵 GetPersonelCountAsync çağrıldı: Hizmet Binası ID = {Id}", hizmetBinasiId);

                var response = await _httpClient.GetAsync($"hizmetbinasi/{hizmetBinasiId}/personel-count");

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

        public async Task<ServiceResult<bool>> ToggleStatusAsync(int id)
        {
            try
            {
                _logger.LogInformation("🔵 ToggleStatusAsync çağrıldı: ID = {Id}", id);

                var response = await _httpClient.PatchAsync($"hizmetbinasi/{id}/toggle-status", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("❌ ToggleStatusAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Durum değiştirilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Durum başarıyla değiştirildi!";

                    _logger.LogInformation("✅ Durum değiştirildi: {Message}", successMessage);

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Durum değiştirilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ToggleStatusAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}
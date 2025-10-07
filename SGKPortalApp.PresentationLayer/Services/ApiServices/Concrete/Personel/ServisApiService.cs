using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class ServisApiService : IServisApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServisApiService> _logger;

        public ServisApiService(HttpClient httpClient, ILogger<ServisApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<ServisResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/servis");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<ServisResponseDto>>.Fail("Servis listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ServisResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<ServisResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<ServisResponseDto>>.Fail(
                    apiResponse?.Message ?? "Servis listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<ServisResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ServisResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/servis/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<ServisResponseDto>.Fail("Servis bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ServisResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<ServisResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<ServisResponseDto>.Fail(
                    apiResponse?.Message ?? "Servis bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<ServisResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ServisResponseDto>> CreateAsync(ServisCreateRequestDto request)
        {
            try
            {
                _logger.LogInformation("🔵 CreateAsync çağrıldı: {@Request}", request);

                var response = await _httpClient.PostAsJsonAsync("api/servis", request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📡 Status: {Status}, Content: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ API Hatası: {Error}", responseContent);
                    return ServiceResult<ServisResponseDto>.Fail(
                        $"Servis eklenemedi. Detay: {responseContent}"
                    );
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ServisResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Servis başarıyla oluşturuldu!";

                    _logger.LogInformation("✅ Servis oluşturuldu: {Message}", successMessage);

                    return ServiceResult<ServisResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                var errorMessage = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                    ? apiResponse.Message
                    : "Servis oluşturulamadı";

                return ServiceResult<ServisResponseDto>.Fail(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateAsync Exception");
                return ServiceResult<ServisResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ServisResponseDto>> UpdateAsync(int id, ServisUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/servis/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<ServisResponseDto>.Fail("Servis güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<ServisResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Servis başarıyla güncellendi!";

                    return ServiceResult<ServisResponseDto>.Ok(
                        apiResponse.Data,
                        successMessage
                    );
                }

                return ServiceResult<ServisResponseDto>.Fail(
                    apiResponse?.Message ?? "Servis güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<ServisResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/servis/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Servis silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    var successMessage = !string.IsNullOrWhiteSpace(apiResponse.Message)
                        ? apiResponse.Message
                        : "Servis başarıyla silindi!";

                    return ServiceResult<bool>.Ok(true, successMessage);
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Servis silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ServisResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/servis/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<ServisResponseDto>>.Fail("Aktif servisler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ServisResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<ServisResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<ServisResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif servisler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<ServisResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetPersonelCountAsync(int servisId)
        {
            try
            {
                _logger.LogInformation("🔵 GetPersonelCountAsync çağrıldı: Servis ID = {ServisId}", servisId);

                var response = await _httpClient.GetAsync($"api/servis/{servisId}/personel-count");

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
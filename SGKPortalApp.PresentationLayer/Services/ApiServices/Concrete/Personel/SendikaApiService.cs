using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Personel
{
    public class SendikaApiService : ISendikaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SendikaApiService> _logger;

        public SendikaApiService(HttpClient httpClient, ILogger<SendikaApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<SendikaResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/sendika");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<SendikaResponseDto>>.Fail("Sendika listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<SendikaResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<SendikaResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<SendikaResponseDto>>.Fail(
                    apiResponse?.Message ?? "Sendika listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<SendikaResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SendikaResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/sendika/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<SendikaResponseDto>.Fail("Sendika bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SendikaResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SendikaResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<SendikaResponseDto>.Fail(
                    apiResponse?.Message ?? "Sendika bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<SendikaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SendikaResponseDto>> CreateAsync(SendikaCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/sendika", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<SendikaResponseDto>.Fail("Sendika oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SendikaResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SendikaResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Sendika başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<SendikaResponseDto>.Fail(
                    apiResponse?.Message ?? "Sendika oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<SendikaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SendikaResponseDto>> UpdateAsync(int id, SendikaUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/sendika/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<SendikaResponseDto>.Fail("Sendika güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SendikaResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SendikaResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Sendika başarıyla güncellendi"
                    );
                }

                return ServiceResult<SendikaResponseDto>.Fail(
                    apiResponse?.Message ?? "Sendika güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<SendikaResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/sendika/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Sendika silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Sendika başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Sendika silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KanalApiService : IKanalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KanalApiService> _logger;

        public KanalApiService(HttpClient httpClient, ILogger<KanalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KanalResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("kanal");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalResponseDto>>.Fail("Kanal listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalResponseDto>>.Fail(
                    apiResponse?.Message ?? "Kanal listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<KanalResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanal/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalResponseDto>.Fail("Kanal bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<KanalResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<KanalResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalResponseDto>> CreateAsync(KanalCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("kanal", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal oluşturulamadı.";
                    _logger.LogError("CreateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal başarıyla oluşturuldu"
                    );
                }

                return ServiceResult<KanalResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<KanalResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalResponseDto>> UpdateAsync(int id, KanalUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"kanal/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Kanal başarıyla güncellendi"
                    );
                }

                return ServiceResult<KanalResponseDto>.Fail(
                    apiResponse?.Message ?? "Kanal güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<KanalResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kanal/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Kanal silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Kanal başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Kanal silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("kanal/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalResponseDto>>.Fail("Aktif kanallar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif kanallar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<KanalResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

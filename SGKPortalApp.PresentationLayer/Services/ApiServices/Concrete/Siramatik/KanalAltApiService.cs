using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Siramatik
{
    public class KanalAltApiService : IKanalAltApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KanalAltApiService> _logger;

        public KanalAltApiService(HttpClient httpClient, ILogger<KanalAltApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<KanalAltResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("KanalAlt");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltResponseDto>>.Fail("Alt kanal listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltResponseDto>>.Fail(
                    apiResponse?.Message ?? "Alt kanal listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<KanalAltResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"KanalAlt/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<KanalAltResponseDto>.Fail("Alt kanal bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<KanalAltResponseDto>.Fail(
                    apiResponse?.Message ?? "Alt kanal bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<KanalAltResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltResponseDto>> CreateAsync(KanalAltKanalCreateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("kanalalt", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Alt kanal eklenemedi.";
                    _logger.LogError("CreateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalAltResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Alt kanal başarıyla eklendi"
                    );
                }

                return ServiceResult<KanalAltResponseDto>.Fail(
                    apiResponse?.Message ?? "Alt kanal eklenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<KanalAltResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<KanalAltResponseDto>> UpdateAsync(int id, KanalAltKanalUpdateRequestDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"kanalalt/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<KanalAltResponseDto>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Alt kanal güncellenemedi.";
                    _logger.LogError("UpdateAsync failed: {Error}", errorMessage);
                    return ServiceResult<KanalAltResponseDto>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<KanalAltResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "Alt kanal başarıyla güncellendi"
                    );
                }

                return ServiceResult<KanalAltResponseDto>.Fail(
                    apiResponse?.Message ?? "Alt kanal güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<KanalAltResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"kanalalt/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = apiResponse?.Message ?? "Alt kanal silinemedi.";
                    _logger.LogError("DeleteAsync failed: {Error}", errorMessage);
                    return ServiceResult<bool>.Fail(errorMessage);
                }

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "Alt kanal başarıyla silindi"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Alt kanal silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalAltResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("kanalalt/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltResponseDto>>.Fail("Aktif alt kanallar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif alt kanallar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<KanalAltResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<KanalAltResponseDto>>> GetByKanalIdAsync(int kanalId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"kanalalt/kanal/{kanalId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByKanalIdAsync failed: {Error}", errorContent);
                    return ServiceResult<List<KanalAltResponseDto>>.Fail("Alt kanallar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<KanalAltResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<KanalAltResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<KanalAltResponseDto>>.Fail(
                    apiResponse?.Message ?? "Alt kanallar alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByKanalIdAsync Exception");
                return ServiceResult<List<KanalAltResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

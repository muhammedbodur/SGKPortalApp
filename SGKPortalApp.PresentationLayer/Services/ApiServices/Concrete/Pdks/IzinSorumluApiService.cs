using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class IzinSorumluApiService : IIzinSorumluApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IzinSorumluApiService> _logger;

        public IzinSorumluApiService(HttpClient httpClient, ILogger<IzinSorumluApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<IzinSorumluResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("izin-sorumlu");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinSorumluResponseDto>>.Fail("İzin sorumlu listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinSorumluResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinSorumluResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinSorumluResponseDto>>.Fail(
                    apiResponse?.Message ?? "İzin sorumlu listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<IzinSorumluResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IzinSorumluResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("izin-sorumlu/aktif");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinSorumluResponseDto>>.Fail("Aktif izin sorumlu listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinSorumluResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinSorumluResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinSorumluResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif izin sorumlu listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<IzinSorumluResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinSorumluResponseDto>> CreateAsync(IzinSorumluCreateDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-sorumlu", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinSorumluResponseDto>.Fail("İzin sorumlusu oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinSorumluResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinSorumluResponseDto>.Fail(
                    apiResponse?.Message ?? "İzin sorumlusu oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<IzinSorumluResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinSorumluResponseDto>> UpdateAsync(int id, IzinSorumluUpdateDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"izin-sorumlu/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinSorumluResponseDto>.Fail("İzin sorumlusu güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinSorumluResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinSorumluResponseDto>.Fail(
                    apiResponse?.Message ?? "İzin sorumlusu güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<IzinSorumluResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"izin-sorumlu/{id}/pasif", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeactivateAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("İzin sorumlusu pasif yapılamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "İzin sorumlusu pasif yapılamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ActivateAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"izin-sorumlu/{id}/aktif", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ActivateAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("İzin sorumlusu aktif yapılamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "İzin sorumlusu aktif yapılamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"izin-sorumlu/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("İzin sorumlusu silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        true,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "İzin sorumlusu silinemedi"
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

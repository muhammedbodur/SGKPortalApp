using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class IzinMazeretTuruTanimApiService : IIzinMazeretTuruTanimApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IzinMazeretTuruTanimApiService> _logger;

        public IzinMazeretTuruTanimApiService(
            HttpClient httpClient,
            ILogger<IzinMazeretTuruTanimApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("izin-mazeret-turu");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail("İzin türleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinMazeretTuruResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail(
                    apiResponse?.Message ?? "İzin türleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync Exception");
                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<IzinMazeretTuruResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("izin-mazeret-turu/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail("Aktif izin türleri alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<IzinMazeretTuruResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<IzinMazeretTuruResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail(
                    apiResponse?.Message ?? "Aktif izin türleri alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync Exception");
                return ServiceResult<List<IzinMazeretTuruResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinMazeretTuruResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"izin-mazeret-turu/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinMazeretTuruResponseDto>.Fail("İzin türü alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinMazeretTuruResponseDto>.Fail(
                    apiResponse?.Message ?? "İzin türü alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync Exception");
                return ServiceResult<IzinMazeretTuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinMazeretTuruResponseDto>> CreateAsync(IzinMazeretTuruResponseDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("izin-mazeret-turu", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinMazeretTuruResponseDto>.Fail("İzin türü oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinMazeretTuruResponseDto>.Fail(
                    apiResponse?.Message ?? "İzin türü oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync Exception");
                return ServiceResult<IzinMazeretTuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IzinMazeretTuruResponseDto>> UpdateAsync(int id, IzinMazeretTuruResponseDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"izin-mazeret-turu/{id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<IzinMazeretTuruResponseDto>.Fail("İzin türü güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinMazeretTuruResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<IzinMazeretTuruResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<IzinMazeretTuruResponseDto>.Fail(
                    apiResponse?.Message ?? "İzin türü güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync Exception");
                return ServiceResult<IzinMazeretTuruResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ToggleActiveAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"izin-mazeret-turu/{id}/toggle", null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ToggleActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Aktiflik durumu değiştirilemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "Aktiflik durumu değiştirilemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleActiveAsync Exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"izin-mazeret-turu/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("İzin türü silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<bool>.Fail(
                    apiResponse?.Message ?? "İzin türü silinemedi"
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

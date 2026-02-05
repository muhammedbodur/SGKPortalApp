using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class GununMenusuApiService : IGununMenusuApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GununMenusuApiService> _logger;

        public GununMenusuApiService(HttpClient httpClient, ILogger<GununMenusuApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<GununMenusuResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("gununmenusu");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<GununMenusuResponseDto>>.Fail("Günün menüsü listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<GununMenusuResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<GununMenusuResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<GununMenusuResponseDto>>.Fail(
                    apiResponse?.Message ?? "Günün menüsü listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<GununMenusuResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<GununMenusuResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"gununmenusu/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<GununMenusuResponseDto>.Fail("Menü bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<GununMenusuResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<GununMenusuResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<GununMenusuResponseDto>.Fail(
                    apiResponse?.Message ?? "Menü bulunamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<GununMenusuResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<GununMenusuResponseDto?>> GetByDateAsync(DateTime date)
        {
            try
            {
                var response = await _httpClient.GetAsync($"gununmenusu/by-date?date={Uri.EscapeDataString(date.ToString("O"))}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByDateAsync failed: {Error}", errorContent);
                    return ServiceResult<GununMenusuResponseDto?>.Fail("Menü alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<GununMenusuResponseDto?>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<GununMenusuResponseDto?>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<GununMenusuResponseDto?>.Fail(
                    apiResponse?.Message ?? "Menü alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByDateAsync exception");
                return ServiceResult<GununMenusuResponseDto?>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<GununMenusuResponseDto>> CreateAsync(GununMenusuCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("gununmenusu", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<GununMenusuResponseDto>.Fail("Menü oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<GununMenusuResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<GununMenusuResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<GununMenusuResponseDto>.Fail(
                    apiResponse?.Message ?? "Menü oluşturulamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync exception");
                return ServiceResult<GununMenusuResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<GununMenusuResponseDto>> UpdateAsync(int id, GununMenusuUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"gununmenusu/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<GununMenusuResponseDto>.Fail("Menü güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<GununMenusuResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<GununMenusuResponseDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<GununMenusuResponseDto>.Fail(
                    apiResponse?.Message ?? "Menü güncellenemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync exception");
                return ServiceResult<GununMenusuResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"gununmenusu/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Menü silinemedi.");
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
                    apiResponse?.Message ?? "Menü silinemedi"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResponseDto<GununMenusuResponseDto>>> GetPagedAsync(GununMenusuFilterRequestDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("gununmenusu/paged", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPagedAsync failed: {Error}", errorContent);
                    return ServiceResult<PagedResponseDto<GununMenusuResponseDto>>.Fail("Menüler alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResponseDto<GununMenusuResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PagedResponseDto<GununMenusuResponseDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<PagedResponseDto<GununMenusuResponseDto>>.Fail(
                    apiResponse?.Message ?? "Menüler alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPagedAsync exception");
                return ServiceResult<PagedResponseDto<GununMenusuResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

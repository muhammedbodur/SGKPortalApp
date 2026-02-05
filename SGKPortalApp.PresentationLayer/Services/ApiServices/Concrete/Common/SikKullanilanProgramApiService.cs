using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class SikKullanilanProgramApiService : ISikKullanilanProgramApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SikKullanilanProgramApiService> _logger;

        public SikKullanilanProgramApiService(HttpClient httpClient, ILogger<SikKullanilanProgramApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<SikKullanilanProgramResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sikkullanilanprogram");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllAsync failed: {Error}", errorContent);
                    return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail("Program listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<SikKullanilanProgramResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<SikKullanilanProgramResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail(apiResponse?.Message ?? "Program listesi alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync exception");
                return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SikKullanilanProgramResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("sikkullanilanprogram/active");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetActiveAsync failed: {Error}", errorContent);
                    return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail("Aktif programlar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<SikKullanilanProgramResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<SikKullanilanProgramResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail(apiResponse?.Message ?? "Aktif programlar alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync exception");
                return ServiceResult<List<SikKullanilanProgramResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SikKullanilanProgramResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"sikkullanilanprogram/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetByIdAsync failed: {Error}", errorContent);
                    return ServiceResult<SikKullanilanProgramResponseDto>.Fail("Program bulunamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SikKullanilanProgramResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SikKullanilanProgramResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<SikKullanilanProgramResponseDto>.Fail(apiResponse?.Message ?? "Program bulunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync exception");
                return ServiceResult<SikKullanilanProgramResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SikKullanilanProgramResponseDto>> CreateAsync(SikKullanilanProgramCreateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("sikkullanilanprogram", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("CreateAsync failed: {Error}", errorContent);
                    return ServiceResult<SikKullanilanProgramResponseDto>.Fail("Program oluşturulamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SikKullanilanProgramResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SikKullanilanProgramResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<SikKullanilanProgramResponseDto>.Fail(apiResponse?.Message ?? "Program oluşturulamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync exception");
                return ServiceResult<SikKullanilanProgramResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<SikKullanilanProgramResponseDto>> UpdateAsync(int id, SikKullanilanProgramUpdateRequestDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"sikkullanilanprogram/{id}", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("UpdateAsync failed: {Error}", errorContent);
                    return ServiceResult<SikKullanilanProgramResponseDto>.Fail("Program güncellenemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<SikKullanilanProgramResponseDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<SikKullanilanProgramResponseDto>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<SikKullanilanProgramResponseDto>.Fail(apiResponse?.Message ?? "Program güncellenemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync exception");
                return ServiceResult<SikKullanilanProgramResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"sikkullanilanprogram/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("DeleteAsync failed: {Error}", errorContent);
                    return ServiceResult<bool>.Fail("Program silinemedi.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                if (apiResponse?.Success == true)
                {
                    return ServiceResult<bool>.Ok(true, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<bool>.Fail(apiResponse?.Message ?? "Program silinemedi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync exception");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>> GetPagedAsync(SikKullanilanProgramFilterRequestDto filter)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("sikkullanilanprogram/paged", filter);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetPagedAsync failed: {Error}", errorContent);
                    return ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>.Fail("Programlar alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResponseDto<SikKullanilanProgramResponseDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>.Ok(apiResponse.Data, apiResponse.Message ?? "İşlem başarılı");
                }

                return ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>.Fail(apiResponse?.Message ?? "Programlar alınamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPagedAsync exception");
                return ServiceResult<PagedResponseDto<SikKullanilanProgramResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

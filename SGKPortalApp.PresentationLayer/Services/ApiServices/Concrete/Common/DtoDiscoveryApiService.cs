// ════════════════════════════════════════════════════════════════
// 📁 PresentationLayer/Services/ApiServices/Concrete/Common/DtoDiscoveryApiService.cs
// ════════════════════════════════════════════════════════════════
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class DtoDiscoveryApiService : IDtoDiscoveryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DtoDiscoveryApiService> _logger;

        public DtoDiscoveryApiService(HttpClient httpClient, ILogger<DtoDiscoveryApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DtoTypeInfo>>> GetAllDtoTypesAsync()
        {
            try
            {
                _logger.LogInformation("GetAllDtoTypesAsync: BaseAddress={BaseAddress}", _httpClient.BaseAddress);
                
                var response = await _httpClient.GetAsync("dtodiscovery/dto-types");
                
                _logger.LogInformation("GetAllDtoTypesAsync: StatusCode={StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllDtoTypesAsync failed: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    return ServiceResult<List<DtoTypeInfo>>.Fail("DTO listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DtoTypeInfo>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DtoTypeInfo>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DtoTypeInfo>>.Fail(
                    apiResponse?.Message ?? "DTO listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllDtoTypesAsync Exception");
                return ServiceResult<List<DtoTypeInfo>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DtoPropertyInfo>>> GetDtoPropertiesAsync(string dtoTypeName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"dtodiscovery/dto-properties/{dtoTypeName}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDtoPropertiesAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DtoPropertyInfo>>.Fail("DTO property listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DtoPropertyInfo>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DtoPropertyInfo>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DtoPropertyInfo>>.Fail(
                    apiResponse?.Message ?? "DTO property listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDtoPropertiesAsync Exception: {DtoTypeName}", dtoTypeName);
                return ServiceResult<List<DtoPropertyInfo>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<FieldAnalysisResult>> GetFieldAnalysisAsync(string pageKey, string dtoTypeName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"dtodiscovery/field-analysis?pageKey={Uri.EscapeDataString(pageKey)}&dtoTypeName={Uri.EscapeDataString(dtoTypeName)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetFieldAnalysisAsync failed: {Error}", errorContent);
                    return ServiceResult<FieldAnalysisResult>.Fail("Field analizi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<FieldAnalysisResult>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<FieldAnalysisResult>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<FieldAnalysisResult>.Fail(
                    apiResponse?.Message ?? "Field analizi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFieldAnalysisAsync Exception: {PageKey}, {DtoTypeName}", pageKey, dtoTypeName);
                return ServiceResult<FieldAnalysisResult>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

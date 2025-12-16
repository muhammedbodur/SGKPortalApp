using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

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
                var response = await _httpClient.GetAsync("common/dto-discovery/dto-types");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetAllDtoTypesAsync failed: {Error}", errorContent);
                    return new ServiceResult<List<DtoTypeInfo>>
                    {
                        Success = false,
                        Message = new[] { $"API hatası: {response.StatusCode}" }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DtoTypeInfo>>>();
                return result ?? new ServiceResult<List<DtoTypeInfo>>
                {
                    Success = false,
                    Message = new[] { "Response deserialize edilemedi" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllDtoTypesAsync exception");
                return new ServiceResult<List<DtoTypeInfo>>
                {
                    Success = false,
                    Message = new[] { $"Hata: {ex.Message}" }
                };
            }
        }

        public async Task<ServiceResult<List<DtoPropertyInfo>>> GetDtoPropertiesAsync(string dtoTypeName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"common/dto-discovery/dto-properties/{dtoTypeName}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDtoPropertiesAsync failed: {Error}", errorContent);
                    return new ServiceResult<List<DtoPropertyInfo>>
                    {
                        Success = false,
                        Message = new[] { $"API hatası: {response.StatusCode}" }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DtoPropertyInfo>>>();
                return result ?? new ServiceResult<List<DtoPropertyInfo>>
                {
                    Success = false,
                    Message = new[] { "Response deserialize edilemedi" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDtoPropertiesAsync exception: {DtoTypeName}", dtoTypeName);
                return new ServiceResult<List<DtoPropertyInfo>>
                {
                    Success = false,
                    Message = new[] { $"Hata: {ex.Message}" }
                };
            }
        }

        public async Task<ServiceResult<FieldAnalysisResult>> GetFieldAnalysisAsync(string pageKey, string dtoTypeName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"common/dto-discovery/field-analysis?pageKey={Uri.EscapeDataString(pageKey)}&dtoTypeName={Uri.EscapeDataString(dtoTypeName)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetFieldAnalysisAsync failed: {Error}", errorContent);
                    return new ServiceResult<FieldAnalysisResult>
                    {
                        Success = false,
                        Message = new[] { $"API hatası: {response.StatusCode}" }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ServiceResult<FieldAnalysisResult>>();
                return result ?? new ServiceResult<FieldAnalysisResult>
                {
                    Success = false,
                    Message = new[] { "Response deserialize edilemedi" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFieldAnalysisAsync exception: {PageKey}, {DtoTypeName}", pageKey, dtoTypeName);
                return new ServiceResult<FieldAnalysisResult>
                {
                    Success = false,
                    Message = new[] { $"Hata: {ex.Message}" }
                };
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Common
{
    public class ModulControllerIslemApiService : IModulControllerIslemApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ModulControllerIslemApiService> _logger;

        public ModulControllerIslemApiService(HttpClient httpClient, ILogger<ModulControllerIslemApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("modulcontrollerislem/dropdown");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetDropdownAsync failed: {Error}", errorContent);
                    return ServiceResult<List<DropdownItemDto>>.Fail("Dropdown listesi alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DropdownItemDto>>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<List<DropdownItemDto>>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<List<DropdownItemDto>>.Fail(
                    apiResponse?.Message ?? "Dropdown listesi alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDropdownAsync Exception");
                return ServiceResult<List<DropdownItemDto>>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

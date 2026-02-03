using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.Pdks
{
    public class DepartmanMesaiApiService : IDepartmanMesaiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmanMesaiApiService> _logger;

        public DepartmanMesaiApiService(HttpClient httpClient, ILogger<DepartmanMesaiApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResult<DepartmanMesaiReportDto>> GetRaporAsync(DepartmanMesaiFilterRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("departman-mesai/rapor", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("GetRaporAsync failed: {Error}", errorContent);
                    return ServiceResult<DepartmanMesaiReportDto>.Fail("Departman mesai raporu alınamadı.");
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanMesaiReportDto>>();

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ServiceResult<DepartmanMesaiReportDto>.Ok(
                        apiResponse.Data,
                        apiResponse.Message ?? "İşlem başarılı"
                    );
                }

                return ServiceResult<DepartmanMesaiReportDto>.Fail(
                    apiResponse?.Message ?? "Departman mesai raporu alınamadı"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRaporAsync Exception");
                return ServiceResult<DepartmanMesaiReportDto>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

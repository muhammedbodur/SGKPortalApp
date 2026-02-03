using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    /// <summary>
    /// Dashboard API Service Interface
    /// </summary>
    public interface IDashboardApiService
    {
        Task<ApiResponseDto<DashboardDataResponseDto>> GetDashboardDataAsync();
        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetSliderDuyurularAsync(int count = 5);
        Task<ApiResponseDto<List<DuyuruResponseDto>>> GetListeDuyurularAsync(int count = 10);
        Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetSikKullanilanProgramlarAsync(int count = 8);
        Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetOnemliLinklerAsync(int count = 10);
        Task<ApiResponseDto<GununMenusuResponseDto?>> GetGununMenusuAsync();
        Task<ApiResponseDto<List<BugunDoganResponseDto>>> GetBugunDoganlarAsync();
    }
}

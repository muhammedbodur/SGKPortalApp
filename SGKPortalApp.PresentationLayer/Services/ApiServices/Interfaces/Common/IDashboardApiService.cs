using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    /// <summary>
    /// Dashboard API Service Interface
    /// </summary>
    public interface IDashboardApiService
    {
        Task<ApiResponseDto<DashboardDataResponseDto>> GetDashboardDataAsync();
        Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetSliderHaberleriAsync(int count = 5);
        Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetListeHaberleriAsync(int count = 10);
        Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetSikKullanilanProgramlarAsync(int count = 8);
        Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetOnemliLinklerAsync(int count = 10);
        Task<ApiResponseDto<GununMenusuResponseDto?>> GetGununMenusuAsync();
        Task<ApiResponseDto<List<BugunDoganResponseDto>>> GetBugunDoganlarAsync();
    }
}

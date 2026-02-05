using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    /// <summary>
    /// Dashboard Service Interface
    /// Ana sayfa için gerekli tüm verileri sağlar
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Dashboard için tüm verileri tek seferde getirir
        /// </summary>
        Task<ApiResponseDto<DashboardDataResponseDto>> GetDashboardDataAsync();

        /// <summary>
        /// Slider haberlerini getirir
        /// </summary>
        Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetSliderHaberleriAsync(int count = 5);

        /// <summary>
        /// Liste haberlerini getirir
        /// </summary>
        Task<ApiResponseDto<List<HaberDashboardResponseDto>>> GetListeHaberleriAsync(int count = 10);

        /// <summary>
        /// Sık kullanılan programları getirir
        /// </summary>
        Task<ApiResponseDto<List<SikKullanilanProgramResponseDto>>> GetSikKullanilanProgramlarAsync(int count = 8);

        /// <summary>
        /// Önemli linkleri getirir
        /// </summary>
        Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetOnemliLinklerAsync(int count = 10);

        /// <summary>
        /// Bugünün menüsünü getirir
        /// </summary>
        Task<ApiResponseDto<GununMenusuResponseDto?>> GetGununMenusuAsync();

        /// <summary>
        /// Bugün doğanları getirir
        /// </summary>
        Task<ApiResponseDto<List<BugunDoganResponseDto>>> GetBugunDoganlarAsync();
    }
}

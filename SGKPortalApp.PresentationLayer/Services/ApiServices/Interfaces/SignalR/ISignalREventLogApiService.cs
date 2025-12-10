using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.SignalR
{
    /// <summary>
    /// SignalR Event Log API Service Interface
    /// </summary>
    public interface ISignalREventLogApiService
    {
        /// <summary>
        /// Filtrelenmiş ve sayfalanmış event loglarını getirir
        /// </summary>
        Task<PagedResultDto<SignalREventLogResponseDto>?> GetFilteredAsync(SignalREventLogFilterDto filter);

        /// <summary>
        /// Son N dakikadaki eventleri getirir
        /// </summary>
        Task<List<SignalREventLogResponseDto>> GetRecentAsync(int minutes = 5);

        /// <summary>
        /// Belirli bir sıranın eventlerini getirir
        /// </summary>
        Task<List<SignalREventLogResponseDto>> GetBySiraAsync(int siraId);

        /// <summary>
        /// İstatistikleri getirir
        /// </summary>
        Task<SignalREventLogStatsDto?> GetStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}

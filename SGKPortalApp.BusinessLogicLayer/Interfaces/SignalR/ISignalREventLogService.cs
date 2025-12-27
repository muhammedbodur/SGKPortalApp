using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR
{
    /// <summary>
    /// SignalR Event Log Service Interface
    /// </summary>
    public interface ISignalREventLogService
    {
        /// <summary>
        /// Event loglarını filtreli ve sayfalı olarak getirir
        /// </summary>
        Task<PagedResultDto<SignalREventLogResponseDto>> GetFilteredAsync(SignalREventLogFilterDto filter);

        /// <summary>
        /// Son N dakikadaki eventleri getirir (canlı izleme için)
        /// </summary>
        Task<List<SignalREventLogResponseDto>> GetRecentAsync(int minutes = 5);

        /// <summary>
        /// Belirli bir sıranın tüm eventlerini getirir
        /// </summary>
        Task<List<SignalREventLogResponseDto>> GetBySiraAsync(int siraId);

        /// <summary>
        /// İstatistikleri getirir
        /// </summary>
        Task<SignalREventLogStatsDto> GetStatsAsync(DateTime? startDate, DateTime? endDate);
    }
}

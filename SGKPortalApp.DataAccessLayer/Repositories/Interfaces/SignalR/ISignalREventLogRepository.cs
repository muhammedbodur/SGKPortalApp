using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SignalR
{
    /// <summary>
    /// SignalR Event Log Repository Interface
    /// Event loglarının kaydı ve sorgulanması için
    /// </summary>
    public interface ISignalREventLogRepository : IGenericRepository<SignalREventLog>
    {
        /// <summary>
        /// Yeni event log kaydı oluşturur (async, non-blocking)
        /// </summary>
        Task<SignalREventLog> LogEventAsync(SignalREventLog eventLog);

        /// <summary>
        /// Toplu event log kaydı oluşturur
        /// </summary>
        Task LogEventsAsync(IEnumerable<SignalREventLog> eventLogs);

        /// <summary>
        /// Event ID'ye göre log getirir
        /// </summary>
        Task<SignalREventLog?> GetByEventIdAsync(Guid eventId);

        /// <summary>
        /// Belirli bir sıra için tüm eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetBySiraIdAsync(int siraId);

        /// <summary>
        /// Belirli bir banko için eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetByBankoIdAsync(int bankoId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Belirli bir TV için eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetByTvIdAsync(int tvId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Belirli bir personel için eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetByPersonelTcAsync(string personelTc, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Başarısız eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetFailedEventsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Correlation ID'ye göre ilişkili eventleri getirir
        /// </summary>
        Task<List<SignalREventLog>> GetByCorrelationIdAsync(Guid correlationId);

        /// <summary>
        /// Tarih aralığında event istatistiklerini getirir
        /// </summary>
        Task<SignalREventStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Status'u doğrudan günceller (fire-and-forget için)
        /// </summary>
        Task UpdateStatusDirectAsync(Guid eventId, SignalRDeliveryStatus status, string? errorMessage, int? durationMs);

        /// <summary>
        /// ACK günceller (Faz 2 için)
        /// </summary>
        Task<bool> AcknowledgeEventAsync(Guid eventId);

        /// <summary>
        /// Eski logları temizler (retention policy)
        /// </summary>
        Task<int> CleanupOldLogsAsync(int retentionDays);
    }

    /// <summary>
    /// Event istatistikleri DTO
    /// </summary>
    public class SignalREventStatistics
    {
        public int TotalEvents { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int NoTargetCount { get; set; }
        public int AcknowledgedCount { get; set; }
        public double AverageDurationMs { get; set; }
        public Dictionary<SignalREventType, int> EventTypeCounts { get; set; } = new();
        public Dictionary<SignalRTargetType, int> TargetTypeCounts { get; set; } = new();
    }
}

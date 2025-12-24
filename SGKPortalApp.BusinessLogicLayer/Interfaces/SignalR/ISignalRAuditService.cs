using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR
{
    /// <summary>
    /// SignalR Audit Service Interface
    /// Event loglarının oluşturulması ve yönetimi için
    /// </summary>
    public interface ISignalRAuditService
    {
        /// <summary>
        /// Yeni bir correlation ID oluşturur (ilişkili eventleri gruplamak için)
        /// </summary>
        Guid CreateCorrelationId();

        /// <summary>
        /// Event log oluşturur ve kaydeder
        /// </summary>
        Task<SignalREventLog> LogEventAsync(SignalREventLogRequest request);

        /// <summary>
        /// Toplu event log oluşturur
        /// </summary>
        Task LogEventsAsync(IEnumerable<SignalREventLogRequest> requests);

        /// <summary>
        /// Event durumunu günceller (başarılı/başarısız)
        /// </summary>
        Task UpdateEventStatusAsync(Guid eventId, SignalRDeliveryStatus status, string? errorMessage = null, int? durationMs = null);
    }

    /// <summary>
    /// Event log oluşturma isteği
    /// </summary>
    public class SignalREventLogRequest
    {
        public SignalREventType EventType { get; set; }
        public string EventName { get; set; } = string.Empty;
        public SignalRTargetType TargetType { get; set; }
        public string? TargetId { get; set; }
        public int TargetCount { get; set; }
        public int? SiraId { get; set; }
        public int? SiraNo { get; set; }
        public int? BankoId { get; set; }
        public int? TvId { get; set; }
        public string? PersonelTc { get; set; }
        public object? Payload { get; set; }
        public int? HizmetBinasiId { get; set; }
        public Guid? CorrelationId { get; set; }
    }
}

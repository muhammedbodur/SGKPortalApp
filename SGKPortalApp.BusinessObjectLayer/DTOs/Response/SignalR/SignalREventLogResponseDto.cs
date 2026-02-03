using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// SignalR Event Log Response DTO
    /// </summary>
    public class SignalREventLogResponseDto
    {
        public long EventLogId { get; set; }
        public Guid EventId { get; set; }
        public SignalREventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public SignalRTargetType TargetType { get; set; }
        public string TargetTypeName { get; set; } = string.Empty;
        public string? TargetId { get; set; }
        public int TargetCount { get; set; }
        public int? SiraId { get; set; }
        public int? SiraNo { get; set; }
        public int? BankoId { get; set; }
        public string? BankoAdi { get; set; }
        public int? TvId { get; set; }
        public string? TvAdi { get; set; }
        public string? PersonelTc { get; set; }
        public string? PersonelAdSoyad { get; set; }
        public SignalRDeliveryStatus DeliveryStatus { get; set; }
        public string DeliveryStatusName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public int? DurationMs { get; set; }
        public string? PayloadSummary { get; set; }
        public int? HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }
        public Guid? CorrelationId { get; set; }
    }
}

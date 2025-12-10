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

    /// <summary>
    /// SignalR Event Log Filter DTO
    /// </summary>
    public class SignalREventLogFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SignalREventType? EventType { get; set; }
        public SignalRTargetType? TargetType { get; set; }
        public SignalRDeliveryStatus? DeliveryStatus { get; set; }
        public int? SiraId { get; set; }
        public int? SiraNo { get; set; }
        public int? BankoId { get; set; }
        public int? TvId { get; set; }
        public string? PersonelTc { get; set; }
        public int? HizmetBinasiId { get; set; }
        public Guid? CorrelationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// SignalR Event Log Statistics DTO
    /// </summary>
    public class SignalREventLogStatsDto
    {
        public int TotalEvents { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int NoTargetCount { get; set; }
        public int AcknowledgedCount { get; set; }
        public double AverageDurationMs { get; set; }
        public Dictionary<string, int> EventTypeCounts { get; set; } = new();
        public Dictionary<string, int> TargetTypeCounts { get; set; } = new();
    }

    /// <summary>
    /// Paged Result DTO
    /// </summary>
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

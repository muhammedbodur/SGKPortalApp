using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
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
}

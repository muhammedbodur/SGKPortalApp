namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
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
}

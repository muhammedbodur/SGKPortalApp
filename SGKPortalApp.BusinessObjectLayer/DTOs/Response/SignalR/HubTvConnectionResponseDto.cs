namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// Hub TV Connection Response DTO
    /// </summary>
    public class HubTvConnectionResponseDto
    {
        public int HubTvConnectionId { get; set; }
        public int HubConnectionId { get; set; }
        public int TvId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
    }
}

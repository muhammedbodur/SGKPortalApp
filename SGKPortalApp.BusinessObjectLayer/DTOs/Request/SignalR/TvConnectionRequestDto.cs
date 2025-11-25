namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    public class TvConnectionRequestDto
    {
        public int TvId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string TcKimlikNo { get; set; } = string.Empty;
    }
}

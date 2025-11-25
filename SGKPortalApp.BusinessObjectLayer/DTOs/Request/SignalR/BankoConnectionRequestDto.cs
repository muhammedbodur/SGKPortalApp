namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    public class BankoConnectionRequestDto
    {
        public int BankoId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string TcKimlikNo { get; set; } = string.Empty;
    }
}

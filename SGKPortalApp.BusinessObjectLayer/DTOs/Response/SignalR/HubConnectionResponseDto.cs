using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// Hub Connection Response DTO
    /// </summary>
    public class HubConnectionResponseDto
    {
        public int HubConnectionId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = "MainLayout";
        public ConnectionStatus ConnectionStatus { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
    }
}

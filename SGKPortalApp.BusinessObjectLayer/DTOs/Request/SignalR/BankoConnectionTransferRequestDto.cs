namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Banko bağlantısını yeni SignalR connection'a devretme isteği
    /// </summary>
    public class BankoConnectionTransferRequestDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}

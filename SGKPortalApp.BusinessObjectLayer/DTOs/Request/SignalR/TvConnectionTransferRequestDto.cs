namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// TV bağlantısını yeni SignalR connection'a devretme isteği
    /// </summary>
    public class TvConnectionTransferRequestDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}

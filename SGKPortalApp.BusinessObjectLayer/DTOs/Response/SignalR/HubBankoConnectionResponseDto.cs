namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// Hub Banko Connection Response DTO
    /// </summary>
    public class HubBankoConnectionResponseDto
    {
        public int HubBankoConnectionId { get; set; }
        public int HubConnectionId { get; set; }
        public int BankoId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public bool BankoModuAktif { get; set; }
        public DateTime BankoModuBaslangic { get; set; }
        public DateTime? BankoModuBitis { get; set; }
    }
}

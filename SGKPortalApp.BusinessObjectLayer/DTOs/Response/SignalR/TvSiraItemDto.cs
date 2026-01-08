namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// TV ekranındaki sıra listesi için item DTO
    /// </summary>
    public class TvSiraItemDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string KatTipi { get; set; } = string.Empty;
        public int SiraNo { get; set; }
    }
}

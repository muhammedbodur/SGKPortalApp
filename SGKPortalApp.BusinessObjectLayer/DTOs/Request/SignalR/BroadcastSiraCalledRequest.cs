using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Sıra çağırma broadcast request
    /// </summary>
    public class BroadcastSiraCalledRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int CallerBankoId { get; set; }
        public string BankoNo { get; set; } = string.Empty;
        public string CallerPersonelTc { get; set; } = string.Empty;
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// TV'ye sıra çağırma broadcast request (yeni yapı)
    /// </summary>
    public class BroadcastSiraCalledToTvRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int BankoId { get; set; }
        public string BankoNo { get; set; } = string.Empty;
        public string UpdateType { get; set; } = "SiraCalled";
        public bool ShowOverlay { get; set; } = true;
    }
}

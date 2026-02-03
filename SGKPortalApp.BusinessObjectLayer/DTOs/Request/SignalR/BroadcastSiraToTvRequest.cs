using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// TV'ye sıra broadcast request (eski yapı)
    /// </summary>
    public class BroadcastSiraToTvRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public string BankoNo { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
    }
}

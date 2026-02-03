using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Yeni sıra (Kiosk) broadcast request
    /// </summary>
    public class BroadcastNewSiraRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }
}

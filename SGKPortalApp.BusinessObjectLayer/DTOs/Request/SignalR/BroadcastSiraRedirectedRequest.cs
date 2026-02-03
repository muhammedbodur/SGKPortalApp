using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Sıra yönlendirme broadcast request
    /// </summary>
    public class BroadcastSiraRedirectedRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int SourceBankoId { get; set; }
        public int? TargetBankoId { get; set; }
        public string SourcePersonelTc { get; set; } = string.Empty;
    }
}

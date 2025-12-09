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

    /// <summary>
    /// Sıra tamamlama broadcast request
    /// </summary>
    public class BroadcastSiraCompletedRequest
    {
        public int SiraId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }

    /// <summary>
    /// Sıra iptal broadcast request
    /// </summary>
    public class BroadcastSiraCancelledRequest
    {
        public int SiraId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }

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

    /// <summary>
    /// Yeni sıra (Kiosk) broadcast request
    /// </summary>
    public class BroadcastNewSiraRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }

    /// <summary>
    /// TV'ye sıra broadcast request (eski yapı)
    /// </summary>
    public class BroadcastSiraToTvRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public string BankoNo { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
    }

    /// <summary>
    /// TV'ye sıra çağırma broadcast request (yeni yapı)
    /// </summary>
    public class BroadcastSiraCalledToTvRequest
    {
        public SiraCagirmaResponseDto Sira { get; set; } = null!;
        public int BankoId { get; set; }
        public string BankoNo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Banko panel güncelleme request
    /// </summary>
    public class BroadcastBankoPanelGuncellemesiRequest
    {
        public int SiraId { get; set; }
    }
}

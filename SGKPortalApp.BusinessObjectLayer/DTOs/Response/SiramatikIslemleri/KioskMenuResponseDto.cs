using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskMenuResponseDto
    {
        public int KioskMenuId { get; set; }
        public string MenuAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public int MenuSira { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime? DuzenlenmeTarihi { get; set; }

        public List<KioskSummaryDto> Kiosklar { get; set; } = new();
    }
}

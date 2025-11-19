using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskMenuResponseDto
    {
        public int KioskMenuId { get; set; }
        public string MenuAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime? DuzenlenmeTarihi { get; set; }

        public List<KioskSummaryDto> Kiosklar { get; set; } = new();
    }

    public class KioskSummaryDto
    {
        public int KioskId { get; set; }
        public string KioskAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
    }
}

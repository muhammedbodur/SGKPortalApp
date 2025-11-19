using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskMenuIslemResponseDto
    {
        public int KioskMenuIslemId { get; set; }
        public int KioskMenuId { get; set; }
        public string? KioskMenuAdi { get; set; }
        public string IslemAdi { get; set; } = string.Empty;
        public int KanalAltId { get; set; }
        public string? KanalAltAdi { get; set; }
        public int MenuSira { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}

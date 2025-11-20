namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskSummaryDto
    {
        public int KioskId { get; set; }
        public string KioskAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
    }
}

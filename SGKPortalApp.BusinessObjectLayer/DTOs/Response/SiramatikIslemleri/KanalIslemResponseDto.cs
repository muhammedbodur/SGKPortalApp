using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalIslemResponseDto
    {
        public int KanalIslemId { get; set; }
        public int KanalId { get; set; }
        public string KanalAdi { get; set; } = string.Empty;
        public string KanalIslemAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        
        // Navigation properties için ek bilgiler
        public int KanalAltIslemSayisi { get; set; }
    }
}

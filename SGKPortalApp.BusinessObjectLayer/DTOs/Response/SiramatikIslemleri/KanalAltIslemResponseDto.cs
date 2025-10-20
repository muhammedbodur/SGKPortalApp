using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalAltIslemResponseDto
    {
        public int KanalAltIslemId { get; set; }
        public int KanalAltId { get; set; }
        public string KanalAltAdi { get; set; } = string.Empty;
        public int KanalIslemId { get; set; }
        public string KanalIslemAdi { get; set; } = string.Empty;
        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        
        // Navigation properties için ek bilgiler
        public int PersonelSayisi { get; set; }
    }
}

using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalResponseDto
    {
        public int KanalId { get; set; }
        public string KanalAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        
        // Navigation properties için ek bilgiler
        public int KanalAltSayisi { get; set; }
        public int KanalIslemSayisi { get; set; }
    }
}

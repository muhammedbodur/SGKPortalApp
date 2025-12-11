using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalAltIslemResponseDto
    {
        public int KanalAltIslemId { get; set; }
        public int KanalAltId { get; set; }
        public string KanalAltAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public int KanalIslemId { get; set; }
        public string KanalAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        
        // Navigation properties için ek bilgiler
        public int PersonelSayisi { get; set; }
        
        // Uzmanlık seviyelerine göre personel sayıları
        public int SefSayisi { get; set; }
        public int UzmanSayisi { get; set; }
        public int YrdUzmanSayisi { get; set; }
    }
}

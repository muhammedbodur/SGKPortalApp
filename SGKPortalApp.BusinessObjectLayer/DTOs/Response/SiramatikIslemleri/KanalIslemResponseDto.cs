using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KanalIslemResponseDto
    {
        public int KanalIslemId { get; set; }

        // Kanal bilgileri
        public int KanalId { get; set; }
        public string KanalAdi { get; set; } = string.Empty;

        // Departman-Hizmet Binası kombinasyonu
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        // Numara aralığı
        public int BaslangicNumara { get; set; }
        public int BitisNumara { get; set; }

        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }

        // Navigation properties
        public int KanalAltIslemSayisi { get; set; }
    }
}

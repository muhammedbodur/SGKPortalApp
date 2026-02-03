using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Banko bilgilerini döndürmek için kullanılan DTO
    /// </summary>
    public class BankoResponseDto
    {
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public BankoTipi BankoTipi { get; set; }
        public string BankoTipiAdi { get; set; } = string.Empty;
        public KatTipi KatTipi { get; set; }
        public string KatTipiAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public string? BankoAciklama { get; set; }
        public int BankoSira { get; set; }

        // Departman-Hizmet Binası kombinasyonu
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        // Atanmış Personel Bilgisi
        public PersonelAtamaDto? AtananPersonel { get; set; }
        public bool BankoMusaitMi { get; set; }

        // Bağlantı Durumu
        public bool IsConnected { get; set; }

        // Audit Fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
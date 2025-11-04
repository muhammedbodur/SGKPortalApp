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
        public Aktiflik BankoAktiflik { get; set; }
        public string? BankoAciklama { get; set; }
        public int BankoSira { get; set; }

        // Foreign Key - Hizmet Binası
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        // Atanmış Personel Bilgisi
        public PersonelAtamaDto? AtananPersonel { get; set; }
        public bool BankoMusaitMi { get; set; }

        // Audit Fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }

    /// <summary>
    /// Bankoya atanmış personel bilgisi
    /// </summary>
    public class PersonelAtamaDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string ServisAdi { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public DateTime AtanmaTarihi { get; set; }
    }
}
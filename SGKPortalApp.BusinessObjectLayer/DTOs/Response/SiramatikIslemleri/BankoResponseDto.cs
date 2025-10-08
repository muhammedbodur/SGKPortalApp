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
        public KatTipi KatTipi { get; set; }
        public Aktiflik BankoAktiflik { get; set; }

        // Foreign Key - Hizmet Binası
        public int HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }

        // Foreign Key - Atanmış Personel (Opsiyonel)
        public string? TcKimlikNo { get; set; }
        public string? PersonelAdSoyad { get; set; }
        public string? PersonelResim { get; set; }

        // İstatistikler
        public int BekleyenSiraSayisi { get; set; }
        public int TamamlananSiraSayisi { get; set; }

        // Audit Fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
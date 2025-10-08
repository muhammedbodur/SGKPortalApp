using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// TV ekran bilgilerini döndürmek için kullanılan DTO
    /// </summary>
    public class TvResponseDto
    {
        public int TvId { get; set; }
        public string TvAdi { get; set; } = string.Empty;
        public string? TvAciklama { get; set; }
        public Aktiflik TvAktiflik { get; set; }

        // Foreign Key - Hizmet Binası
        public int HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }

        // İlişkili Banko Sayısı
        public int BankoSayisi { get; set; }

        // Audit Fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
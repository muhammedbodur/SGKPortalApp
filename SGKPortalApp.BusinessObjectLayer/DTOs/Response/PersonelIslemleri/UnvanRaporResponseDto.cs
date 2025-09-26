using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    /// <summary>
    /// Unvan Rapor Response DTO - Unvan bazında personel istatistikleri için
    /// </summary>
    public class UnvanRaporResponseDto
    {
        /// <summary>
        /// Unvan ID
        /// </summary>
        public int UnvanId { get; set; }

        /// <summary>
        /// Unvan adı
        /// </summary>
        public string UnvanAdi { get; set; } = string.Empty;

        /// <summary>
        /// Toplam personel sayısı
        /// </summary>
        public int ToplamPersonel { get; set; }

        /// <summary>
        /// Aktif personel sayısı
        /// </summary>
        public int AktifPersonel { get; set; }

        /// <summary>
        /// Pasif personel sayısı
        /// </summary>
        public int PasifPersonel { get; set; }

        /// <summary>
        /// Emekli personel sayısı
        /// </summary>
        public int EmekliPersonel { get; set; }

        /// <summary>
        /// Unvanın durumu (Aktif/Pasif)
        /// </summary>
        public Aktiflik UnvanDurumu { get; set; }

        /// <summary>
        /// Erkek personel sayısı
        /// </summary>
        public int ErkekPersonel { get; set; }

        /// <summary>
        /// Kadın personel sayısı
        /// </summary>
        public int KadinPersonel { get; set; }

        /// <summary>
        /// Kullanım yüzdesi (aktif personel / toplam personel * 100)
        /// </summary>
        public decimal KullanimYuzdesi => ToplamPersonel > 0
            ? Math.Round((decimal)AktifPersonel / ToplamPersonel * 100, 2)
            : 0;

        /// <summary>
        /// Unvan oluşturulma tarihi
        /// </summary>
        public DateTime EklenmeTarihi { get; set; }

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
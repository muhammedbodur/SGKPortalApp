using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk'tan sıra alma yanıtı için DTO
    /// Masaüstü kiosk uygulamasını simüle eder
    /// </summary>
    public class KioskSiraAlResponseDto
    {
        /// <summary>
        /// Oluşturulan sıra ID
        /// </summary>
        public int SiraId { get; set; }

        /// <summary>
        /// Sıra numarası (örn: 101, 102, ...)
        /// </summary>
        public int SiraNo { get; set; }

        /// <summary>
        /// Kanal alt adı (örn: "4/B İŞLEMLERİ")
        /// </summary>
        public string KanalAltAdi { get; set; } = string.Empty;

        /// <summary>
        /// Hizmet binası ID
        /// </summary>
        public int HizmetBinasiId { get; set; }

        /// <summary>
        /// Hizmet binası adı
        /// </summary>
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        /// <summary>
        /// Kanal alt işlem ID
        /// </summary>
        public int KanalAltIslemId { get; set; }

        /// <summary>
        /// Sıra alış zamanı
        /// </summary>
        public DateTime SiraAlisZamani { get; set; }

        /// <summary>
        /// Tahmini bekleme süresi (dakika)
        /// </summary>
        public int? TahminiBeklemeSuresi { get; set; }

        /// <summary>
        /// Öndeki bekleyen sıra sayısı
        /// </summary>
        public int BekleyenSiraSayisi { get; set; }

        /// <summary>
        /// Bu işlem için banko modunda aktif personel var mı?
        /// </summary>
        public bool AktifPersonelVar { get; set; }

        /// <summary>
        /// Fiş yazdırma için mesaj
        /// </summary>
        public string FisMesaji { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk'tan sıra alma isteği için DTO
    /// Eski proje mantığı: Sadece KanalAltIslemId ile çalışır
    /// HizmetBinasiId, KanalAltIslem tablosundan otomatik alınır
    /// </summary>
    public class KioskSiraAlRequestDto
    {
        /// <summary>
        /// Kanal Alt İşlem ID - Hangi işlem için sıra alınacak
        /// Bu ID üzerinden HizmetBinasiId ve diğer bilgiler otomatik alınır
        /// </summary>
        [Required(ErrorMessage = "KanalAltIslemId zorunludur")]
        public int KanalAltIslemId { get; set; }

        /// <summary>
        /// Kiosk ID - Hangi kiosk'tan sıra alındığı (opsiyonel, loglama için)
        /// </summary>
        public int? KioskId { get; set; }
    }
}

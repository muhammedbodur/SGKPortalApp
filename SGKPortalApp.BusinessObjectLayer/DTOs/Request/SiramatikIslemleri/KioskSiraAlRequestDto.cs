using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk'tan sıra alma isteği için DTO
    /// Masaüstü kiosk uygulamasını simüle eder
    /// </summary>
    public class KioskSiraAlRequestDto
    {
        /// <summary>
        /// Kiosk menü işlem ID - Hangi işlem için sıra alınacak
        /// KioskMenuIslem üzerinden KanalAltId'ye ulaşılır
        /// </summary>
        [Required(ErrorMessage = "KioskMenuIslemId zorunludur")]
        public int KioskMenuIslemId { get; set; }

        /// <summary>
        /// Hizmet binası ID - Hangi binada sıra alınacak
        /// </summary>
        [Required(ErrorMessage = "HizmetBinasiId zorunludur")]
        public int HizmetBinasiId { get; set; }

        /// <summary>
        /// Kiosk ID - Hangi kiosk'tan sıra alındığı (opsiyonel, loglama için)
        /// </summary>
        public int? KioskId { get; set; }
    }
}

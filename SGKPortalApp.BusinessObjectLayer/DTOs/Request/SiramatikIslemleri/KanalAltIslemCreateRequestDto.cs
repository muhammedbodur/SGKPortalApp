using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalAltIslemCreateRequestDto
    {
        [Required(ErrorMessage = "Kanal alt zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kanal alt seçiniz")]
        public int KanalAltId { get; set; }

        [Required(ErrorMessage = "Kanal işlem zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kanal işlem seçiniz")]
        public int KanalIslemId { get; set; }

        [Required(ErrorMessage = "Departman-Hizmet Binası kombinasyonu zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir departman-bina kombinasyonu seçiniz")]
        public int DepartmanHizmetBinasiId { get; set; }

        public int? KioskIslemGrupId { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

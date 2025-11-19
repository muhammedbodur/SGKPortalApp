using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KioskAltKanalEslestirmeFormModel
    {
        [Required(ErrorMessage = "Hizmet binası seçiniz")]
        public int HizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kiosk seçiniz")]
        public int KioskId { get; set; }

        [Required(ErrorMessage = "Menü öğesi seçiniz")]
        public int KioskIslemId { get; set; }

        [Required(ErrorMessage = "Kanal alt işlemi seçiniz")]
        public int KanalAltIslemId { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

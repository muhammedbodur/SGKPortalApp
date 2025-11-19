using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KioskIslemManageFormModel
    {
        [Required(ErrorMessage = "Hizmet binası seçiniz")]
        public int HizmetBinasiId { get; set; }

        [Required(ErrorMessage = "Kiosk seçiniz")]
        public int KioskId { get; set; }
    }
}

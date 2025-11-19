using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KioskFormModel
    {
        public int KioskId { get; set; }

        [Required(ErrorMessage = "Kiosk adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kiosk adı en fazla 100 karakter olabilir")]
        public string KioskAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hizmet binası seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Hizmet binası seçilmelidir")]
        public int HizmetBinasiId { get; set; }

        [StringLength(16, ErrorMessage = "Kiosk IP en fazla 16 karakter olabilir")]
        public string? KioskIp { get; set; }

        [Required(ErrorMessage = "Aktiflik seçilmelidir")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KioskMenuAtamaCreateRequestDto
    {
        [Required(ErrorMessage = "Kiosk seçilmelidir")]
        public int KioskId { get; set; }

        [Required(ErrorMessage = "Kiosk menü seçilmelidir")]
        public int KioskMenuId { get; set; }

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

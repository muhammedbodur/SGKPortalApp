using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalPersonelUpdateRequestDto
    {
        [Required(ErrorMessage = "Uzmanlık seviyesi zorunludur")]
        public PersonelUzmanlik Uzmanlik { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

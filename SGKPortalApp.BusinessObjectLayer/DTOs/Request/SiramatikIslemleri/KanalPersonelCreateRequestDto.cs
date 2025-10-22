using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalPersonelCreateRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kanal alt işlem ID zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kanal alt işlem seçiniz")]
        public int KanalAltIslemId { get; set; }

        [Required(ErrorMessage = "Uzmanlık seviyesi zorunludur")]
        public PersonelUzmanlik Uzmanlik { get; set; } = PersonelUzmanlik.Uzman;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

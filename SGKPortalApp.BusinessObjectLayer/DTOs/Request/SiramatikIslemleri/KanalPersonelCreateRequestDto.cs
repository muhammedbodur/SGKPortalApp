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
        public int KanalAltIslemId { get; set; }

        public PersonelUzmanlik Uzmanlik { get; set; } = PersonelUzmanlik.Normal;
    }
}

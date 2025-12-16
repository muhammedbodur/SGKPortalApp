using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class ModulUpdateRequestDto
    {
        [Required(ErrorMessage = "Modül adı zorunludur")]
        [StringLength(100, ErrorMessage = "Modül adı en fazla 100 karakter olabilir")]
        public string ModulAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modül kodu zorunludur")]
        [StringLength(10, ErrorMessage = "Modül kodu en fazla 10 karakter olabilir")]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Modül kodu sadece büyük harflerden oluşmalıdır")]
        public string ModulKodu { get; set; } = string.Empty;
    }
}

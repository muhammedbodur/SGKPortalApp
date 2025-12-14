using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class ModulUpdateRequestDto
    {
        [Required(ErrorMessage = "Modül adı zorunludur")]
        [StringLength(100, ErrorMessage = "Modül adı en fazla 100 karakter olabilir")]
        public string ModulAdi { get; set; } = string.Empty;
    }
}

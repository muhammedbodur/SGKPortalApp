using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class AtanmaNedeniCreateRequestDto
    {
        [Required(ErrorMessage = "Atanma nedeni zorunludur")]
        [StringLength(200, ErrorMessage = "Atanma nedeni en fazla 200 karakter olabilir")]
        public string AtanmaNedeni { get; set; } = string.Empty;
    }
}

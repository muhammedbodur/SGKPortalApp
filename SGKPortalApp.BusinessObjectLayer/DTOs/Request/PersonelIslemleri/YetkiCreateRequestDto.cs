using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class YetkiCreateRequestDto
    {
        [Required(ErrorMessage = "Yetki adı zorunludur")]
        [StringLength(100, ErrorMessage = "Yetki adı en fazla 100 karakter olabilir")]
        public string YetkiAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Aciklama { get; set; } = string.Empty;

        public int? UstYetkiId { get; set; }

        [StringLength(100, ErrorMessage = "Controller adı en fazla 100 karakter olabilir")]
        public string? ControllerAdi { get; set; }

        [StringLength(100, ErrorMessage = "Action adı en fazla 100 karakter olabilir")]
        public string? ActionAdi { get; set; }
    }
}

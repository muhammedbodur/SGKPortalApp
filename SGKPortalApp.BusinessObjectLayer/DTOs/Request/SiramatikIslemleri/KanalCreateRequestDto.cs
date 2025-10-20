using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalCreateRequestDto
    {
        [Required(ErrorMessage = "Kanal adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal adı en fazla 100 karakter olabilir")]
        public string KanalAdi { get; set; } = string.Empty;
    }
}

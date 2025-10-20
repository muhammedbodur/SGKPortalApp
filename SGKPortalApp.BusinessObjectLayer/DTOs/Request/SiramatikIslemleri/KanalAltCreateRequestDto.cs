using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalAltCreateRequestDto
    {
        [Required(ErrorMessage = "Kanal ID zorunludur")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Kanal alt adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal alt adı en fazla 100 karakter olabilir")]
        public string KanalAltAdi { get; set; } = string.Empty;
    }
}

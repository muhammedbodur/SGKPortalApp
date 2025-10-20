using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalUpdateRequestDto
    {
        [Required(ErrorMessage = "Kanal ID zorunludur")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Kanal adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal adı en fazla 100 karakter olabilir")]
        public string KanalAdi { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; }
    }
}

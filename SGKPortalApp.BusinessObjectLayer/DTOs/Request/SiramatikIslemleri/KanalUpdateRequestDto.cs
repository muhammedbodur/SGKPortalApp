using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalUpdateRequestDto
    {
        [Required(ErrorMessage = "Kanal adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kanal adı en fazla 100 karakter olabilir")]
        public string KanalAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

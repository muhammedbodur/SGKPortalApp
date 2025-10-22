using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KanalAltKanalUpdateRequestDto
    {
        [Required(ErrorMessage = "Ana kanal seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir ana kanal seçiniz")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Alt kanal adı zorunludur")]
        [StringLength(100, ErrorMessage = "Alt kanal adı en fazla 100 karakter olabilir")]
        public string KanalAltAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Aktiflik durumu zorunludur")]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

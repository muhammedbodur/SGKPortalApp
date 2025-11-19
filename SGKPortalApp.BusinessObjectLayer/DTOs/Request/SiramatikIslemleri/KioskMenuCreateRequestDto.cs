using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KioskMenuCreateRequestDto
    {
        [Required(ErrorMessage = "Menu adı zorunludur")]
        [StringLength(150, ErrorMessage = "Menu adı en fazla 150 karakter olabilir")]
        public string MenuAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KioskMenuUpdateRequestDto
    {
        [Required]
        public int KioskMenuId { get; set; }

        [Required(ErrorMessage = "Menu adı zorunludur")]
        [StringLength(150, ErrorMessage = "Menu adı en fazla 150 karakter olabilir")]
        public string MenuAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Menü sırası zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Menü sırası 1'den büyük olmalıdır")]
        public int MenuSira { get; set; }

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

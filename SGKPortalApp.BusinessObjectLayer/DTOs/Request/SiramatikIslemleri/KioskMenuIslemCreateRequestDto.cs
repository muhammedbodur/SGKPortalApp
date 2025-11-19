using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class KioskMenuIslemCreateRequestDto
    {
        [Required(ErrorMessage = "Kiosk menü ID zorunludur")]
        public int KioskMenuId { get; set; }

        [Required(ErrorMessage = "İşlem adı zorunludur")]
        [StringLength(200, ErrorMessage = "İşlem adı en fazla 200 karakter olabilir")]
        public string IslemAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kanal alt seçilmelidir")]
        public int KanalAltId { get; set; }

        [Required(ErrorMessage = "Menü sırası zorunludur")]
        [Range(1, 999, ErrorMessage = "Menü sırası 1-999 arasında olmalıdır")]
        public int MenuSira { get; set; }

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

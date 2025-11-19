using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KioskMenuFormModel
    {
        [Required(ErrorMessage = "Menü adı zorunludur")]
        [StringLength(150, ErrorMessage = "Menü adı en fazla 150 karakter olabilir")]
        public string MenuAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }

        [Required]
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

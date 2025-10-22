using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalAltFormModel
    {
        [Required(ErrorMessage = "Ana kanal seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir ana kanal seçiniz")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Alt kanal adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Alt kanal adı 2-100 karakter arasında olmalıdır")]
        public string KanalAltAdi { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; }
    }
}

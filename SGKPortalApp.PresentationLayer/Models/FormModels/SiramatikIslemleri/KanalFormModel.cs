using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalFormModel
    {
        [Required(ErrorMessage = "Ana kanal adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ana kanal adı 2-100 karakter arasında olmalıdır")]
        public string KanalAdi { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; }
    }
}

using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    public class UnvanFormModel
    {
        [Required(ErrorMessage = "Unvan adı zorunludur")]
        [StringLength(150, ErrorMessage = "Unvan adı en fazla 150 karakter olabilir")]
        [MinLength(2, ErrorMessage = "Unvan adı en az 2 karakter olmalıdır")]
        public string UnvanAdi { get; set; } = string.Empty;

        public Aktiflik UnvanAktiflik { get; set; } = Aktiflik.Aktif;
    }
}

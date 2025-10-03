using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels
{
    public class ServisFormModel
    {
        [Required(ErrorMessage = "Servis adı zorunludur")]
        [StringLength(150, ErrorMessage = "Servis adı en fazla 150 karakter olabilir")]
        [MinLength(2, ErrorMessage = "Servis adı en az 2 karakter olmalıdır")]
        public string ServisAdi { get; set; } = string.Empty;

        public Aktiflik ServisAktiflik { get; set; } = Aktiflik.Aktif;
    }
}

using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    public class DepartmanFormModel
    {
        [Required(ErrorMessage = "Departman adı zorunludur")]
        [StringLength(150, ErrorMessage = "Departman adı en fazla 150 karakter olabilir")]
        [MinLength(2, ErrorMessage = "Departman adı en az 2 karakter olmalıdır")]
        public string DepartmanAdi { get; set; } = string.Empty;

        public Aktiflik DepartmanAktiflik { get; set; } = Aktiflik.Aktif;
    }
}
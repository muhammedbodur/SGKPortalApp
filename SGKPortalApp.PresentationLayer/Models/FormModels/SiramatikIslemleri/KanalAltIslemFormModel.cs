using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalAltIslemFormModel
    {
        [Required(ErrorMessage = "Alt işlem seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir alt işlem seçiniz")]
        public int KanalAltId { get; set; }

        [Required(ErrorMessage = "Kanal işlem seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kanal işlem seçiniz")]
        public int KanalIslemId { get; set; }

        [Required(ErrorMessage = "Departman-Hizmet binası seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Departman-Hizmet binası seçilmelidir")]
        public int DepartmanHizmetBinasiId { get; set; }

        public int HizmetBinasiId { get; set; } // Response'dan gelen bina bilgisi için

        public int? KioskIslemGrupId { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}

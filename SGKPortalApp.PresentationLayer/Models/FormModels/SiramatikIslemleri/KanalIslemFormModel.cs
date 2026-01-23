using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalIslemFormModel
    {
        [Required(ErrorMessage = "Ana kanal seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kanal seçiniz")]
        public int KanalId { get; set; }
        public string KanalAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman-Hizmet binası seçilmelidir")]
        [Range(1, int.MaxValue, ErrorMessage = "Departman-Hizmet binası seçilmelidir")]
        public int DepartmanHizmetBinasiId { get; set; }

        public int HizmetBinasiId { get; set; } // Response'dan gelen bina bilgisi için

        [Range(0, 9999, ErrorMessage = "Başlangıç numarası 0 ile 9999 arasında olmalıdır")]
        public int BaslangicNumara { get; set; }

        [Range(0, 9999, ErrorMessage = "Bitiş numarası 0 ile 9999 arasında olmalıdır")]
        public int BitisNumara { get; set; }

        [Range(1, 999, ErrorMessage = "Sıra 1 ile 999 arasında olmalıdır")]
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; }
    }
}
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalIslemFormModel
    {
        [Required(ErrorMessage = "Ana kanal seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir kanal seçiniz")]
        public int KanalId { get; set; }

        [Required(ErrorMessage = "Kanal işlem adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Kanal işlem adı 2-100 karakter arasında olmalıdır")]
        public string KanalIslemAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hizmet binası seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir hizmet binası seçiniz")]
        public int HizmetBinasiId { get; set; }

        [Range(0, 9999, ErrorMessage = "Başlangıç numarası 0 ile 9999 arasında olmalıdır")]
        public int BaslangicNumara { get; set; }

        [Range(0, 9999, ErrorMessage = "Bitiş numarası 0 ile 9999 arasında olmalıdır")]
        public int BitisNumara { get; set; }

        public Aktiflik Aktiflik { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.Common
{
    public class HizmetBinasiFormModel
    {
        [Required(ErrorMessage = "Hizmet Binası adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Hizmet Binası adı 2-100 karakter arasında olmalıdır")]
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Departman seçimi zorunludur")]
        public int DepartmanId { get; set; }

        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
        public string? Adres { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

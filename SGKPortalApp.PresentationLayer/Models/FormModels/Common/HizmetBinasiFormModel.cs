using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.Common
{
    public class HizmetBinasiFormModel
    {
        [Required(ErrorMessage = "Hizmet Binası adı zorunludur")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Hizmet Binası adı 2-100 karakter arasında olmalıdır")]
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        /// <summary>
        /// Bu binada bulunan departmanların ID'leri (many-to-many)
        /// </summary>
        public List<int> DepartmanIds { get; set; } = new();

        /// <summary>
        /// Ana departman ID'si (opsiyonel)
        /// </summary>
        public int? AnaDepartmanId { get; set; }

        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
        public string? Adres { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

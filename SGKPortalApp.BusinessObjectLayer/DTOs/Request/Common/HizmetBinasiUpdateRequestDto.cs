using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class HizmetBinasiUpdateRequestDto
    {
        [Required(ErrorMessage = "Hizmet Binası adı zorunludur")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Hizmet Binası adı 2-200 karakter arasında olmalıdır")]
        public required string HizmetBinasiAdi { get; set; }

        /// <summary>
        /// Bu binada bulunan departmanların ID'leri (many-to-many)
        /// </summary>
        public List<int> DepartmanIds { get; set; } = new();

        /// <summary>
        /// Ana departman ID'si (opsiyonel)
        /// </summary>
        public int? AnaDepartmanId { get; set; }

        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir")]
        public string Adres { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; }
    }
}
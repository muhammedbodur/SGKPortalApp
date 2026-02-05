using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class OnemliLinkUpdateRequestDto
    {
        [Required(ErrorMessage = "Link adı zorunludur")]
        [StringLength(100)]
        public string LinkAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL zorunludur")]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Sıra 1'den büyük olmalıdır")]
        public int Sira { get; set; } = 1;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

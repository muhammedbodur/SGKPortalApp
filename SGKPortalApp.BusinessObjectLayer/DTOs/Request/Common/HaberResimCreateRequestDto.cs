using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class HaberResimCreateRequestDto
    {
        [Required(ErrorMessage = "Haber ID zorunludur")]
        public int HaberId { get; set; }

        [Required(ErrorMessage = "Resim URL zorunludur")]
        [StringLength(500, ErrorMessage = "Resim URL en fazla 500 karakter olabilir")]
        public string ResimUrl { get; set; } = string.Empty;

        public bool IsVitrin { get; set; } = false;

        public int Sira { get; set; } = 1;
    }
}

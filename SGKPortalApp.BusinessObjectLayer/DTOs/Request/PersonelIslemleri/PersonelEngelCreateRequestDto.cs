using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelEngelCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Engel derecesi zorunludur")]
        public EngelDerecesi EngelDerecesi { get; set; }

        [StringLength(200)]
        public string? EngelNedeni1 { get; set; }

        [StringLength(200)]
        public string? EngelNedeni2 { get; set; }

        [StringLength(200)]
        public string? EngelNedeni3 { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

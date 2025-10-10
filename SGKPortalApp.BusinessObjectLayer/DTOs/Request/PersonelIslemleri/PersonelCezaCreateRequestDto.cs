using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCezaCreateRequestDto
    {
        [Required(ErrorMessage = "Personel TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ceza sebebi zorunludur")]
        [StringLength(200)]
        public string CezaSebebi { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AltBendi { get; set; }

        [Required(ErrorMessage = "Ceza tarihi zorunludur")]
        public DateTime CezaTarihi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

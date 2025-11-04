using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class BankoPersonelAtaDto
    {
        [Required(ErrorMessage = "Banko seçilmelidir")]
        public int BankoId { get; set; }

        [Required(ErrorMessage = "Personel seçilmelidir")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;
    }
}

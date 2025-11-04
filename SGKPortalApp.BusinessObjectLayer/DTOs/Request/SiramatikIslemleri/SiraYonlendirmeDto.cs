using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri
{
    public class SiraYonlendirmeDto
    {
        [Required(ErrorMessage = "Sıra seçilmelidir")]
        public int SiraId { get; set; }

        [Required(ErrorMessage = "Yönlendiren personel bilgisi gereklidir")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string YonlendirenPersonelTc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kaynak banko bilgisi gereklidir")]
        public int YonlendirmeBankoId { get; set; }

        [Required(ErrorMessage = "Hedef banko seçilmelidir")]
        public int HedefBankoId { get; set; }

        [Required(ErrorMessage = "Yönlendirme tipi seçilmelidir")]
        public YonlendirmeTipi YonlendirmeTipi { get; set; }

        [StringLength(500, ErrorMessage = "Yönlendirme nedeni en fazla 500 karakter olabilir")]
        public string? YonlendirmeNedeni { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    public class SpecialCardUpdateRequestDto
    {
        [Required(ErrorMessage = "Kart tipi zorunludur")]
        public CardType CardType { get; set; }

        [Required(ErrorMessage = "Kart numarası zorunludur")]
        [Range(1, long.MaxValue, ErrorMessage = "Kart numarası geçerli bir değer olmalıdır")]
        public long CardNumber { get; set; }

        [Required(ErrorMessage = "Kart adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kart adı en fazla 100 karakter olabilir")]
        public string CardName { get; set; } = string.Empty;

        [Required(ErrorMessage = "EnrollNumber zorunludur")]
        [StringLength(50, ErrorMessage = "EnrollNumber en fazla 50 karakter olabilir")]
        public string EnrollNumber { get; set; } = string.Empty;

        [StringLength(12, ErrorMessage = "NickName en fazla 12 karakter olabilir")]
        public string? NickName { get; set; }

        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
        public string? Notes { get; set; }
    }
}

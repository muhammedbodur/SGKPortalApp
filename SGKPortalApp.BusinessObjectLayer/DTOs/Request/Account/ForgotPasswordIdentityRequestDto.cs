using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account
{
    /// <summary>
    /// Şifremi unuttum - Kimlik doğrulama isteği
    /// </summary>
    public class ForgotPasswordIdentityRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sicil No zorunludur")]
        public string SicilNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum Tarihi zorunludur")]
        public DateTime DogumTarihi { get; set; }

        [Required(ErrorMessage = "Telefon son 4 hanesi zorunludur")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Telefon son 4 hanesi 4 karakter olmalıdır")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Sadece rakam giriniz")]
        public string TelefonSon4Hane { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth
{
    /// <summary>
    /// Login isteği için DTO
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "TC Kimlik No en az 9 karakter olmalıdır")]
        [RegularExpression(@"^(TV\d{7}|\d{11})$", ErrorMessage = "TC Kimlik No 11 haneli rakam veya TV formatında (TV0000001) olmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Şifre en az 4 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Beni hatırla seçeneği (opsiyonel)
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}

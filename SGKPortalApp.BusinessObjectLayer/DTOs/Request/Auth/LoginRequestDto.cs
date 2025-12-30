using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth
{
    /// <summary>
    /// Login modu
    /// </summary>
    public enum LoginMode
    {
        /// <summary>
        /// Standart login (TC Kimlik No + Şifre)
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Active Directory login (Domain Username + Domain Şifre)
        /// </summary>
        ActiveDirectory = 1
    }

    /// <summary>
    /// Login isteği için DTO
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Login modu (Standard veya ActiveDirectory)
        /// Default: Standard
        /// </summary>
        public LoginMode Mode { get; set; } = LoginMode.Standard;

        /// <summary>
        /// TC Kimlik No (Standard mode için)
        /// </summary>
        [StringLength(11, MinimumLength = 9, ErrorMessage = "TC Kimlik No en az 9 karakter olmalıdır")]
        [RegularExpression(@"^(TV\d{7}|\d{11})$", ErrorMessage = "TC Kimlik No 11 haneli rakam veya TV formatında (TV0000001) olmalıdır")]
        public string? TcKimlikNo { get; set; }

        /// <summary>
        /// Domain kullanıcı adı (ActiveDirectory mode için)
        /// Örnek: "mbodur3" (domain prefix olmadan)
        /// </summary>
        [StringLength(100, ErrorMessage = "Domain kullanıcı adı maksimum 100 karakter olmalıdır")]
        public string? DomainUsername { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Şifre en az 4 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Beni hatırla seçeneği (opsiyonel)
        /// </summary>
        public bool RememberMe { get; set; } = false;

        /// <summary>
        /// İstemci IP adresi (Controller tarafından set edilir)
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// İstemci User-Agent bilgisi (Controller tarafından set edilir)
        /// </summary>
        public string? UserAgent { get; set; }
    }
}

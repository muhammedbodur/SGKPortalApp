using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth
{
    /// <summary>
    /// Şifre sıfırlama için kimlik doğrulama isteği
    /// 4 alan ile kullanıcıyı doğrular: TC, Sicil No, Doğum Tarihi, Email
    /// </summary>
    public class VerifyIdentityRequestDto
    {
        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No sadece rakamlardan oluşmalıdır")]
        public string TcKimlikNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sicil No zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Sicil No giriniz")]
        public int SicilNo { get; set; }

        [Required(ErrorMessage = "Doğum Tarihi zorunludur")]
        public DateTime DogumTarihi { get; set; }

        [Required(ErrorMessage = "Email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}

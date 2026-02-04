using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account
{
    /// <summary>
    /// Şifremi unuttum - Güvenlik sorusu yanıtı
    /// </summary>
    public class ForgotPasswordSecurityRequestDto
    {
        [Required(ErrorMessage = "Güvenlik sorusu yanıtı zorunludur")]
        public string SecurityAnswer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Anne kızlık soyadı zorunludur")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "İlk 3 harfi giriniz")]
        public string AnneKizlikSoyadi { get; set; } = string.Empty;
    }
}

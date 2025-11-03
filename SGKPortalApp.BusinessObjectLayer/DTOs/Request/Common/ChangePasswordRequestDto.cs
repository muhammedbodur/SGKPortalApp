using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    /// <summary>
    /// Şifre değiştirme isteği DTO
    /// </summary>
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Eski şifre zorunludur")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Eski şifre en az 1 karakter olmalıdır")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifre ve tekrarı eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Auth
{
    /// <summary>
    /// Authentication işlemleri için servis interface
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcı girişi yapar
        /// TC Kimlik No ve şifre ile doğrulama
        /// </summary>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        /// <summary>
        /// Şifre sıfırlama için kimlik doğrulama
        /// TC, Sicil No, Doğum Tarihi ve Email ile doğrulama
        /// </summary>
        Task<VerifyIdentityResponseDto> VerifyIdentityAsync(VerifyIdentityRequestDto request);

        /// <summary>
        /// Şifre sıfırlama
        /// Kimlik doğrulandıktan sonra yeni şifre belirleme
        /// </summary>
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);

        /// <summary>
        /// Şifre hash'leme (BCrypt kullanarak)
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Şifre doğrulama (hash ile karşılaştırma)
        /// </summary>
        bool VerifyPassword(string password, string hashedPassword);
    }
}

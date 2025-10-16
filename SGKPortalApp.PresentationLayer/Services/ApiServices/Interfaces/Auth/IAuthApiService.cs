using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth
{
    public interface IAuthApiService
    {
        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        /// <summary>
        /// Şifre sıfırlama için kimlik doğrulama (4 alan kontrolü)
        /// </summary>
        Task<VerifyIdentityResponseDto?> VerifyIdentityAsync(VerifyIdentityRequestDto request);

        /// <summary>
        /// Şifre sıfırlama
        /// </summary>
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);

        /// <summary>
        /// Çıkış
        /// </summary>
        Task<bool> LogoutAsync();
    }
}
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;

namespace SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Interfaces
{
    /// <summary>
    /// Cookie-based Authentication işlemleri için servis
    /// Cookie oluşturma, silme vb.
    /// </summary>
    public interface ICookieAuthService
    {
        /// <summary>
        /// Login başarılı olduğunda Cookie oluşturur ve Claims'e kaydeder
        /// </summary>
        Task SignInAsync(LoginResponseDto loginResponse, bool rememberMe = true);

        /// <summary>
        /// Logout - Cookie'yi siler
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// Kullanıcının login olup olmadığını kontrol eder
        /// </summary>
        Task<bool> IsAuthenticatedAsync();
    }
}

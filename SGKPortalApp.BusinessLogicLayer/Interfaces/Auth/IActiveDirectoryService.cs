using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Auth
{
    /// <summary>
    /// Active Directory authentication servisi
    /// Domain kullanıcılarının doğrulanması ve email-to-TC mapping
    /// </summary>
    public interface IActiveDirectoryService
    {
        /// <summary>
        /// Domain kullanıcı adı ve şifresini Active Directory'de doğrula
        /// Başarılıysa email'den TC Kimlik No'yu bul
        /// </summary>
        /// <param name="username">Domain kullanıcı adı (domain prefix olmadan, örn: "mbodur3")</param>
        /// <param name="password">Domain şifresi</param>
        /// <returns>Başarılıysa TC Kimlik No, başarısızsa hata mesajı</returns>
        Task<ActiveDirectoryValidationResult> ValidateAndMapUserAsync(string username, string password);

        /// <summary>
        /// Email adresinden TC Kimlik No'yu bul
        /// </summary>
        /// <param name="email">Email adresi (örn: "mbodur3@sgk.gov.tr")</param>
        /// <returns>TC Kimlik No (bulunamazsa null)</returns>
        Task<string?> GetTcKimlikNoByEmailAsync(string email);
    }
}

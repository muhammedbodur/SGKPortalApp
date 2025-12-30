using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System.DirectoryServices.AccountManagement;

namespace SGKPortalApp.BusinessLogicLayer.Services.Auth
{
    /// <summary>
    /// Active Directory authentication servisi
    /// Domain kullanıcılarının doğrulanması ve email-to-TC mapping
    /// </summary>
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        // AD Domain yapılandırması (appsettings.json'dan okunabilir)
        private readonly string _domainName;
        private readonly string _emailSuffix;

        public ActiveDirectoryService(
            ILogger<ActiveDirectoryService> logger,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;

            // Domain bilgilerini appsettings'ten al (yoksa default değerler)
            _domainName = _configuration["ActiveDirectory:DomainName"] ?? "sgk.gov.tr";
            _emailSuffix = _configuration["ActiveDirectory:EmailSuffix"] ?? "@sgk.gov.tr";
        }

        public async Task<ActiveDirectoryValidationResult> ValidateAndMapUserAsync(string username, string password)
        {
            try
            {
                // 1. AD Credentials'ı doğrula
                bool isValid = ValidateActiveDirectoryCredentials(username, password);

                if (!isValid)
                {
                    _logger.LogWarning("AD validation başarısız - Username: {Username}", username);
                    return new ActiveDirectoryValidationResult
                    {
                        Success = false,
                        Message = "Domain kullanıcı adı veya şifre hatalı!"
                    };
                }

                _logger.LogInformation("AD validation başarılı - Username: {Username}", username);

                // 2. Email'den TC Kimlik No'yu bul
                var email = $"{username}{_emailSuffix}"; // örn: mbodur3@sgk.gov.tr
                var tcKimlikNo = await GetTcKimlikNoByEmailAsync(email);

                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    _logger.LogWarning("AD kullanıcısı için TC Kimlik No bulunamadı - Email: {Email}", email);
                    return new ActiveDirectoryValidationResult
                    {
                        Success = false,
                        Message = $"Active Directory kullanıcınız ({username}) sistemde kayıtlı değil. Lütfen yöneticinizle iletişime geçin."
                    };
                }

                // 3. Personel bilgilerini al
                var personelRepo = _unitOfWork.Repository<Personel>();
                var allPersonel = await personelRepo.GetAllAsync();
                var personel = allPersonel.FirstOrDefault(p => p.TcKimlikNo == tcKimlikNo);

                if (personel == null)
                {
                    _logger.LogWarning("Personel bulunamadı - TC: {TcKimlikNo}", tcKimlikNo);
                    return new ActiveDirectoryValidationResult
                    {
                        Success = false,
                        Message = "Personel kaydı bulunamadı."
                    };
                }

                _logger.LogInformation("AD kullanıcısı başarıyla eşleştirildi - Username: {Username}, TC: {TcKimlikNo}, AdSoyad: {AdSoyad}",
                    username, tcKimlikNo, personel.AdSoyad);

                return new ActiveDirectoryValidationResult
                {
                    Success = true,
                    Message = "Active Directory doğrulaması başarılı!",
                    TcKimlikNo = tcKimlikNo,
                    AdSoyad = personel.AdSoyad,
                    Email = email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AD validation sırasında hata - Username: {Username}", username);
                return new ActiveDirectoryValidationResult
                {
                    Success = false,
                    Message = "Active Directory doğrulaması sırasında bir hata oluştu. Lütfen tekrar deneyin."
                };
            }
        }

        public async Task<string?> GetTcKimlikNoByEmailAsync(string email)
        {
            try
            {
                var personelRepo = _unitOfWork.Repository<Personel>();
                var allPersonel = await personelRepo.GetAllAsync();

                // Email case-insensitive arama
                var personel = allPersonel.FirstOrDefault(p =>
                    p.Email != null && p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (personel != null)
                {
                    _logger.LogDebug("Email'den TC bulundu - Email: {Email}, TC: {TcKimlikNo}", email, personel.TcKimlikNo);
                    return personel.TcKimlikNo;
                }

                _logger.LogWarning("Email'e ait personel bulunamadı - Email: {Email}", email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email'den TC bulma hatası - Email: {Email}", email);
                return null;
            }
        }

        /// <summary>
        /// Active Directory'de kullanıcı adı ve şifre doğrulaması yapar
        /// </summary>
        private bool ValidateActiveDirectoryCredentials(string username, string password)
        {
            try
            {
                // PrincipalContext ile AD doğrulaması
                using (var context = new PrincipalContext(ContextType.Domain, _domainName))
                {
                    // AD'de kullanıcıyı doğrula
                    bool isValid = context.ValidateCredentials(username, password);

                    _logger.LogDebug("AD credential validation - Username: {Username}, Result: {Result}",
                        username, isValid ? "SUCCESS" : "FAILED");

                    return isValid;
                }
            }
            catch (PrincipalServerDownException ex)
            {
                _logger.LogError(ex, "AD sunucusuna bağlanılamadı - Username: {Username}", username);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AD credential validation hatası - Username: {Username}", username);
                return false;
            }
        }
    }
}

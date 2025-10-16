using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.DataAccessLayer.Context;

namespace SGKPortalApp.BusinessLogicLayer.Services.Auth
{
    /// <summary>
    /// Authentication işlemleri için servis
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly SGKDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(SGKDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Kullanıcıyı TC Kimlik No ile bul (ilişkili tablolarla birlikte)
                var personel = await _context.Personeller
                    .Include(p => p.Departman)
                    .Include(p => p.Servis)
                    .Include(p => p.HizmetBinasi)
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == request.TcKimlikNo);

                if (personel == null)
                {
                    _logger.LogWarning("Login başarısız: TC Kimlik No bulunamadı - {TcKimlikNo}", request.TcKimlikNo);
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "TC Kimlik No veya şifre hatalı!"
                    };
                }

                // Şifre kontrolü (düz metin karşılaştırması)
                if (request.Password != personel.PassWord)
                {
                    _logger.LogWarning("Login başarısız: Hatalı şifre - {TcKimlikNo}", request.TcKimlikNo);
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "TC Kimlik No veya şifre hatalı!"
                    };
                }

                // Session ID oluştur
                var sessionId = Guid.NewGuid().ToString();

                // Session ID'yi veritabanına kaydet
                personel.SessionID = sessionId;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Login başarılı - {TcKimlikNo} - {AdSoyad}", personel.TcKimlikNo, personel.AdSoyad);

                // Başarılı response
                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Giriş başarılı!",
                    TcKimlikNo = personel.TcKimlikNo,
                    SicilNo = personel.SicilNo,
                    AdSoyad = personel.AdSoyad,
                    Email = personel.Email,
                    DepartmanId = personel.DepartmanId,
                    DepartmanAdi = personel.Departman?.DepartmanAdi ?? "",
                    ServisId = personel.ServisId,
                    ServisAdi = personel.Servis?.ServisAdi ?? "",
                    HizmetBinasiId = personel.HizmetBinasiId,
                    HizmetBinasiAdi = personel.HizmetBinasi?.HizmetBinasiAdi ?? "",
                    Resim = personel.Resim,
                    SessionId = sessionId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login sırasında hata oluştu - {TcKimlikNo}", request.TcKimlikNo);
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin."
                };
            }
        }

        public async Task<VerifyIdentityResponseDto> VerifyIdentityAsync(VerifyIdentityRequestDto request)
        {
            try
            {
                // 4 alan ile kullanıcıyı doğrula
                var personel = await _context.Personeller
                    .FirstOrDefaultAsync(p =>
                        p.TcKimlikNo == request.TcKimlikNo &&
                        p.SicilNo == request.SicilNo &&
                        p.DogumTarihi.Date == request.DogumTarihi.Date &&
                        p.Email.ToLower() == request.Email.ToLower());

                if (personel == null)
                {
                    _logger.LogWarning("Kimlik doğrulama başarısız - {TcKimlikNo}", request.TcKimlikNo);
                    return new VerifyIdentityResponseDto
                    {
                        Success = false,
                        Message = "Girdiğiniz bilgiler eşleşmiyor. Lütfen kontrol ediniz."
                    };
                }

                _logger.LogInformation("Kimlik doğrulama başarılı - {TcKimlikNo} - {AdSoyad}", personel.TcKimlikNo, personel.AdSoyad);

                return new VerifyIdentityResponseDto
                {
                    Success = true,
                    Message = "Kimlik doğrulandı. Yeni şifrenizi belirleyebilirsiniz.",
                    TcKimlikNo = personel.TcKimlikNo,
                    AdSoyad = personel.AdSoyad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kimlik doğrulama sırasında hata oluştu - {TcKimlikNo}", request.TcKimlikNo);
                return new VerifyIdentityResponseDto
                {
                    Success = false,
                    Message = "Kimlik doğrulama sırasında bir hata oluştu. Lütfen tekrar deneyin."
                };
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            try
            {
                var personel = await _context.Personeller
                    .FirstOrDefaultAsync(p => p.TcKimlikNo == request.TcKimlikNo);

                if (personel == null)
                {
                    _logger.LogWarning("Şifre sıfırlama başarısız: Kullanıcı bulunamadı - {TcKimlikNo}", request.TcKimlikNo);
                    return false;
                }

                // Yeni şifreyi düz metin olarak kaydet
                personel.PassWord = request.NewPassword;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Şifre başarıyla sıfırlandı - {TcKimlikNo}", request.TcKimlikNo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre sıfırlama sırasında hata oluştu - {TcKimlikNo}", request.TcKimlikNo);
                return false;
            }
        }

        public string HashPassword(string password)
        {
            // Düz metin olarak döndür (hash'leme yok)
            return password;
        }

        public bool VerifyPassword(string password, string storedPassword)
        {
            // Düz metin karşılaştırması
            return password == storedPassword;
        }
    }
}

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
                // 🆕 User tablosundan kullanıcıyı bul (Personel ile birlikte)
                var user = await _context.Users
                    .Include(u => u.Personel)
                        .ThenInclude(p => p.Departman)
                    .Include(u => u.Personel)
                        .ThenInclude(p => p.Servis)
                    .Include(u => u.Personel)
                        .ThenInclude(p => p.HizmetBinasi)
                    .FirstOrDefaultAsync(u => u.TcKimlikNo == request.TcKimlikNo);

                if (user == null || user.Personel == null)
                {
                    _logger.LogWarning("Login başarısız: Kullanıcı bulunamadı - {TcKimlikNo}", request.TcKimlikNo);
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "TC Kimlik No veya şifre hatalı!"
                    };
                }

                // Hesap aktif mi kontrol et
                if (!user.AktifMi)
                {
                    _logger.LogWarning("Login başarısız: Hesap pasif - {TcKimlikNo}", request.TcKimlikNo);
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Hesabınız pasif durumda. Lütfen yöneticinizle iletişime geçin."
                    };
                }

                // Hesap kilitli mi kontrol et
                if (user.HesapKilitTarihi.HasValue)
                {
                    _logger.LogWarning("Login başarısız: Hesap kilitli - {TcKimlikNo}", request.TcKimlikNo);
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Hesabınız kilitlenmiştir. Lütfen yöneticinizle iletişime geçin."
                    };
                }

                // Şifre kontrolü
                if (request.Password != user.PassWord)
                {
                    // Başarısız giriş sayısını artır
                    user.BasarisizGirisSayisi++;
                    
                    // 5 başarısız denemeden sonra hesabı kilitle
                    if (user.BasarisizGirisSayisi >= 5)
                    {
                        user.HesapKilitTarihi = DateTime.Now;
                        user.AktifMi = false;
                        await _context.SaveChangesAsync();
                        
                        _logger.LogWarning("Hesap kilitlendi (5 başarısız deneme) - {TcKimlikNo}", request.TcKimlikNo);
                        return new LoginResponseDto
                        {
                            Success = false,
                            Message = "5 başarısız giriş denemesi nedeniyle hesabınız kilitlenmiştir!"
                        };
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    _logger.LogWarning("Login başarısız: Hatalı şifre ({Deneme}/5) - {TcKimlikNo}", 
                        user.BasarisizGirisSayisi, request.TcKimlikNo);
                    
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = $"TC Kimlik No veya şifre hatalı! ({user.BasarisizGirisSayisi}/5 deneme)"
                    };
                }

                // Başarılı giriş - Session ID oluştur
                var sessionId = Guid.NewGuid().ToString();
                user.SessionID = sessionId;
                user.SonGirisTarihi = DateTime.Now;
                user.BasarisizGirisSayisi = 0; // Başarılı girişte sıfırla
                
                await _context.SaveChangesAsync();

                _logger.LogInformation("Login başarılı - {TcKimlikNo} - {AdSoyad}", 
                    user.TcKimlikNo, user.Personel.AdSoyad);

                // Başarılı response
                return new LoginResponseDto
                {
                    Success = true,
                    Message = "Giriş başarılı!",
                    TcKimlikNo = user.Personel.TcKimlikNo,
                    SicilNo = user.Personel.SicilNo,
                    AdSoyad = user.Personel.AdSoyad,
                    Email = user.Personel.Email, // Personel'den alınıyor
                    DepartmanId = user.Personel.DepartmanId,
                    DepartmanAdi = user.Personel.Departman?.DepartmanAdi ?? "",
                    ServisId = user.Personel.ServisId,
                    ServisAdi = user.Personel.Servis?.ServisAdi ?? "",
                    HizmetBinasiId = user.Personel.HizmetBinasiId,
                    HizmetBinasiAdi = user.Personel.HizmetBinasi?.HizmetBinasiAdi ?? "",
                    Resim = user.Personel.Resim,
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
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.TcKimlikNo == request.TcKimlikNo);

                if (user == null)
                {
                    _logger.LogWarning("Şifre sıfırlama başarısız: Kullanıcı bulunamadı - {TcKimlikNo}", request.TcKimlikNo);
                    return false;
                }

                // Yeni şifreyi düz metin olarak kaydet
                user.PassWord = request.NewPassword;
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

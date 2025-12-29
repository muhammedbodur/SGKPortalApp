using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.Auth
{
    /// <summary>
    /// Authentication işlemleri için servis
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IPersonelYetkiService _personelYetkiService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            ILogger<AuthService> logger,
            IPersonelYetkiService personelYetkiService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _personelYetkiService = personelYetkiService;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // 🆕 User tablosundan kullanıcıyı bul (Personel ile birlikte - opsiyonel)
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);

                if (user == null)
                {
                    _logger.LogWarning("Login başarısız: Kullanıcı bulunamadı - {TcKimlikNo}", request.TcKimlikNo);

                    // Başarısız login kaydı
                    await CreateLoginLogAsync(request.TcKimlikNo, null, null,
                        request.IpAddress, request.UserAgent, false, "Kullanıcı bulunamadı");

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

                    // Başarısız login kaydı
                    await CreateLoginLogAsync(user.TcKimlikNo, user.Personel?.AdSoyad, null,
                        request.IpAddress, request.UserAgent, false, "Hesap pasif");

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

                    // Başarısız login kaydı
                    await CreateLoginLogAsync(user.TcKimlikNo, user.Personel?.AdSoyad, null,
                        request.IpAddress, request.UserAgent, false, "Hesap kilitli");

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
                        await _unitOfWork.SaveChangesAsync();

                        _logger.LogWarning("Hesap kilitlendi (5 başarısız deneme) - {TcKimlikNo}", request.TcKimlikNo);

                        // Başarısız login kaydı
                        await CreateLoginLogAsync(user.TcKimlikNo, user.Personel?.AdSoyad, null,
                            request.IpAddress, request.UserAgent, false, "5 başarısız deneme - hesap kilitlendi");

                        return new LoginResponseDto
                        {
                            Success = false,
                            Message = "5 başarısız giriş denemesi nedeniyle hesabınız kilitlenmiştir!"
                        };
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogWarning("Login başarısız: Hatalı şifre ({Deneme}/5) - {TcKimlikNo}",
                        user.BasarisizGirisSayisi, request.TcKimlikNo);

                    // Başarısız login kaydı
                    await CreateLoginLogAsync(user.TcKimlikNo, user.Personel?.AdSoyad, null,
                        request.IpAddress, request.UserAgent, false, $"Hatalı şifre ({user.BasarisizGirisSayisi}/5)");

                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = $"TC Kimlik No veya şifre hatalı! ({user.BasarisizGirisSayisi}/5 deneme)"
                    };
                }

                // Başarılı giriş - Session ID oluştur
                var oldSessionId = user.SessionID; // Eski session ID'yi sakla
                var sessionId = Guid.NewGuid().ToString();
                user.SessionID = sessionId;
                user.SonGirisTarihi = DateTime.Now;
                user.BasarisizGirisSayisi = 0; // Başarılı girişte sıfırla

                // ⚠️ Yeni login sonrası orphan banko mode flag'ini temizle
                // (Önceki oturumdan kalmış olabilir - HubBankoConnection kaydı yok ama User flag'i aktif)
                if (user.BankoModuAktif)
                {
                    _logger.LogWarning("⚠️ Login sırasında orphan banko mode flag tespit edildi: {TcKimlikNo}", user.TcKimlikNo);
                    user.BankoModuAktif = false;
                    user.AktifBankoId = null;
                    _logger.LogInformation("✅ Orphan banko mode flag temizlendi: {TcKimlikNo}", user.TcKimlikNo);
                }

                await _unitOfWork.SaveChangesAsync();

                // 🔥 Eski oturum varsa loglayalım (farklı cihazdan login uyarısı için)
                if (!string.IsNullOrEmpty(oldSessionId) && oldSessionId != sessionId)
                {
                    _logger.LogWarning("⚠️ Kullanıcı farklı bir cihazdan/tarayıcıdan giriş yaptı - TcKimlikNo: {TcKimlikNo}, Eski SessionID: {OldSessionId}, Yeni SessionID: {NewSessionId}",
                        user.TcKimlikNo, oldSessionId, sessionId);
                }

                // 🔥 TV login mi yoksa Personel login mi?
                if (user.Personel == null)
                {
                    // TV Login
                    _logger.LogInformation("TV Login başarılı - {TcKimlikNo}", user.TcKimlikNo);

                    // ✅ Başarılı login kaydı
                    await CreateLoginLogAsync(user.TcKimlikNo, "TV Kullanıcısı", sessionId,
                        request.IpAddress, request.UserAgent, true, null);

                    return new LoginResponseDto
                    {
                        Success = true,
                        Message = "TV girişi başarılı!",
                        TcKimlikNo = user.TcKimlikNo,
                        AdSoyad = "TV Kullanıcısı",
                        SessionId = sessionId,
                        UserType = "TvUser", // 🎯 TV kullanıcısı
                        RedirectUrl = "/tv/display" // TV ekranına yönlendir
                    };
                }
                else
                {
                    // Personel Login
                    _logger.LogInformation("Login başarılı - {TcKimlikNo} - {AdSoyad}",
                        user.TcKimlikNo, user.Personel.AdSoyad);

                    // ✅ Başarılı login kaydı
                    await CreateLoginLogAsync(user.TcKimlikNo, user.Personel.AdSoyad, sessionId,
                        request.IpAddress, request.UserAgent, true, null);

                    // 🔑 Yetkileri çek (atanmış + MinYetkiSeviyesi >= None olan varsayılanlar)
                    var permissions = await _personelYetkiService.GetUserPermissionsWithDefaultsAsync(user.TcKimlikNo);

                    _logger.LogDebug("🔑 Login: {Count} yetki yüklendi (atanmış + varsayılan) - {TcKimlikNo}", permissions.Count, user.TcKimlikNo);

                    return new LoginResponseDto
                    {
                        Success = true,
                        Message = "Giriş başarılı!",
                        TcKimlikNo = user.Personel.TcKimlikNo,
                        SicilNo = user.Personel.SicilNo,
                        AdSoyad = user.Personel.AdSoyad,
                        Email = user.Personel.Email,
                        DepartmanId = user.Personel.DepartmanId,
                        DepartmanAdi = user.Personel.Departman?.DepartmanAdi ?? "",
                        ServisId = user.Personel.ServisId,
                        ServisAdi = user.Personel.Servis?.ServisAdi ?? "",
                        HizmetBinasiId = user.Personel.HizmetBinasiId,
                        HizmetBinasiAdi = user.Personel.HizmetBinasi?.HizmetBinasiAdi ?? "",
                        Resim = user.Personel.Resim,
                        SessionId = sessionId,
                        UserType = "Personel", // 🎯 Personel kullanıcısı
                        RedirectUrl = "/", // Ana dashboard'a yönlendir
                        Permissions = permissions // 🔑 Yetkiler
                    };
                }
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
                var personelRepo = _unitOfWork.Repository<Personel>();
                var allPersonel = await personelRepo.GetAllAsync();
                var personel = allPersonel.FirstOrDefault(p =>
                    p.TcKimlikNo == request.TcKimlikNo &&
                    p.SicilNo == request.SicilNo &&
                    p.DogumTarihi.Date == request.DogumTarihi.Date &&
                    p.Email != null && p.Email.ToLower() == request.Email.ToLower());

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
                var userRepo = _unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(request.TcKimlikNo);

                if (user == null)
                {
                    _logger.LogWarning("Şifre sıfırlama başarısız: Kullanıcı bulunamadı - {TcKimlikNo}", request.TcKimlikNo);
                    return false;
                }

                // Yeni şifreyi düz metin olarak kaydet
                user.PassWord = request.NewPassword;
                await _unitOfWork.SaveChangesAsync();

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

        /// <summary>
        /// UserAgent string'inden browser bilgisini çıkarır
        /// </summary>
        private string ParseBrowser(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";

            if (userAgent.Contains("Edg/")) return "Edge";
            if (userAgent.Contains("Chrome/")) return "Chrome";
            if (userAgent.Contains("Firefox/")) return "Firefox";
            if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome")) return "Safari";
            if (userAgent.Contains("Opera/") || userAgent.Contains("OPR/")) return "Opera";

            return "Other";
        }

        /// <summary>
        /// UserAgent string'inden işletim sistemi bilgisini çıkarır
        /// </summary>
        private string ParseOperatingSystem(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";

            if (userAgent.Contains("Windows NT 10.0")) return "Windows 10/11";
            if (userAgent.Contains("Windows NT 6.3")) return "Windows 8.1";
            if (userAgent.Contains("Windows NT 6.2")) return "Windows 8";
            if (userAgent.Contains("Windows NT 6.1")) return "Windows 7";
            if (userAgent.Contains("Mac OS X")) return "macOS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) return "iOS";

            return "Other";
        }

        /// <summary>
        /// UserAgent string'inden cihaz tipi bilgisini çıkarır
        /// </summary>
        private string ParseDeviceType(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";

            if (userAgent.Contains("Mobile") || userAgent.Contains("Android")) return "Mobile";
            if (userAgent.Contains("Tablet") || userAgent.Contains("iPad")) return "Tablet";

            return "Desktop";
        }

        /// <summary>
        /// Login/Logout log kaydı oluşturur
        /// </summary>
        private async Task CreateLoginLogAsync(string? tcKimlikNo, string? adSoyad, string? sessionId,
            string? ipAddress, string? userAgent, bool loginSuccessful, string? failureReason = null)
        {
            try
            {
                var loginLog = new LoginLogoutLog
                {
                    TcKimlikNo = tcKimlikNo,
                    AdSoyad = adSoyad,
                    LoginTime = DateTime.Now,
                    SessionID = sessionId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Browser = ParseBrowser(userAgent),
                    OperatingSystem = ParseOperatingSystem(userAgent),
                    DeviceType = ParseDeviceType(userAgent),
                    LoginSuccessful = loginSuccessful,
                    FailureReason = failureReason
                };

                var loginLogRepo = _unitOfWork.Repository<LoginLogoutLog>();
                await loginLogRepo.AddAsync(loginLog);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("LoginLogoutLog kaydı oluşturuldu - TC: {TcKimlikNo}, Success: {Success}",
                    tcKimlikNo, loginSuccessful);
            }
            catch (Exception ex)
            {
                // Log kaydı hata verirse ana işlemi etkilemesin
                _logger.LogError(ex, "LoginLogoutLog kaydı oluşturulurken hata - TC: {TcKimlikNo}", tcKimlikNo);
            }
        }
    }
}

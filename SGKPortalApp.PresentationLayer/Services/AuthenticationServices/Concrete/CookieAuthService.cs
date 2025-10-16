using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Interfaces;
using System.Security.Claims;

namespace SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Concrete
{
    /// <summary>
    /// Cookie-based Authentication işlemleri için servis
    /// </summary>
    public class CookieAuthService : ICookieAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CookieAuthService> _logger;

        public CookieAuthService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CookieAuthService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task SignInAsync(LoginResponseDto loginResponse, bool rememberMe = true)
        {
            if (loginResponse == null || !loginResponse.Success)
            {
                throw new ArgumentException("Geçersiz login response", nameof(loginResponse));
            }

            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext bulunamadı");

            // Claims oluştur
            var claims = new List<Claim>
            {
                new Claim("TcKimlikNo", loginResponse.TcKimlikNo),
                new Claim("SicilNo", loginResponse.SicilNo.ToString()),
                new Claim("AdSoyad", loginResponse.AdSoyad),
                new Claim(ClaimTypes.Name, loginResponse.AdSoyad),
                new Claim(ClaimTypes.Email, loginResponse.Email),
                new Claim("DepartmanId", loginResponse.DepartmanId.ToString()),
                new Claim("DepartmanAdi", loginResponse.DepartmanAdi),
                new Claim("ServisId", loginResponse.ServisId.ToString()),
                new Claim("ServisAdi", loginResponse.ServisAdi),
                new Claim("HizmetBinasiId", loginResponse.HizmetBinasiId.ToString()),
                new Claim("HizmetBinasiAdi", loginResponse.HizmetBinasiAdi),
                new Claim("SessionID", loginResponse.SessionId)
            };

            if (!string.IsNullOrEmpty(loginResponse.Resim))
            {
                claims.Add(new Claim("Resim", loginResponse.Resim));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Kullanıcı giriş yaptı: {AdSoyad} ({TcKimlikNo})", 
                loginResponse.AdSoyad, loginResponse.TcKimlikNo);
        }

        public async Task SignOutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HttpContext bulunamadı");

            var userName = httpContext.User?.FindFirst("AdSoyad")?.Value ?? "Bilinmeyen";

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Kullanıcı çıkış yaptı: {UserName}", userName);
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated ?? false;
            return Task.FromResult(isAuthenticated);
        }
    }
}

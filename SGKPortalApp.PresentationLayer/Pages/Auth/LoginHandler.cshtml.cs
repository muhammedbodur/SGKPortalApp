using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginHandlerModel : PageModel
    {
        private readonly ILogger<LoginHandlerModel> _logger;
        private readonly IBankoModeService _bankoModeService;

        public LoginHandlerModel(
            ILogger<LoginHandlerModel> logger,
            IBankoModeService bankoModeService)
        {
            _logger = logger;
            _bankoModeService = bankoModeService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogDebug("🔵 LoginHandler.OnPostAsync başladı");

                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                _logger.LogDebug($"🔵 Request Body uzunluğu: {body.Length}");

                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogError("❌ Request body boş!");
                    return BadRequest("Request body boş");
                }

                var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse == null || !loginResponse.Success)
                {
                    _logger.LogError("❌ Login response null veya başarısız!");
                    return BadRequest("Geçersiz login response");
                }

                _logger.LogDebug($"🔵 Login response alındı: {loginResponse.AdSoyad}");

                var claims = new List<Claim>
                {
                    new Claim("TcKimlikNo", loginResponse.TcKimlikNo),
                    new Claim("AdSoyad", loginResponse.AdSoyad),
                    new Claim(ClaimTypes.Name, loginResponse.AdSoyad),
                    new Claim("SessionID", loginResponse.SessionId),
                    new Claim("UserType", loginResponse.UserType ?? "Personel") // TV veya Personel
                };

                // Personel için ek claim'ler
                if (loginResponse.UserType != "TvUser")
                {
                    claims.Add(new Claim("SicilNo", loginResponse.SicilNo.ToString()));
                    claims.Add(new Claim(ClaimTypes.Email, loginResponse.Email ?? ""));
                    claims.Add(new Claim("DepartmanId", loginResponse.DepartmanId.ToString()));
                    claims.Add(new Claim("DepartmanAdi", loginResponse.DepartmanAdi ?? ""));
                    claims.Add(new Claim("ServisId", loginResponse.ServisId.ToString()));
                    claims.Add(new Claim("ServisAdi", loginResponse.ServisAdi ?? ""));
                    claims.Add(new Claim("HizmetBinasiId", loginResponse.HizmetBinasiId.ToString()));
                    claims.Add(new Claim("HizmetBinasiAdi", loginResponse.HizmetBinasiAdi ?? ""));
                    
                    if (!string.IsNullOrEmpty(loginResponse.Resim))
                    {
                        claims.Add(new Claim("Resim", loginResponse.Resim));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("✅ Kullanıcı giriş yaptı: {AdSoyad} ({TcKimlikNo}) - {UserType}",
                    loginResponse.AdSoyad, loginResponse.TcKimlikNo, loginResponse.UserType);

                // ⭐ Banko modu kontrolü - Personel ise
                if (loginResponse.UserType != "TvUser")
                {
                    var isBankoMode = await _bankoModeService.IsPersonelInBankoModeAsync(loginResponse.TcKimlikNo);
                    if (isBankoMode)
                    {
                        // Login olurken banko modundan çık (eski bağlantıları temizle)
                        await _bankoModeService.ExitBankoModeAsync(loginResponse.TcKimlikNo);
                        _logger.LogInformation("🏦 Login sırasında banko modundan çıkıldı: {TcKimlikNo}", loginResponse.TcKimlikNo);
                    }
                }

                // Ana sayfaya yönlendir
                _logger.LogDebug("🔵 Ana sayfaya yönlendiriliyor");
                return Redirect("/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ LoginHandler hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
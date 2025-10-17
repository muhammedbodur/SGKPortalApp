using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using System.Security.Claims;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class LoginHandlerModel : PageModel
    {
        private readonly ILogger<LoginHandlerModel> _logger;

        public LoginHandlerModel(ILogger<LoginHandlerModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogDebug("🔵 LoginHandler.OnPostAsync başladı");

                // Request body'den LoginResponseDto'yu al
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
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("✅ Kullanıcı giriş yaptı: {AdSoyad} ({TcKimlikNo})",
                    loginResponse.AdSoyad, loginResponse.TcKimlikNo);

                // Başarılı yanıt dön
                return new JsonResult(new { success = true, message = "Login başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ LoginHandler hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

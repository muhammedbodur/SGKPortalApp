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

        public async Task<IActionResult> OnPostAsync([FromBody] LoginResponseDto loginResponse)
        {
            try
            {
                _logger.LogInformation("LoginHandler: POST isteği alındı");

                if (loginResponse == null)
                {
                    _logger.LogError("LoginHandler: loginResponse null");
                    return BadRequest(new { message = "Login response null" });
                }

                if (!loginResponse.Success)
                {
                    _logger.LogError("LoginHandler: Login başarısız");
                    return BadRequest(new { message = "Login başarısız" });
                }

                _logger.LogInformation($"LoginHandler: {loginResponse.AdSoyad} için cookie oluşturuluyor...");

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

                _logger.LogInformation($"LoginHandler: Cookie başarıyla oluşturuldu - {loginResponse.AdSoyad}");
                return new JsonResult(new { success = true, message = "Giriş başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginHandler: Hata oluştu");
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}

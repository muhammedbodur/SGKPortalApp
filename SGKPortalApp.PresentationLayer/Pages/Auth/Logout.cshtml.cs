using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userName = User?.FindFirst("AdSoyad")?.Value ?? "Bilinmeyen Kullanıcı";
                
                _logger.LogInformation($"Logout: {userName} çıkış yapıyor...");

                // Cookie'yi sil
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation($"Logout: {userName} başarıyla çıkış yaptı");

                // Login sayfasına yönlendir (Blazor route kullan)
                return Redirect("/auth/login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout: Hata oluştu");
                return Redirect("/auth/login");
            }
        }
    }
}

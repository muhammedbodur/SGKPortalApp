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
                var tcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
                
                _logger.LogInformation("🔄 Logout: {UserName} çıkış yapıyor...", userName);

                // Authentication Cookie'yi sil
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Session'ı temizle (varsa)
                try
                {
                    HttpContext.Session?.Clear();
                }
                catch (Exception sessionEx)
                {
                    _logger.LogWarning(sessionEx, "⚠️ Session temizlenirken hata");
                }

                _logger.LogInformation("✅ Logout: {UserName} başarıyla çıkış yaptı", userName);

                // Blazor Circuit'i tamamen kapat (SPA cache'i temizle)
                Response.Headers["Clear-Site-Data"] = "\"cache\", \"cookies\", \"storage\"";
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";

                return Redirect("/auth/login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout: Hata oluştu");
                return Redirect("/auth/login");
            }
        }
    }
}

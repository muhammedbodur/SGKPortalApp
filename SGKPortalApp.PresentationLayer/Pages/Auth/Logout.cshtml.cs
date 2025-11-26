using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IBankoModeService _bankoModeService;

        public LogoutModel(
            ILogger<LogoutModel> logger,
            IBankoModeService bankoModeService)
        {
            _logger = logger;
            _bankoModeService = bankoModeService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userName = User?.FindFirst("AdSoyad")?.Value ?? "Bilinmeyen Kullanıcı";
                var tcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
                
                _logger.LogInformation($"Logout: {userName} çıkış yapıyor...");

                // Banko modundan çık (eğer aktifse)
                if (!string.IsNullOrEmpty(tcKimlikNo))
                {
                    try
                    {
                        var isBankoMode = await _bankoModeService.IsPersonelInBankoModeAsync(tcKimlikNo);
                        if (isBankoMode)
                        {
                            await _bankoModeService.ExitBankoModeAsync(tcKimlikNo);
                            _logger.LogInformation($"Logout: {userName} banko modundan çıkarıldı");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Logout: Banko modundan çıkış hatası");
                    }
                }

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

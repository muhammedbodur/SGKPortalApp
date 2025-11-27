using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.PresentationLayer.Services.Hubs;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IBankoModeService _bankoModeService;
        private readonly IHubContext<SiramatikHub> _hubContext;

        public LogoutModel(
            ILogger<LogoutModel> logger,
            IBankoModeService bankoModeService,
            IHubContext<SiramatikHub> hubContext)
        {
            _logger = logger;
            _bankoModeService = bankoModeService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userName = User?.FindFirst("AdSoyad")?.Value ?? "Bilinmeyen Kullanıcı";
                var tcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
                
                _logger.LogInformation($"🔄 Logout: {userName} çıkış yapıyor...");

                // Server-side temizlik işlemleri
                if (!string.IsNullOrEmpty(tcKimlikNo))
                {
                    try
                    {
                        // 1. Banko modundan çık (eğer aktifse)
                        var isBankoMode = await _bankoModeService.IsPersonelInBankoModeAsync(tcKimlikNo);
                        if (isBankoMode)
                        {
                            await _bankoModeService.ExitBankoModeAsync(tcKimlikNo);
                            _logger.LogInformation($"✅ Logout: {userName} banko modundan çıkarıldı");
                        }

                        // 2. SignalR bağlantılarını temizle (OnDisconnectedAsync otomatik çağrılacak)
                        // HubConnection'lar SignalR tarafından otomatik temizlenecek
                        
                        _logger.LogInformation($"✅ Logout: {userName} için server-side temizlik tamamlandı");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Logout: Server-side temizlik hatası");
                    }
                }

                // 3. Authentication Cookie'yi sil
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. Session'ı temizle (varsa)
                try
                {
                    HttpContext.Session?.Clear();
                }
                catch (Exception sessionEx)
                {
                    _logger.LogWarning(sessionEx, "⚠️ Session temizlenirken hata (Session yapılandırılmamış olabilir)");
                }

                _logger.LogInformation($"✅ Logout: {userName} başarıyla çıkış yaptı");

                // 5. ⭐ TÜM TAB'LERE SignalR ile logout bildirimi gönder
                if (!string.IsNullOrEmpty(tcKimlikNo))
                {
                    await _hubContext.Clients.User(tcKimlikNo).SendAsync("ForceLogout", "Oturum sonlandırıldı. Lütfen tekrar giriş yapın.");
                    _logger.LogInformation($"📢 Logout: {userName} için tüm tab'lere bildirim gönderildi");
                }

                // 6. ⭐ Blazor Circuit'i tamamen kapat (SPA cache'i temizle)
                Response.Headers.Add("Clear-Site-Data", "\"cache\", \"cookies\", \"storage\"");
                
                // 7. ⭐ Tarayıcıyı zorla yenile (Circuit'i kapat)
                Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
                Response.Headers.Add("Pragma", "no-cache");

                // Login sayfasına yönlendir
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

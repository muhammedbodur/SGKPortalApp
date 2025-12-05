using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LogoutModel(
            ILogger<LogoutModel> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userName = User?.FindFirst("AdSoyad")?.Value ?? "Bilinmeyen Kullanıcı";
                var tcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;

                _logger.LogInformation("🔄 Logout: {UserName} çıkış yapıyor...", userName);

                // ⚠️ API'ye logout çağrısı yap (banko modundan çıkış için)
                if (!string.IsNullOrEmpty(tcKimlikNo))
                {
                    try
                    {
                        var httpClient = _httpClientFactory.CreateClient();
                        var apiUrl = _configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
                        var logoutRequest = new { TcKimlikNo = tcKimlikNo };
                        var content = new StringContent(
                            JsonSerializer.Serialize(logoutRequest),
                            Encoding.UTF8,
                            "application/json");

                        var response = await httpClient.PostAsync($"{apiUrl}/api/auth/logout", content);
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("✅ API Logout başarılı - {TcKimlikNo}", tcKimlikNo);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ API Logout başarısız - {TcKimlikNo}, Status: {StatusCode}",
                                tcKimlikNo, response.StatusCode);
                        }
                    }
                    catch (Exception apiEx)
                    {
                        _logger.LogWarning(apiEx, "⚠️ API Logout çağrısı başarısız - {TcKimlikNo}", tcKimlikNo);
                    }
                }

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

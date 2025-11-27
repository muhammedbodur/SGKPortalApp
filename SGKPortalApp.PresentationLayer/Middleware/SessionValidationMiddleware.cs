using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.Hubs;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Middleware
{
    /// <summary>
    /// Her request'te kullanıcının session ID'sini kontrol eder
    /// Eğer başka bir cihazdan login olunmuşsa (SessionID değişmişse) logout yapar
    /// VE SignalR ile ForceLogout event'i gönderir!
    /// </summary>
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserInfoService userInfoService,
            IUserApiService userApiService,
            IHubContext<SiramatikHub> hubContext) // ✅ SignalR Hub inject edildi
        {
            // Session expired sayfasını ve login sayfasını kontrol dışında tut
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/auth/session-expired") ||
                path.Contains("/auth/login") ||
                path.Contains("/auth/logout") ||
                path.Contains("/_blazor")) // ✅ Blazor SignalR endpoint'lerini de atla
            {
                await _next(context);
                return;
            }

            // Kullanıcı login mi kontrol et
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var currentSessionId = userInfoService.GetSessionId();
                    var tcKimlikNo = userInfoService.GetTcKimlikNo();

                    _logger.LogDebug("🔍 Session kontrolü - TcKimlikNo: {TcKimlikNo}, Cookie SessionID: {SessionId}",
                        tcKimlikNo, currentSessionId);

                    if (!string.IsNullOrEmpty(currentSessionId) && !string.IsNullOrEmpty(tcKimlikNo))
                    {
                        // Database'den kullanıcının aktif session ID'sini al
                        var userResult = await userApiService.GetByTcKimlikNoAsync(tcKimlikNo);

                        if (userResult.Success && userResult.Data != null)
                        {
                            var dbSessionId = userResult.Data.SessionID;

                            // Session ID'ler farklıysa başka bir cihazdan login olunmuş demektir
                            if (!string.IsNullOrEmpty(dbSessionId) && dbSessionId != currentSessionId)
                            {
                                _logger.LogWarning("⚠️ Session uyuşmazlığı tespit edildi! " +
                                    "TcKimlikNo: {TcKimlikNo}, Cookie SessionID: {CookieSessionId}, DB SessionID: {DbSessionId}",
                                    tcKimlikNo, currentSessionId, dbSessionId);

                                // ✅ 1. SignalR ile ForceLogout event'i gönder
                                try
                                {
                                    var adSoyad = context.User.FindFirst("AdSoyad")?.Value ?? "Kullanıcı";
                                    var message = $"{adSoyad}, hesabınıza başka bir cihazdan giriş yapıldı. Oturumunuz sonlandırılıyor.";

                                    // TcKimlikNo'ya özel grup varsa oraya gönder
                                    await hubContext.Clients.Group($"USER_{tcKimlikNo}")
                                        .SendAsync("ForceLogout", message);

                                    _logger.LogInformation("📡 ForceLogout SignalR event'i gönderildi: {TcKimlikNo}", tcKimlikNo);
                                }
                                catch (Exception signalREx)
                                {
                                    _logger.LogError(signalREx, "❌ SignalR ForceLogout gönderimi hatası");
                                    // SignalR hatası olsa bile devam et
                                }

                                // ✅ 2. HTTP response logout
                                await context.SignOutAsync();
                                context.Response.Redirect("/auth/login?sessionExpired=true");
                                return;
                            }
                            else
                            {
                                _logger.LogDebug("✅ Session ID eşleşti - Kullanıcı geçerli");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Session validation sırasında hata oluştu");
                    // Hata durumunda devam et, kullanıcıyı logout etme
                }
            }

            await _next(context);
        }
    }
}
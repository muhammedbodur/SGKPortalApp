using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Middleware
{
    /// <summary>
    /// Her request'te kullanıcının session ID'sini kontrol eder
    /// Eğer başka bir cihazdan login olunmuşsa (SessionID değişmişse) logout yapar
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

        public async Task InvokeAsync(HttpContext context, IUserInfoService userInfoService, IUserApiService userApiService)
        {
            // Session expired sayfasını ve login sayfasını kontrol dışında tut
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/auth/session-expired") || 
                path.Contains("/auth/login") || 
                path.Contains("/auth/logout"))
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
                    
                    _logger.LogInformation("🔍 Session kontrolü yapılıyor - TcKimlikNo: {TcKimlikNo}, Cookie SessionID: {SessionId}", 
                        tcKimlikNo, currentSessionId);

                    if (!string.IsNullOrEmpty(currentSessionId) && !string.IsNullOrEmpty(tcKimlikNo))
                    {
                        // Database'den kullanıcının aktif session ID'sini al
                        var userResult = await userApiService.GetByTcKimlikNoAsync(tcKimlikNo);

                        if (userResult.Success && userResult.Data != null)
                        {
                            var dbSessionId = userResult.Data.SessionID;
                            
                            _logger.LogInformation("✅ DB SessionID: {DbSessionId}", dbSessionId);

                            // Session ID'ler farklıysa başka bir cihazdan login olunmuş demektir
                            if (!string.IsNullOrEmpty(dbSessionId) && dbSessionId != currentSessionId)
                            {
                                _logger.LogWarning("⚠️ Session ID uyuşmazlığı tespit edildi! TcKimlikNo: {TcKimlikNo}, Cookie SessionID: {CookieSessionId}, DB SessionID: {DbSessionId}",
                                    tcKimlikNo, currentSessionId, dbSessionId);

                                // Kullanıcıyı logout et ve login sayfasına yönlendir
                                await context.SignOutAsync();
                                context.Response.Redirect("/auth/login?sessionExpired=true");
                                return;
                            }
                            else
                            {
                                _logger.LogInformation("✅ Session ID eşleşti - Kullanıcı geçerli");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Session validation sırasında hata oluştu");
                    // Hata durumunda devam et, kullanıcıyı logout etme
                }
            }

            await _next(context);
        }
    }
}

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

        // ⚠️ Login sonrası grace period için cache
        // Key: TcKimlikNo, Value: Login time
        private static readonly Dictionary<string, DateTime> _recentLogins = new();
        private static readonly TimeSpan _gracePeriod = TimeSpan.FromSeconds(3); // 3 saniye grace period

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Login sonrası grace period'u başlat
        /// LoginHandler tarafından çağrılır
        /// </summary>
        public static void StartGracePeriod(string tcKimlikNo)
        {
            lock (_recentLogins)
            {
                _recentLogins[tcKimlikNo] = DateTime.UtcNow;

                // Eski kayıtları temizle (10 dakikadan eski)
                var oldKeys = _recentLogins
                    .Where(x => (DateTime.UtcNow - x.Value).TotalMinutes > 10)
                    .Select(x => x.Key)
                    .ToList();

                foreach (var key in oldKeys)
                {
                    _recentLogins.Remove(key);
                }
            }
        }

        /// <summary>
        /// Kullanıcı için grace period aktif mi kontrol et
        /// </summary>
        private static bool IsInGracePeriod(string tcKimlikNo)
        {
            lock (_recentLogins)
            {
                if (_recentLogins.TryGetValue(tcKimlikNo, out var loginTime))
                {
                    var elapsed = DateTime.UtcNow - loginTime;
                    if (elapsed < _gracePeriod)
                    {
                        return true;
                    }
                    else
                    {
                        // Grace period bitti, cache'den kaldır
                        _recentLogins.Remove(tcKimlikNo);
                    }
                }
            }
            return false;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserInfoService userInfoService,
            IUserApiService userApiService)
        {
            // Session expired sayfasını ve login sayfasını kontrol dışında tut
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/auth/session-expired") ||
                path.Contains("/auth/login") ||
                path.Contains("/auth/loginhandler") || // ⚠️ ADDED: Login sonrası cookie set ediliyor, session kontrolü atla
                path.Contains("/auth/logout") ||
                path.Contains("/_blazor"))
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
                        // ⚠️ Grace period kontrolü: Login'den sonra 3 saniye içinde session kontrolü yapma
                        if (IsInGracePeriod(tcKimlikNo))
                        {
                            _logger.LogDebug("ℹ️ Grace period aktif, session kontrolü atlandı - TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                            await _next(context);
                            return;
                        }

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

                                // HTTP response logout
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
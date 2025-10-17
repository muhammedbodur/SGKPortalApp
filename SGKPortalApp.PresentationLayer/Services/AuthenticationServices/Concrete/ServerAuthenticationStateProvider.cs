using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SGKPortalApp.PresentationLayer.Services.AuthenticationServices.Concrete
{
    /// <summary>
    /// Blazor Server için Authentication State Provider
    /// Cookie'deki kullanıcı bilgilerini Blazor'a söyler
    /// </summary>
    public class ServerAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServerAuthenticationStateProvider> _logger;

        public ServerAuthenticationStateProvider(
            IHttpContextAccessor httpContextAccessor,
            ILogger<ServerAuthenticationStateProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            _logger.LogDebug("✅ ServerAuthenticationStateProvider oluşturuldu");
        }

        /// <summary>
        /// Kullanıcının authentication durumunu döndürür
        /// Her sayfa yüklendiğinde çağrılır
        /// </summary>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                _logger.LogDebug("🔵 GetAuthenticationStateAsync ÇAĞRILDI!");

                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext?.User?.Identity != null)
                {
                    var user = httpContext.User;

                    if (user.Identity.IsAuthenticated)
                    {
                        var userName = user.FindFirst("AdSoyad")?.Value ?? "Unknown";
                        _logger.LogDebug("✅ Authenticated user: {UserName}", userName);
                    }
                    else
                    {
                        _logger.LogDebug("ℹ️ Anonymous user");
                    }

                    return Task.FromResult(new AuthenticationState(user));
                }

                // HttpContext yoksa anonymous user
                _logger.LogDebug("⚠️ HttpContext veya User null - anonymous user döndürülüyor");
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymousUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetAuthenticationStateAsync error");

                // Hata durumunda anonymous user
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymousUser));
            }
        }

        /// <summary>
        /// Authentication state değiştiğinde dışarıdan çağrılır (örn: login/logout)
        /// </summary>
        public void NotifyAuthenticationStateChanged()
        {
            _logger.LogDebug("🔄 Authentication state değişti, Blazor'a bildirim gönderiliyor");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}

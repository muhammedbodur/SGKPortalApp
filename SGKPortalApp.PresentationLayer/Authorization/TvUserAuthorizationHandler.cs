using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace SGKPortalApp.PresentationLayer.Authorization
{
    /// <summary>
    /// TV User'ların sadece kendi Display sayfalarına erişmesini sağlar
    /// Diğer tüm sayfalara erişimi engeller
    /// </summary>
    public class TvUserAuthorizationHandler : AuthorizationHandler<TvUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TvUserAuthorizationHandler> _logger;

        public TvUserAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<TvUserAuthorizationHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TvUserRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var user = context.User;
            var userType = user.FindFirst("UserType")?.Value;

            // Personel ise izin ver
            if (userType == "Personel")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // TV User ise sadece kendi Display sayfasına izin ver
            if (userType == "TvUser")
            {
                var path = httpContext.Request.Path.Value?.ToLower() ?? "";
                var tcKimlikNo = user.FindFirst("TcKimlikNo")?.Value;

                // TcKimlikNo formatı: TV0000001
                if (tcKimlikNo?.StartsWith("TV") == true)
                {
                    var tvIdFromUser = int.Parse(tcKimlikNo.Substring(2)); // "TV0000001" -> 1
                    var allowedPath = $"/siramatik/tv/display/{tvIdFromUser}";

                    if (path == allowedPath.ToLower() || 
                        path.StartsWith("/auth/") || 
                        path.StartsWith("/_blazor") ||
                        path.StartsWith("/css/") ||
                        path.StartsWith("/js/") ||
                        path.StartsWith("/lib/") ||
                        path.StartsWith("/logo/") ||
                        path.StartsWith("/sounds/") ||
                        path.StartsWith("/video/"))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }

                    _logger.LogWarning($"❌ TV User yetkisiz erişim engellendi: {tcKimlikNo} -> {path}");
                }

                context.Fail();
                return Task.CompletedTask;
            }

            // Bilinmeyen kullanıcı tipi
            context.Fail();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// TV User authorization requirement
    /// </summary>
    public class TvUserRequirement : IAuthorizationRequirement
    {
    }
}

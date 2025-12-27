using Microsoft.AspNetCore.Http;
using SGKPortalApp.Common.Interfaces;
using System.Security.Claims;

namespace SGKPortalApp.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetTcKimlikNo()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return "ANONYMOUS";

            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("TcKimlikNo")?.Value
                ?? "ANONYMOUS";
        }

        public string? GetIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // X-Forwarded-For header'ından al (proxy/load balancer arkasındaysa)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // İlk IP'yi al (client IP'si)
                return forwardedFor.Split(',')[0].Trim();
            }

            // Direkt bağlantıdan al
            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        public string? GetUserAgent()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            return httpContext.Request.Headers["User-Agent"].ToString();
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public int? GetDepartmanId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var departmanIdClaim = user.FindFirst("DepartmanId")?.Value;
            if (int.TryParse(departmanIdClaim, out int departmanId))
                return departmanId;

            return null;
        }

        public int? GetServisId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            var servisIdClaim = user.FindFirst("ServisId")?.Value;
            if (int.TryParse(servisIdClaim, out int servisId))
                return servisId;

            return null;
        }
    }
}

using Microsoft.AspNetCore.Http;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
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
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return "SYSTEM"; // Background job veya system işlemi

            // Session'dan al (login sırasında session'a kaydediliyor)
            var tcKimlikNo = httpContext.Session.GetString("TcKimlikNo");

            if (!string.IsNullOrEmpty(tcKimlikNo))
                return tcKimlikNo;

            // Claim'den al (JWT kullanılıyorsa)
            var claim = httpContext.User?.FindFirst("TcKimlikNo");
            if (claim != null)
                return claim.Value;

            return "ANONYMOUS";
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
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return false;

            // Session kontrolü
            var tcKimlikNo = httpContext.Session.GetString("TcKimlikNo");
            if (!string.IsNullOrEmpty(tcKimlikNo))
                return true;

            // User.Identity kontrolü
            return httpContext.User?.Identity?.IsAuthenticated ?? false;
        }

        public int? GetDepartmanId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Session'dan al
            var departmanIdStr = httpContext.Session.GetString("DepartmanId");
            if (!string.IsNullOrEmpty(departmanIdStr) && int.TryParse(departmanIdStr, out var departmanId))
                return departmanId;

            // Claim'den al
            var claim = httpContext.User?.FindFirst("DepartmanId");
            if (claim != null && int.TryParse(claim.Value, out var claimDepartmanId))
                return claimDepartmanId;

            return null;
        }

        public int? GetServisId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Session'dan al
            var servisIdStr = httpContext.Session.GetString("ServisId");
            if (!string.IsNullOrEmpty(servisIdStr) && int.TryParse(servisIdStr, out var servisId))
                return servisId;

            // Claim'den al
            var claim = httpContext.User?.FindFirst("ServisId");
            if (claim != null && int.TryParse(claim.Value, out var claimServisId))
                return claimServisId;

            return null;
        }
    }
}

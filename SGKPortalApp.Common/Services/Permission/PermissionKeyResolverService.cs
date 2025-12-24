using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SGKPortalApp.Common.Interfaces.Permission;

namespace SGKPortalApp.Common.Services.Permission
{
    /// <summary>
    /// HTTP isteğinin route bilgisinden permission key'i otomatik çözümleyen servis.
    /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
    ///
    /// Cache PermissionStateService tarafından yüklenir. Bu servis sadece lookup yapar.
    /// </summary>
    public class PermissionKeyResolverService : IPermissionKeyResolverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PermissionKeyResolverService> _logger;

        private const string RoutePermissionMapCacheKey = "PermissionKeyResolverService.RoutePermissionMap";

        public PermissionKeyResolverService(
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            ILogger<PermissionKeyResolverService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Mevcut HTTP request'in route'undan permission key'i çözümler (SYNC - cache lookup).
        /// Backend service'lerde kullanım için.
        /// </summary>
        public string? ResolveFromCurrentRequest()
        {
            try
            {
                var path = _httpContextAccessor.HttpContext?.Request.Path.Value;
                if (string.IsNullOrWhiteSpace(path))
                {
                    _logger.LogDebug("⚠️ HttpContext veya Request.Path boş");
                    return null;
                }

                return ResolveFromRoute(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ResolveFromCurrentRequest hatası");
                return null;
            }
        }

        /// <summary>
        /// Belirtilen route'tan permission key'i çözümler (SYNC - cache lookup).
        /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
        /// </summary>
        public string? ResolveFromRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            try
            {
                // Route'u normalize et (trailing slash kaldır, lowercase)
                var normalizedRoute = route.TrimEnd('/').ToLowerInvariant();

                // Sadece cache'den oku (PermissionStateService cache'i yükler)
                if (!_memoryCache.TryGetValue(RoutePermissionMapCacheKey, out Dictionary<string, string>? routeMap)
                    || routeMap == null || routeMap.Count == 0)
                {
                    _logger.LogDebug("⚠️ Route mapping cache'de yok: {Route}", normalizedRoute);
                    return null;
                }

                return ResolveFromRouteSyncInternal(normalizedRoute, routeMap);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ResolveFromRoute hatası: {Route}", route);
                return null;
            }
        }

        /// <summary>
        /// Route çözümleme mantığı (internal helper).
        /// Frontend'deki GetPermissionKeyByRoute() ile aynı algoritma.
        /// </summary>
        private string? ResolveFromRouteSyncInternal(string normalizedRoute, Dictionary<string, string> routeMap)
        {
            // 1. Direkt eşleşme dene
            if (routeMap.TryGetValue(normalizedRoute, out var permissionKey))
            {
                _logger.LogDebug("✅ Route mapping bulundu (direkt): {Route} → {Key}", normalizedRoute, permissionKey);
                return permissionKey;
            }

            // 2. /index ekleyerek dene
            if (!normalizedRoute.EndsWith("/index"))
            {
                var indexRoute = $"{normalizedRoute}/index";
                if (routeMap.TryGetValue(indexRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route mapping bulundu (/index eklendi): {Route} → {Key}", indexRoute, permissionKey);
                    return permissionKey;
                }
            }

            // 3. /index kaldırarak dene
            if (normalizedRoute.EndsWith("/index"))
            {
                var baseRoute = normalizedRoute.Substring(0, normalizedRoute.Length - 6);
                if (routeMap.TryGetValue(baseRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route mapping bulundu (/index kaldırıldı): {Route} → {Key}", baseRoute, permissionKey);
                    return permissionKey;
                }
            }

            // 4. Dynamic route parametrelerini dene (parent route fallback)
            var segments = normalizedRoute.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = segments.Length - 1; i > 0; i--)
            {
                var parentRoute = "/" + string.Join("/", segments.Take(i));
                if (routeMap.TryGetValue(parentRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route mapping bulundu (parent): {Route} → {Key}", parentRoute, permissionKey);
                    return permissionKey;
                }
            }

            _logger.LogWarning("⚠️ Route için permission key bulunamadı: {Route}", normalizedRoute);
            return null;
        }
    }
}

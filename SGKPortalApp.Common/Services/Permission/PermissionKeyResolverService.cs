using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Interfaces.Common;

namespace SGKPortalApp.Common.Services.Permission
{
    /// <summary>
    /// HTTP isteğinin route bilgisinden permission key'i otomatik çözümleyen servis.
    /// Frontend PermissionStateService ile aynı cache formatını kullanır.
    /// </summary>
    public class PermissionKeyResolverService : IPermissionKeyResolverService
    {
        public const string RoutePermissionMapCacheKey = "PermissionKeyResolverService.RoutePermissionMap";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PermissionKeyResolverService> _logger;

        public PermissionKeyResolverService(
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            ILogger<PermissionKeyResolverService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _logger = logger;
        }

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

        public string? ResolveFromRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            try
            {
                var normalizedRoute = route.TrimEnd('/').ToLowerInvariant();

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

        private string? ResolveFromRouteSyncInternal(string normalizedRoute, Dictionary<string, string> routeMap)
        {
            if (routeMap.TryGetValue(normalizedRoute, out var permissionKey))
            {
                _logger.LogDebug("✅ Route çözümlendi: {Route} → {PermissionKey}", normalizedRoute, permissionKey);
                return permissionKey;
            }

            if (!normalizedRoute.EndsWith("/index"))
            {
                var indexRoute = $"{normalizedRoute}/index";
                if (routeMap.TryGetValue(indexRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route çözümlendi (index): {Route} → {PermissionKey}", indexRoute, permissionKey);
                    return permissionKey;
                }
            }

            if (normalizedRoute.EndsWith("/index"))
            {
                var baseRoute = normalizedRoute[..^6];
                if (routeMap.TryGetValue(baseRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route çözümlendi (base): {Route} → {PermissionKey}", baseRoute, permissionKey);
                    return permissionKey;
                }
            }

            var segments = normalizedRoute.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = segments.Length - 1; i > 0; i--)
            {
                var parentRoute = "/" + string.Join("/", segments.Take(i));
                if (routeMap.TryGetValue(parentRoute, out permissionKey))
                {
                    _logger.LogDebug("✅ Route çözümlendi (parent): {ParentRoute} → {PermissionKey}", parentRoute, permissionKey);
                    return permissionKey;
                }
            }

            _logger.LogWarning("⚠️ Route için permission key bulunamadı: {Route}", normalizedRoute);
            return null;
        }
    }
}

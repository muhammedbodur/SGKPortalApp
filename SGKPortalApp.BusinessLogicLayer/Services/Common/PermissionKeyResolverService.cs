using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// HTTP isteğinin route bilgisinden permission key'i otomatik çözümleyen servis.
    /// Frontend'deki PermissionStateService.GetPermissionKeyByRoute() ile aynı mantık.
    /// </summary>
    public class PermissionKeyResolverService : IPermissionKeyResolverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModulControllerIslemService _modulControllerIslemService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PermissionKeyResolverService> _logger;

        private const string RoutePermissionMapCacheKey = "PermissionKeyResolverService.RoutePermissionMap";
        private static readonly MemoryCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        public PermissionKeyResolverService(
            IHttpContextAccessor httpContextAccessor,
            IModulControllerIslemService modulControllerIslemService,
            IMemoryCache memoryCache,
            ILogger<PermissionKeyResolverService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _modulControllerIslemService = modulControllerIslemService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Mevcut HTTP request'in route'undan permission key'i çözümler.
        /// </summary>
        public async Task<string?> ResolveFromCurrentRequestAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("⚠️ HttpContext bulunamadı, permission key çözümlenemedi");
                    return null;
                }

                var path = httpContext.Request.Path.Value;
                if (string.IsNullOrWhiteSpace(path))
                {
                    _logger.LogWarning("⚠️ Request path boş, permission key çözümlenemedi");
                    return null;
                }

                return await ResolveFromRouteAsync(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ResolveFromCurrentRequestAsync hatası");
                return null;
            }
        }

        /// <summary>
        /// Belirtilen route'tan permission key'i çözümler.
        /// Frontend'deki GetPermissionKeyByRoute() ile aynı mantık.
        /// </summary>
        public async Task<string?> ResolveFromRouteAsync(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            try
            {
                // Route'u normalize et (trailing slash kaldır, lowercase)
                var normalizedRoute = route.TrimEnd('/').ToLowerInvariant();

                // Cache'den route → permission key mapping'i al
                var routeMap = await GetRoutePermissionMapAsync();
                if (routeMap == null || routeMap.Count == 0)
                {
                    _logger.LogWarning("⚠️ Route → Permission Key mapping yüklenemedi");
                    return null;
                }

                // 1. Direkt eşleşme dene
                if (routeMap.TryGetValue(normalizedRoute, out var permissionKey))
                {
                    _logger.LogDebug("✅ Route çözümlendi: {Route} → {PermissionKey}", normalizedRoute, permissionKey);
                    return permissionKey;
                }

                // 2. /index ekleyerek dene (örn: /personel/unvan → /personel/unvan/index)
                if (!normalizedRoute.EndsWith("/index"))
                {
                    var indexRoute = $"{normalizedRoute}/index";
                    if (routeMap.TryGetValue(indexRoute, out permissionKey))
                    {
                        _logger.LogDebug("✅ Route çözümlendi (index): {Route} → {PermissionKey}", indexRoute, permissionKey);
                        return permissionKey;
                    }
                }

                // 3. /index kaldırarak dene (örn: /personel/unvan/index → /personel/unvan)
                if (normalizedRoute.EndsWith("/index"))
                {
                    var baseRoute = normalizedRoute.Substring(0, normalizedRoute.Length - 6); // "/index" = 6 karakter
                    if (routeMap.TryGetValue(baseRoute, out permissionKey))
                    {
                        _logger.LogDebug("✅ Route çözümlendi (base): {Route} → {PermissionKey}", baseRoute, permissionKey);
                        return permissionKey;
                    }
                }

                // 4. Dynamic route parametrelerini dene (örn: /personel/manage/12345678901 → /personel/manage/{tcKimlikNo})
                // Geriye doğru "/" kaldırarak parent route'ları dene
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ResolveFromRouteAsync hatası: {Route}", route);
                return null;
            }
        }

        /// <summary>
        /// Route → PermissionKey mapping'ini cache'den veya DB'den yükler.
        /// Frontend'deki _routeToPermissionKey ile aynı mantık.
        /// </summary>
        private async Task<Dictionary<string, string>?> GetRoutePermissionMapAsync()
        {
            // Cache'den oku
            if (_memoryCache.TryGetValue(RoutePermissionMapCacheKey, out Dictionary<string, string>? cachedMap))
            {
                return cachedMap;
            }

            // DB'den yükle
            try
            {
                _logger.LogInformation("📋 Route → Permission Key mapping yükleniyor...");

                var result = await _modulControllerIslemService.GetAllAsync();
                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("⚠️ ModulControllerIslem verileri yüklenemedi");
                    return null;
                }

                // Route → PermissionKey dictionary oluştur
                var routeMap = result.Data
                    .Where(x => !string.IsNullOrWhiteSpace(x.Route) && !string.IsNullOrWhiteSpace(x.PermissionKey))
                    .GroupBy(x => x.Route.TrimEnd('/').ToLowerInvariant())
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().PermissionKey,
                        StringComparer.OrdinalIgnoreCase);

                // Cache'e kaydet
                _memoryCache.Set(RoutePermissionMapCacheKey, routeMap, CacheOptions);

                _logger.LogInformation("✅ Route mapping yüklendi: {Count} adet", routeMap.Count);
                return routeMap;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Route mapping yüklenirken hata oluştu");
                return null;
            }
        }
    }
}

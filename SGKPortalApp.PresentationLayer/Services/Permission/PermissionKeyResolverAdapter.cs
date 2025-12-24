using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SGKPortalApp.Common.Interfaces.Permission;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Services.Permission
{
    /// <summary>
    /// Presentation katmanında PermissionStateService cache'ini kullanarak
    /// IPermissionKeyResolverService arayüzünü sağlayan adapter.
    /// Backend (BusinessLogicLayer) ile aynı interface'i implement ederek
    /// component'lerin permission key çözümlemesini DI üzerinden yapmasına izin verir.
    /// </summary>
    public class PermissionKeyResolverAdapter : IPermissionKeyResolverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PermissionStateService _permissionStateService;
        private readonly ILogger<PermissionKeyResolverAdapter> _logger;

        public PermissionKeyResolverAdapter(
            IHttpContextAccessor httpContextAccessor,
            PermissionStateService permissionStateService,
            ILogger<PermissionKeyResolverAdapter> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionStateService = permissionStateService;
            _logger = logger;
        }

        public string? ResolveFromCurrentRequest()
        {
            var path = _httpContextAccessor.HttpContext?.Request.Path.Value;
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogDebug("PermissionKeyResolverAdapter.ResolveFromCurrentRequest: HttpContext veya Request.Path boş");
                return null;
            }

            return ResolveFromRoute(path);
        }

        public string? ResolveFromRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            if (!_permissionStateService.RouteMappingsLoaded)
            {
                _logger.LogDebug("PermissionKeyResolverAdapter.ResolveFromRoute: Route mapping henüz yüklenmedi. Route={Route}", route);
                return null;
            }

            var normalizedRoute = route.TrimEnd('/');
            var resolvedKey = _permissionStateService.GetPermissionKeyByRoute(normalizedRoute);

            if (resolvedKey == PermissionStateService.RouteLoadingPlaceholderKey)
            {
                _logger.LogDebug("PermissionKeyResolverAdapter.ResolveFromRoute: Route mapping placeholder döndü. Route={Route}", normalizedRoute);
                return null;
            }

            if (resolvedKey == null)
            {
                _logger.LogWarning("PermissionKeyResolverAdapter.ResolveFromRoute: Route bulunamadı. Route={Route}", normalizedRoute);
            }
            else
            {
                _logger.LogDebug("PermissionKeyResolverAdapter.ResolveFromRoute: Route çözüldü {Route} → {PermissionKey}", normalizedRoute, resolvedKey);
            }

            return resolvedKey;
        }
    }
}

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    /// <summary>
    /// Sayfa bazlı yetki kontrolü için merkezi servis.
    /// Route-Permission mapping'leri veritabanından dinamik olarak yüklenir.
    /// </summary>
    public class PagePermissionService
    {
        private readonly PermissionStateService _permissionStateService;
        private readonly IModulControllerIslemApiService _islemApiService;
        private readonly ILogger<PagePermissionService> _logger;

        private List<ModulControllerIslemResponseDto> _pagePermissions = new();
        private bool _isLoaded = false;
        private readonly SemaphoreSlim _loadLock = new(1, 1);

        public PagePermissionService(
            PermissionStateService permissionStateService,
            IModulControllerIslemApiService islemApiService,
            ILogger<PagePermissionService> logger)
        {
            _permissionStateService = permissionStateService;
            _islemApiService = islemApiService;
            _logger = logger;
        }

        /// <summary>
        /// Route-Permission mapping'lerini veritabanından yükler
        /// </summary>
        public async Task EnsureLoadedAsync(bool force = false)
        {
            if (_isLoaded && !force)
                return;

            await _loadLock.WaitAsync();
            try
            {
                if (_isLoaded && !force)
                    return;

                var result = await _islemApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    _pagePermissions = result.Data;
                    _logger.LogInformation("PagePermissionService: {Count} sayfa yetkisi yüklendi", _pagePermissions.Count);
                }
                else
                {
                    _pagePermissions = new();
                    _logger.LogWarning("PagePermissionService: Sayfa yetkileri yüklenemedi - {Message}", result.Message);
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PagePermissionService: Sayfa yetkileri yüklenirken hata");
                _pagePermissions = new();
                _isLoaded = true;
            }
            finally
            {
                _loadLock.Release();
            }
        }

        /// <summary>
        /// Cache'i yeniden yükler
        /// </summary>
        public Task RefreshAsync() => EnsureLoadedAsync(force: true);

        /// <summary>
        /// Sayfa için gerekli yetki bilgisini döner (DB'den dinamik)
        /// </summary>
        public async Task<(string? PermissionKey, YetkiSeviyesi MinLevel, SayfaTipi SayfaTipi)?> GetPagePermissionAsync(string path)
        {
            await EnsureLoadedAsync();

            var normalizedPath = NormalizePath(path);

            // Route'u olan kayıtları filtrele
            var pagesWithRoutes = _pagePermissions.Where(p => !string.IsNullOrEmpty(p.Route)).ToList();

            // 1. Exact match
            var exactMatch = pagesWithRoutes.FirstOrDefault(p =>
                string.Equals(p.Route, normalizedPath, StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
                return (exactMatch.PermissionKey, exactMatch.MinYetkiSeviyesi, exactMatch.SayfaTipi);

            // 2. Pattern match (örn: /personel/manage ile /personel/manage/12345 eşleşir)
            var patternMatch = pagesWithRoutes
                .Where(p => IsPatternMatch(normalizedPath, p.Route!))
                .OrderByDescending(p => p.Route!.Length) // En spesifik eşleşmeyi al
                .FirstOrDefault();

            if (patternMatch != null)
                return (patternMatch.PermissionKey, patternMatch.MinYetkiSeviyesi, patternMatch.SayfaTipi);

            // 3. Prefix match (parent route'a fallback)
            var segments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = segments.Length - 1; i >= 1; i--)
            {
                var parentPath = "/" + string.Join("/", segments.Take(i));
                var parentMatch = pagesWithRoutes.FirstOrDefault(p =>
                    string.Equals(p.Route, parentPath, StringComparison.OrdinalIgnoreCase));

                if (parentMatch != null)
                    return (parentMatch.PermissionKey, parentMatch.MinYetkiSeviyesi, parentMatch.SayfaTipi);
            }

            return null;
        }

        /// <summary>
        /// Kullanıcının belirtilen sayfaya erişim yetkisi var mı?
        /// </summary>
        public async Task<bool> CanAccessPageAsync(string path)
        {
            var pagePermission = await GetPagePermissionAsync(path);

            // DB'de tanımsız sayfa
            if (pagePermission == null)
            {
                // Varsayılan: Authenticated (login yeterli) - yeni sayfalar için güvenli başlangıç
                // İsterseniz burayı false yaparak "Default Deny" uygulayabilirsiniz
                return true;
            }

            var (permissionKey, minLevel, sayfaTipi) = pagePermission.Value;

            // Public sayfalar herkese açık
            if (sayfaTipi == SayfaTipi.Public)
                return true;

            // Authenticated sayfalar login ile erişilebilir (yetki gerekmez)
            if (sayfaTipi == SayfaTipi.Authenticated)
                return true;

            // Protected sayfalar için yetki kontrolü
            if (string.IsNullOrEmpty(permissionKey))
                return true; // PermissionKey tanımlı değilse erişime izin ver

            var userLevel = await _permissionStateService.GetPermissionLevelAsync(permissionKey);

            // None seviyesi "yetkisiz" demektir, erişim yok
            if (userLevel == YetkiSeviyesi.None)
                return false;

            // MinYetkiSeviyesi de None ise, bu sayfa için en az View seviyesi gerekli
            var requiredLevel = minLevel > YetkiSeviyesi.None ? minLevel : YetkiSeviyesi.View;

            return userLevel >= requiredLevel;
        }

        /// <summary>
        /// Menü öğesi için yetki kontrolü
        /// </summary>
        public async Task<bool> CanShowMenuItemAsync(string path)
        {
            return await CanAccessPageAsync(path);
        }

        /// <summary>
        /// Banko moduna giriş yetkisi var mı?
        /// </summary>
        public async Task<bool> CanEnterBankoModeAsync()
        {
            var level = await _permissionStateService.GetPermissionLevelAsync("SIRA.BANKO.MODE");
            return level >= YetkiSeviyesi.Edit;
        }

        /// <summary>
        /// Tüm sayfa yetkilerini döner (NavMenu için)
        /// </summary>
        public async Task<List<ModulControllerIslemResponseDto>> GetAllPagePermissionsAsync()
        {
            await EnsureLoadedAsync();
            return _pagePermissions.Where(p => !string.IsNullOrEmpty(p.Route)).ToList();
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "/";

            // Remove query string
            var queryIndex = path.IndexOf('?');
            if (queryIndex >= 0)
                path = path.Substring(0, queryIndex);

            // Ensure starts with /
            if (!path.StartsWith("/"))
                path = "/" + path;

            // Remove trailing slash (except for root)
            if (path.Length > 1 && path.EndsWith("/"))
                path = path.TrimEnd('/');

            return path.ToLowerInvariant();
        }

        private static bool IsPatternMatch(string path, string pattern)
        {
            var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var patternSegments = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments.Length < patternSegments.Length)
                return false;

            for (int i = 0; i < patternSegments.Length; i++)
            {
                if (!string.Equals(pathSegments[i], patternSegments[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}

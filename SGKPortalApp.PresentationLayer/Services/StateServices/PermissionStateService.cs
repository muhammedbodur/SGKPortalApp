using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    public class PermissionStateService
    {
        private readonly IPersonelYetkiApiService _personelYetkiApiService;
        private readonly IModulControllerIslemApiService _modulControllerIslemApiService;
        private readonly IUserInfoService _userInfoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PermissionStateService> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly SemaphoreSlim _loadLock = new(1, 1);

        // PermissionKey -> YetkiSeviyesi dictionary (kullanıcının yetkileri)
        private Dictionary<string, YetkiSeviyesi> _permissions = new();

        // Sistemde tanımlı tüm permission key'ler ve MinYetkiSeviyesi değerleri (ModulControllerIslem tablosundan)
        // Key: PermissionKey, Value: MinYetkiSeviyesi (yetki atanmamış personel için varsayılan seviye)
        private Dictionary<string, YetkiSeviyesi> _definedPermissions = new(StringComparer.OrdinalIgnoreCase);

        // Route → PermissionKey mapping (ModulControllerIslem tablosundan)
        // Key: Route (örn: /personel/departman), Value: PermissionKey (örn: PER.DEPARTMAN.INDEX)
        private Dictionary<string, string> _routeToPermissionKey = new(StringComparer.OrdinalIgnoreCase);

        private const string DefinedPermissionsCacheKey = "PermissionStateService.DefinedPermissions";
        private const string RoutePermissionMapCacheKey = "PermissionStateService.RoutePermissionMap";
        private static readonly MemoryCacheEntryOptions CacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        public event Action? OnChange;

        public bool IsLoaded { get; private set; }

        public PermissionStateService(
            IPersonelYetkiApiService personelYetkiApiService,
            IModulControllerIslemApiService modulControllerIslemApiService,
            IUserInfoService userInfoService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PermissionStateService> logger,
            IMemoryCache memoryCache)
        {
            _personelYetkiApiService = personelYetkiApiService;
            _modulControllerIslemApiService = modulControllerIslemApiService;
            _userInfoService = userInfoService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public Task EnsureLoadedAsync(bool force = false)
        {
            // Zaten yüklüyse ve force değilse, hemen dön
            if (IsLoaded && !force)
                return Task.CompletedTask;

            return LoadPermissionsInternalAsync(force);
        }

        private async Task LoadPermissionsInternalAsync(bool force)
        {
            await _loadLock.WaitAsync();
            try
            {
                // Double-check: Lock aldıktan sonra tekrar kontrol
                if (IsLoaded && !force)
                    return;

                // 0. Sistemde tanımlı tüm permission key'leri yükle (her login'de temiz yükle)
                await LoadDefinedPermissionKeysAsync();

                // 1. Önce claims'den okumayı dene (DB'ye gitmeden)
                if (TryLoadFromClaims())
                {
                    IsLoaded = true;
                    _logger.LogDebug("🔑 Yetkiler claims'den yüklendi. Toplam: {Count}", _permissions.Count);
                    OnChange?.Invoke();
                    return;
                }

                // 2. Claims'de yoksa DB'den çek
                var tcKimlikNo = _userInfoService.GetTcKimlikNo();
                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                {
                    _permissions = new();
                    IsLoaded = true;
                    return;
                }

                _logger.LogDebug("🔑 Yetkiler DB'den yükleniyor. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);

                var permsResult = await _personelYetkiApiService.GetByTcKimlikNoAsync(tcKimlikNo);
                if (!permsResult.Success || permsResult.Data == null)
                {
                    _permissions = new();
                }
                else
                {
                    _permissions = permsResult.Data
                        .Where(p => !string.IsNullOrEmpty(p.PermissionKey))
                        .ToDictionary(p => p.PermissionKey, p => p.YetkiSeviyesi);
                }

                IsLoaded = true;
                _logger.LogDebug("🔑 Yetkiler DB'den yüklendi. Toplam: {Count}", _permissions.Count);
                OnChange?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PermissionStateService.LoadPermissionsInternalAsync hata");
                _permissions = new();
                IsLoaded = true;
                OnChange?.Invoke();
            }
            finally
            {
                _loadLock.Release();
            }
        }

        /// <summary>
        /// Sistemde tanımlı tüm permission key'leri yükler (ModulControllerIslem tablosundan)
        /// </summary>
        private async Task LoadDefinedPermissionKeysAsync()
        {
            try
            {
                if (TryLoadDefinitionsFromCache())
                {
                    _logger.LogInformation("📋 Permission tanımları memory cache'den yüklendi. Count={Count}", _definedPermissions.Count);
                    return;
                }

                _logger.LogInformation("📋 LoadDefinedPermissionKeysAsync başlıyor...");
                var result = await _modulControllerIslemApiService.GetAllAsync();

                _logger.LogInformation("📋 API sonucu: Success={Success}, DataCount={Count}",
                    result.Success, result.Data?.Count ?? 0);

                if (result.Success && result.Data != null)
                {
                    var permissionsWithKey = result.Data
                        .Where(x => !string.IsNullOrEmpty(x.PermissionKey))
                        .ToList();

                    _logger.LogInformation("📋 PermissionKey'li kayıt sayısı: {Count}", permissionsWithKey.Count);

                    _definedPermissions = permissionsWithKey
                        .ToDictionary(
                            x => x.PermissionKey!,
                            x => x.MinYetkiSeviyesi,
                            StringComparer.OrdinalIgnoreCase);

                    var routeMappings = result.Data
                        .Where(x => !string.IsNullOrEmpty(x.Route) && !string.IsNullOrEmpty(x.PermissionKey))
                        .ToList();

                    _routeToPermissionKey = routeMappings
                        .ToDictionary(
                            x => x.Route!.TrimEnd('/'),
                            x => x.PermissionKey!,
                            StringComparer.OrdinalIgnoreCase);

                    _memoryCache.Set(DefinedPermissionsCacheKey, _definedPermissions, CacheOptions);
                    _memoryCache.Set(RoutePermissionMapCacheKey, _routeToPermissionKey, CacheOptions);

                    _logger.LogInformation("🔑 Sistemde tanımlı {Count} permission key yüklendi", _definedPermissions.Count);
                    _logger.LogInformation("🗺️ {Count} route-to-permission mapping yüklendi", _routeToPermissionKey.Count);

                    foreach (var kvp in _definedPermissions.Take(10))
                    {
                        _logger.LogDebug("  - {Key} = {Value}", kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("📋 API başarısız veya data null: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadDefinedPermissionKeysAsync hata");
            }
        }

        /// <summary>
        /// Claims'den yetkileri okur. Başarılıysa true döner.
        /// </summary>
        private bool TryLoadFromClaims()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var permissionsClaim = httpContext?.User?.FindFirst("Permissions")?.Value;

                if (string.IsNullOrEmpty(permissionsClaim))
                {
                    _logger.LogWarning("⚠️ Claims'de Permissions bulunamadı");
                    return false;
                }

                var permissionsDict = JsonSerializer.Deserialize<Dictionary<string, int>>(permissionsClaim);
                if (permissionsDict == null)
                {
                    _logger.LogWarning("⚠️ Permissions claim deserialize edilemedi");
                    return false;
                }

                _permissions = permissionsDict.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (YetkiSeviyesi)kvp.Value);

                _logger.LogInformation("✅ Claims'den {Count} yetki yüklendi", _permissions.Count);
                
                // İlk 20 yetkiyi logla
                foreach (var perm in _permissions.Take(20))
                {
                    _logger.LogInformation("🔑   - {Key} = {Level}", perm.Key, perm.Value);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Claims'den yetki okuma hatası");
                return false;
            }
        }

        /// <summary>
        /// Yetki tanımlarını DB'den yeniden çeker (SignalR permissionDefinitionsChanged geldiğinde çağrılır)
        /// Yeni yetki tanımı eklendiğinde veya MinYetkiSeviyesi değiştiğinde tüm kullanıcılar bu metodu çağırır
        /// </summary>
        public async Task RefreshDefinitionsAsync()
        {
            await _loadLock.WaitAsync();
            try
            {
                _logger.LogInformation("📋 Yetki tanımları yenileniyor...");

                _memoryCache.Remove(DefinedPermissionsCacheKey);
                _memoryCache.Remove(RoutePermissionMapCacheKey);

                await LoadDefinedPermissionKeysAsync();
                _logger.LogInformation("📋 Yetki tanımları yenilendi. Toplam: {Count}", _definedPermissions.Count);
                OnChange?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RefreshDefinitionsAsync hatası");
            }
            finally
            {
                _loadLock.Release();
            }
        }

        private bool TryLoadDefinitionsFromCache()
        {
            var hasDefinitions = _memoryCache.TryGetValue(DefinedPermissionsCacheKey, out Dictionary<string, YetkiSeviyesi>? cachedDefinitions);
            var hasRoutes = _memoryCache.TryGetValue(RoutePermissionMapCacheKey, out Dictionary<string, string>? cachedRoutes);

            if (hasDefinitions && hasRoutes && cachedDefinitions != null && cachedRoutes != null)
            {
                _definedPermissions = cachedDefinitions;
                _routeToPermissionKey = cachedRoutes;
                return true;
            }

            return false;
        }

        public (IReadOnlyDictionary<string, YetkiSeviyesi>? DefinedPermissions, IReadOnlyDictionary<string, string>? RouteMap) GetCacheSnapshot()
        {
            _memoryCache.TryGetValue(DefinedPermissionsCacheKey, out Dictionary<string, YetkiSeviyesi>? defined);
            _memoryCache.TryGetValue(RoutePermissionMapCacheKey, out Dictionary<string, string>? routes);
            return (defined, routes);
        }

        /// <summary>
        /// Yetkileri DB'den yeniden çeker (SignalR bildirimi geldiğinde çağrılır)
        /// NOT: Cookie güncellemesi JS tarafından /auth/refreshpermissions endpoint'i ile yapılır
        /// </summary>
        public async Task RefreshAsync()
        {
            IsLoaded = false;
            
            // DB'den yeniden çek
            var tcKimlikNo = _userInfoService.GetTcKimlikNo();
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return;

            await _loadLock.WaitAsync();
            try
            {
                _logger.LogInformation("🔑 Yetkiler SignalR ile yenileniyor. TcKimlikNo: {TcKimlikNo}", tcKimlikNo);

                var permsResult = await _personelYetkiApiService.GetByTcKimlikNoAsync(tcKimlikNo);
                if (permsResult.Success && permsResult.Data != null)
                {
                    _permissions = permsResult.Data
                        .Where(p => !string.IsNullOrEmpty(p.PermissionKey))
                        .ToDictionary(p => p.PermissionKey, p => p.YetkiSeviyesi);
                    
                    _logger.LogInformation("🔑 Kullanıcı yetkileri yüklendi. Toplam: {Count}", _permissions.Count);
                    
                    // Debug: İlk 20 yetkiyi logla
                    foreach (var perm in _permissions.Take(20))
                    {
                        _logger.LogInformation("🔑   - {Key} = {Level}", perm.Key, perm.Value);
                    }
                }

                IsLoaded = true;
                _logger.LogInformation("🔑 Yetkiler yenilendi. Toplam: {Count}", _permissions.Count);
                OnChange?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RefreshAsync hatası");
            }
            finally
            {
                _loadLock.Release();
            }
        }

        /// <summary>
        /// Permission key bazlı yetki seviyesi döner
        /// Senkron versiyon - EnsureLoadedAsync önceden çağrılmış olmalı
        ///
        /// Mantık:
        /// - Key sistemde tanımlı DEĞİLSE → Edit (henüz permission uygulanmamış, geçici tam yetki)
        /// - Key sistemde tanımlı VE kullanıcıya verilmişse → Kullanıcıya verilen seviye
        /// - Key sistemde tanımlı VE kullanıcıya verilmemişse → None (default deny - güvenlik)
        /// </summary>
        public YetkiSeviyesi GetLevel(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
            {
                _logger.LogWarning("⚠️ GetLevel: PermissionKey boş!");
                return YetkiSeviyesi.None;
            }

            // ⚠️ ÖZEL DURUM: Route mapping bulunamayan sayfalar (geliştirme aşamasında)
            if (permissionKey == "UNKNOWN")
            {
                _logger.LogWarning("🔓 GetLevel: Key=UNKNOWN (route mapping yok), geçici erişim veriliyor (Edit)");
                return YetkiSeviyesi.Edit;
            }

            // 1. Sistemde tanımlı mı kontrol et (önce bu kontrolü yapalım)
            var isDefinedInSystem = _definedPermissions.ContainsKey(permissionKey);

            // 2. Kullanıcının bu key için yetkisi var mı?
            var matchingKey = _permissions.Keys.FirstOrDefault(k =>
                string.Equals(k, permissionKey, StringComparison.OrdinalIgnoreCase));

            if (matchingKey != null && _permissions.TryGetValue(matchingKey, out var level))
            {
                _logger.LogInformation("✅ GetLevel: Key={Key}, Level={Level} (kullanıcıya verilmiş)", permissionKey, level);
                return level;
            }
            else
            {
                _logger.LogWarning("⚠️ GetLevel: Key={Key} kullanıcı permission'larında bulunamadı. _permissions.Count={Count}", permissionKey, _permissions.Count);
            }

            // 3. Kullanıcıya verilmemiş - sistemde tanımlı mı?
            if (isDefinedInSystem)
            {
                // Sistemde tanımlı ama kullanıcıya verilmemiş → None (default deny)
                // MinYetkiSeviyesi kullanılmıyor çünkü o sadece metadata (gerekli minimum seviye)
                _logger.LogWarning("🔒 GetLevel: Key={Key}, Level=None (sistemde tanımlı ama kullanıcıya atanmamış, erişim reddedildi)", permissionKey);
                return YetkiSeviyesi.None;
            }

            // 4. Sistemde tanımlı değil → Edit (henüz permission uygulanmamış)
            // ⚠️ UYARI: Bu durumda geçici olarak tam yetki veriliyor
            // Yeni sayfa eklendiğinde permission tanımlanana kadar erişim sağlanır
            _logger.LogWarning("🔓 GetLevel: Key={Key}, Level=Edit (sistemde tanımlı değil, geçici tam yetki veriliyor!)", permissionKey);
            return YetkiSeviyesi.Edit;
        }

        /// <summary>
        /// Permission key bazlı görüntüleme yetkisi kontrolü
        /// </summary>
        public bool CanView(string permissionKey)
            => GetLevel(permissionKey) >= YetkiSeviyesi.View;

        /// <summary>
        /// Permission key bazlı düzenleme yetkisi kontrolü
        /// </summary>
        public bool CanEdit(string permissionKey)
            => GetLevel(permissionKey) >= YetkiSeviyesi.Edit;

        /// <summary>
        /// Permission key bazlı yetki seviyesi döner (async versiyon)
        /// </summary>
        public async Task<YetkiSeviyesi> GetPermissionLevelAsync(string permissionKey)
        {
            await EnsureLoadedAsync();
            return GetLevel(permissionKey);
        }

        /// <summary>
        /// Permission key bazlı yetki kontrolü
        /// </summary>
        public async Task<bool> HasPermissionAsync(string permissionKey, YetkiSeviyesi minLevel)
        {
            var level = await GetPermissionLevelAsync(permissionKey);
            return level >= minLevel;
        }

        /// <summary>
        /// Belirli bir permission key için kullanıcının yetkisi var mı?
        /// </summary>
        public async Task<bool> CanViewAsync(string permissionKey)
        {
            await EnsureLoadedAsync();
            return CanView(permissionKey);
        }

        /// <summary>
        /// Belirli bir permission key için kullanıcının düzenleme yetkisi var mı?
        /// </summary>
        public async Task<bool> CanEditAsync(string permissionKey)
        {
            await EnsureLoadedAsync();
            return CanEdit(permissionKey);
        }

        /// <summary>
        /// Route'tan PermissionKey'i çözümler
        /// Örnek: "/personel/departman" → "PER.DEPARTMAN.INDEX"
        /// 
        /// ASP.NET Core Routing: /personel ve /personel/index aynı sayfaya gider
        /// Bu yüzden her iki route'u da kontrol ediyoruz
        /// </summary>
        public string? GetPermissionKeyByRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            // Route'u normalize et (trailing slash kaldır)
            var normalizedRoute = route.TrimEnd('/');

            _logger.LogInformation("🗺️ GetPermissionKeyByRoute çağrıldı: {Route}, Dictionary count: {Count}", normalizedRoute, _routeToPermissionKey.Count);

            // DEBUG: İlk 10 route mapping'i logla
            if (_routeToPermissionKey.Count > 0)
            {
                _logger.LogInformation("🗺️ İlk 10 route mapping:");
                foreach (var kvp in _routeToPermissionKey.Take(10))
                {
                    _logger.LogInformation("  - {Route} → {Key}", kvp.Key, kvp.Value);
                }
            }

            // 1. Önce tam eşleşme dene
            if (_routeToPermissionKey.TryGetValue(normalizedRoute, out var permissionKey))
            {
                _logger.LogInformation("✅ Route resolved (exact match): {Route} → {PermissionKey}", normalizedRoute, permissionKey);
                return permissionKey;
            }

            // 2. Eğer route /index ile bitmiyorsa, /index ekleyip tekrar dene
            // Örnek: /personel → /personel/index
            if (!normalizedRoute.EndsWith("/index", StringComparison.OrdinalIgnoreCase))
            {
                var routeWithIndex = $"{normalizedRoute}/index";
                if (_routeToPermissionKey.TryGetValue(routeWithIndex, out permissionKey))
                {
                    _logger.LogInformation("✅ Route resolved (with /index): {RouteOriginal} + /index → {PermissionKey}", normalizedRoute, permissionKey);
                    return permissionKey;
                }
            }

            // 3. Eğer route /index ile bitiyorsa, /index'i kaldırıp tekrar dene
            // Örnek: /personel/index → /personel
            if (normalizedRoute.EndsWith("/index", StringComparison.OrdinalIgnoreCase))
            {
                var routeWithoutIndex = normalizedRoute.Substring(0, normalizedRoute.Length - 6); // "/index" = 6 karakter
                if (_routeToPermissionKey.TryGetValue(routeWithoutIndex, out permissionKey))
                {
                    _logger.LogInformation("✅ Route resolved (without /index): {RouteOriginal} - /index → {PermissionKey}", normalizedRoute, permissionKey);
                    return permissionKey;
                }
            }

            _logger.LogWarning("❌ Route bulunamadı: {Route} (dictionary'de {Count} kayıt var)", normalizedRoute, _routeToPermissionKey.Count);
            return null;
        }
    }
}

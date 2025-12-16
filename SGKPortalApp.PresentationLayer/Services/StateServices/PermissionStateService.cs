using Microsoft.AspNetCore.Http;
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

        private readonly SemaphoreSlim _loadLock = new(1, 1);

        // PermissionKey -> YetkiSeviyesi dictionary (kullanıcının yetkileri)
        private Dictionary<string, YetkiSeviyesi> _permissions = new();
        
        // Sistemde tanımlı tüm permission key'ler (ModulControllerIslem tablosundan)
        private HashSet<string> _definedPermissionKeys = new(StringComparer.OrdinalIgnoreCase);

        public event Action? OnChange;

        public bool IsLoaded { get; private set; }

        public PermissionStateService(
            IPersonelYetkiApiService personelYetkiApiService,
            IModulControllerIslemApiService modulControllerIslemApiService,
            IUserInfoService userInfoService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PermissionStateService> logger)
        {
            _personelYetkiApiService = personelYetkiApiService;
            _modulControllerIslemApiService = modulControllerIslemApiService;
            _userInfoService = userInfoService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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

                // 0. Sistemde tanımlı tüm permission key'leri yükle (bir kez)
                if (_definedPermissionKeys.Count == 0)
                {
                    await LoadDefinedPermissionKeysAsync();
                }

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
                var result = await _modulControllerIslemApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    _definedPermissionKeys = result.Data
                        .Where(x => !string.IsNullOrEmpty(x.PermissionKey))
                        .Select(x => x.PermissionKey!)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    
                    _logger.LogDebug("🔑 Sistemde tanımlı {Count} permission key yüklendi", _definedPermissionKeys.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "LoadDefinedPermissionKeysAsync hata");
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
                    return false;

                var permissionsDict = JsonSerializer.Deserialize<Dictionary<string, int>>(permissionsClaim);
                if (permissionsDict == null)
                    return false;

                _permissions = permissionsDict.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (YetkiSeviyesi)kvp.Value);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Claims'den yetki okuma hatası");
                return false;
            }
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
        /// Permission key bazlı yetki seviyesi döner (örn: "PER.PERSONEL.LIST")
        /// Senkron versiyon - EnsureLoadedAsync önceden çağrılmış olmalı
        /// 
        /// Mantık:
        /// - Key sistemde tanımlı DEĞİLSE → Edit (henüz permission uygulanmamış, tam yetki)
        /// - Key sistemde tanımlı VE kullanıcıya verilmişse → Verilen seviye
        /// - Key sistemde tanımlı VE kullanıcıya verilmemişse → None (yetki yok)
        /// </summary>
        public YetkiSeviyesi GetLevel(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                return YetkiSeviyesi.None;

            // 1. Kullanıcının bu key için yetkisi var mı?
            var matchingKey = _permissions.Keys.FirstOrDefault(k => 
                string.Equals(k, permissionKey, StringComparison.OrdinalIgnoreCase));

            if (matchingKey != null && _permissions.TryGetValue(matchingKey, out var level))
            {
                _logger.LogDebug("GetLevel: Key={Key}, Level={Level} (kullanıcıya verilmiş)", permissionKey, level);
                return level;
            }

            // 2. Kullanıcıya verilmemiş - sistemde tanımlı mı kontrol et
            if (_definedPermissionKeys.Contains(permissionKey))
            {
                // Sistemde tanımlı ama kullanıcıya verilmemiş → None
                _logger.LogDebug("GetLevel: Key={Key}, Level=None (sistemde tanımlı, kullanıcıya verilmemiş)", permissionKey);
                return YetkiSeviyesi.None;
            }

            // 3. Sistemde tanımlı değil → Edit (henüz permission uygulanmamış)
            _logger.LogDebug("GetLevel: Key={Key}, Level=Edit (sistemde tanımlı değil, tam yetki)", permissionKey);
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
    }
}

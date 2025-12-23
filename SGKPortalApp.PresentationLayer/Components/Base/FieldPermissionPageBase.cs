using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Base
{
    /// <summary>
    /// Field-level ve Action-level permission destekli sayfa base class'ı.
    ///
    /// Convention-based permission key üretimi:
    /// - FormField: {FieldPermissionKeyPrefix}.FORMFIELD.{FIELDNAME}  (örn: PERSONEL.MANAGE.FORMFIELD.EMAIL)
    /// - Action: {PagePermissionKey}.ACTION.{ACTIONNAME}              (örn: PERSONEL.INDEX.ACTION.DETAIL)
    ///
    /// Kullanım (Manage/Create sayfaları):
    /// 1. Sayfanızda bu class'ı inherit edin
    /// 2. PagePermissionKey artık otomatik route'tan çözümlenir (manuel override GEREKMEZ)
    /// 3. IsEditMode property'sini override edin (Create/Edit ayrımı için)
    /// 4. Razor'da: @if (IsFieldVisible(nameof(Model.Email))) ve disabled="@(!CanEditField(nameof(Model.Email)))"
    ///
    /// Kullanım (List/Index sayfaları):
    /// 1. PagePermissionKey otomatik çözümlenir (route: /personel → key: PERSONEL.INDEX)
    /// 2. Razor'da: @if (CanAction("DETAIL")) { <button @onclick="NavigateToDetail">Detay</button> }
    /// </summary>
    public abstract class FieldPermissionPageBase : BasePageComponent, IDisposable
    {
        [Inject] protected PermissionStateService PermissionStateService { get; set; } = default!;
        [Inject] protected BusinessLogicLayer.Interfaces.Common.IPermissionKeyResolverService PermissionKeyResolver { get; set; } = default!;
        [Inject] protected ILogger<FieldPermissionPageBase> Logger { get; set; } = default!;

        /// <summary>
        /// Permissions yüklendi mi?
        /// </summary>
        protected bool IsPermissionsLoaded => PermissionStateService.IsLoaded;

        /// <summary>
        /// Sayfa için permission key (örn: "PERSONEL.MANAGE")
        /// Create/Edit ayrımı olan sayfalarda dinamik olabilir
        ///
        /// ⚠️ UYARI: Bu property artık OPTIONAL!
        /// - Eğer override edilirse: Manuel değer kullanılır (geriye uyumluluk)
        /// - Eğer override edilmezse: Route'tan otomatik çözümlenir
        /// </summary>
        protected virtual string? PagePermissionKey => null;

        /// <summary>
        /// Çözümlenmiş permission key (cache)
        /// </summary>
        private string? _resolvedPermissionKey;

        /// <summary>
        /// Gerçek permission key'i döndürür (manuel veya otomatik)
        /// </summary>
        protected string ResolvedPermissionKey
        {
            get
            {
                if (_resolvedPermissionKey != null &&
                    _resolvedPermissionKey != PermissionStateService.RouteLoadingPlaceholderKey)
                    return _resolvedPermissionKey;

                if (!IsPermissionsLoaded || !PermissionStateService.RouteMappingsLoaded)
                {
                    Logger?.LogInformation("⌛ ResolvedPermissionKey: Permissions veya route mapping henüz yüklenmedi");
                    return PermissionStateService.RouteLoadingPlaceholderKey;
                }

                // 1. Manuel override varsa onu kullan (geriye uyumluluk)
                if (!string.IsNullOrEmpty(PagePermissionKey))
                {
                    _resolvedPermissionKey = PagePermissionKey;
                    //Logger?.LogInformation("🔑 ResolvedPermissionKey: Manuel override kullanıldı: {Key}", _resolvedPermissionKey);
                    return _resolvedPermissionKey;
                }

                // 2. Route'tan otomatik çözümle (⚡ PermissionKeyResolver kullanılıyor - TEK MEKANIZMA)
                var currentPath = GetCurrentRoutePath();
                Logger?.LogInformation("🔍 ResolvedPermissionKey: Route={Route}", currentPath);

                // Sync metod kullan (cache'den oku)
                var resolvedKey = PermissionKeyResolver.ResolveFromRouteSync(currentPath);

                if (resolvedKey == null)
                {
                    Logger?.LogWarning("⚠️ ResolvedPermissionKey: Route mapping bulunamadı, UNKNOWN kullanılıyor. Route: {Route}", currentPath);
                    _resolvedPermissionKey = "UNKNOWN";
                    return _resolvedPermissionKey;
                }

                _resolvedPermissionKey = resolvedKey;
                Logger?.LogInformation("✅ ResolvedPermissionKey: PermissionKeyResolver döndü: {Key}", _resolvedPermissionKey);

                return _resolvedPermissionKey;
            }
        }

        /// <summary>
        /// Permission context yüklenene kadar sayfa render'ını bekletmek için kullanılabilir.
        /// </summary>
        protected bool ShouldShowPermissionLoading =>
            !IsPermissionsLoaded ||
            !PermissionStateService.RouteMappingsLoaded ||
            ResolvedPermissionKey == PermissionStateService.RouteLoadingPlaceholderKey;

        /// <summary>
        /// Permission yüklenme sürecinde gösterilecek varsayılan içerik.
        /// İsterseniz override ederek sayfa özelinde özelleştirebilirsiniz.
        /// </summary>
        protected virtual RenderFragment PermissionLoadingFragment => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "d-flex align-items-center justify-content-center");
            builder.AddAttribute(2, "style", "min-height: 40vh;");

            builder.OpenElement(3, "div");
            builder.AddAttribute(4, "class", "text-center");

            builder.OpenElement(5, "div");
            builder.AddAttribute(6, "class", "spinner-border text-primary mb-3");
            builder.AddAttribute(7, "role", "status");

            builder.OpenElement(8, "span");
            builder.AddAttribute(9, "class", "visually-hidden");
            builder.AddContent(10, "Yükleniyor...");
            builder.CloseElement(); // span

            builder.CloseElement(); // spinner div

            builder.OpenElement(11, "p");
            builder.AddAttribute(12, "class", "text-muted mb-0");
            builder.AddContent(13, "Yetki bilgileri yükleniyor, lütfen bekleyin…");
            builder.CloseElement(); // p

            builder.CloseElement(); // text-center div
            builder.CloseElement(); // wrapper div
        };

        /// <summary>
        /// Mevcut route path'ini alır
        /// Örnek: https://localhost:8080/personel/departman → /personel/departman
        /// </summary>
        private string GetCurrentRoutePath()
        {
            try
            {
                var uri = new Uri(NavigationManager.Uri);
                var path = uri.AbsolutePath.TrimEnd('/');
                
                // Query string ve fragment'ı kaldır
                if (path.Contains('?'))
                    path = path.Substring(0, path.IndexOf('?'));
                
                return path;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Field-level permission key prefix'i (örn: "PERSONEL.MANAGE")
        /// Field permission key'leri bu prefix + ".FORMFIELD." + fieldName şeklinde üretilir
        /// Varsayılan olarak ResolvedPermissionKey kullanılır, gerekirse override edilebilir
        /// </summary>
        protected virtual string FieldPermissionKeyPrefix => ResolvedPermissionKey;

        /// <summary>
        /// Sayfa Edit modunda mı? (Create modunda field-level permission uygulanmaz)
        /// Alt sınıflar tarafından override edilmeli
        /// </summary>
        protected virtual bool IsEditMode => false;

        #region Page-Level Permissions

        /// <summary>
        /// Sayfa görüntüleme yetkisi var mı?
        /// </summary>
        protected bool CanViewPage
        {
            get
            {
                var key = ResolvedPermissionKey;
                var level = PermissionStateService.GetLevel(key);
                var canView = level >= YetkiSeviyesi.View;

                Logger?.LogInformation("🔍 CanViewPage: Key={Key}, Level={Level}, CanView={CanView}", key, level, canView);

                return canView;
            }
        }

        /// <summary>
        /// Sayfa düzenleme yetkisi var mı?
        /// </summary>
        protected bool CanEditPage => PermissionStateService.CanEdit(ResolvedPermissionKey);

        #endregion

        #region Field-Level Permissions (Convention-based)

        /// <summary>
        /// Convention-based field permission key üretir: {FieldPermissionKeyPrefix}.FORMFIELD.{FIELDNAME}
        /// Örnek: PERSONEL.MANAGE.FORMFIELD.EMAIL
        /// </summary>
        protected string GetFieldPermissionKey(string fieldName)
        {
            var prefix = FieldPermissionKeyPrefix;
            if (prefix == PermissionStateService.RouteLoadingPlaceholderKey)
                return PermissionStateService.RouteLoadingPlaceholderKey;

            return $"{prefix}.FORMFIELD.{fieldName.ToUpperInvariant()}";
        }

        /// <summary>
        /// Field-level edit yetkisi kontrolü
        /// Create modunda her zaman true döner (field-level permission sadece Edit modunda aktif)
        /// </summary>
        protected bool CanEditField(string fieldName)
        {
            if (!IsEditMode) return true; // Create modunda field-level permission yok
            return PermissionStateService.CanEdit(GetFieldPermissionKey(fieldName));
        }

        /// <summary>
        /// Field-level view yetkisi kontrolü
        /// Create modunda her zaman true döner
        /// </summary>
        protected bool CanViewField(string fieldName)
        {
            if (!IsEditMode) return true;
            return PermissionStateService.CanView(GetFieldPermissionKey(fieldName));
        }

        /// <summary>
        /// Field görünür mü? (None = görünmez, View/Edit = görünür)
        /// Create modunda her alan görünür
        /// </summary>
        protected bool IsFieldVisible(string fieldName)
        {
            if (!IsEditMode) return true;
            var level = PermissionStateService.GetLevel(GetFieldPermissionKey(fieldName));
            return level != YetkiSeviyesi.None;
        }

        /// <summary>
        /// Field'ın mevcut yetki seviyesini döndürür
        /// Create modunda Edit döner (tam yetki)
        /// </summary>
        protected YetkiSeviyesi GetFieldPermissionLevel(string fieldName)
        {
            if (!IsEditMode) return YetkiSeviyesi.Edit;
            return PermissionStateService.GetLevel(GetFieldPermissionKey(fieldName));
        }

        #endregion

        #region Action-Level Permissions (List sayfaları için)

        /// <summary>
        /// Convention-based action permission key üretir: {ResolvedPermissionKey}.ACTION.{ACTIONNAME}
        /// Örnek: PERSONEL.INDEX.ACTION.DETAIL
        /// </summary>
        protected string GetActionPermissionKey(string actionName)
        {
            var key = ResolvedPermissionKey;
            if (key == PermissionStateService.RouteLoadingPlaceholderKey)
                return PermissionStateService.RouteLoadingPlaceholderKey;

            return $"{key}.ACTION.{actionName.ToUpperInvariant()}";
        }

        /// <summary>
        /// Aksiyon yetkisi var mı? (View veya Edit seviyesi yeterli)
        /// Tanımsız permission key = izin ver (sayfa seviyesi kontrolü olmalı)
        /// </summary>
        protected bool CanAction(string actionName)
        {
            var level = PermissionStateService.GetLevel(GetActionPermissionKey(actionName));
            return level >= YetkiSeviyesi.View;
        }

        /// <summary>
        /// Aksiyon görünür mü? (None = görünmez)
        /// </summary>
        protected bool IsActionVisible(string actionName)
        {
            var actionKey = GetActionPermissionKey(actionName);
            var level = PermissionStateService.GetLevel(actionKey);
            var isVisible = level != YetkiSeviyesi.None;
            
            return isVisible;
        }

        /// <summary>
        /// Aksiyon düzenleme yetkisi var mı? (Edit seviyesi gerekli)
        /// </summary>
        protected bool CanEditAction(string actionName)
        {
            return PermissionStateService.CanEdit(GetActionPermissionKey(actionName));
        }

        #endregion

        #region Action-Level Permissions (ActionType Enum Overloads)

        /// <summary>
        /// Convention-based action permission key üretir (ActionType enum ile)
        /// Örnek: PERSONEL.INDEX.ACTION.DETAIL
        /// </summary>
        protected string GetActionPermissionKey(ActionType actionType)
            => GetActionPermissionKey(actionType.ToString());

        /// <summary>
        /// Aksiyon yetkisi var mı? (ActionType enum ile)
        /// </summary>
        protected bool CanAction(ActionType actionType)
            => CanAction(actionType.ToString());

        /// <summary>
        /// Aksiyon görünür mü? (ActionType enum ile)
        /// </summary>
        protected bool IsActionVisible(ActionType actionType)
            => IsActionVisible(actionType.ToString());

        /// <summary>
        /// Aksiyon düzenleme yetkisi var mı? (ActionType enum ile)
        /// </summary>
        protected bool CanEditAction(ActionType actionType)
            => CanEditAction(actionType.ToString());

        /// <summary>
        /// Tıklanabilir satır için CSS style döner (ActionType enum ile)
        /// Yetki varsa "cursor: pointer;", yoksa boş string
        /// </summary>
        protected string GetClickableRowStyle(ActionType actionType)
            => CanAction(actionType) ? "cursor: pointer;" : "";

        #endregion

        #region User Context Helpers

        /// <summary>
        /// Giriş yapmış kullanıcının Hizmet Binası ID'sini döndürür
        /// Claim'de yoksa 0 döner
        /// </summary>
        protected int GetCurrentUserHizmetBinasiId()
        {
            var authState = AuthStateProvider.GetAuthenticationStateAsync().Result;
            var claim = authState.User.FindFirst("HizmetBinasiId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        /// <summary>
        /// Giriş yapmış kullanıcının Hizmet Binası Adını döndürür
        /// </summary>
        protected string GetCurrentUserHizmetBinasiAdi()
        {
            var authState = AuthStateProvider.GetAuthenticationStateAsync().Result;
            return authState.User.FindFirst("HizmetBinasiAdi")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Kullanıcının belirtilen Hizmet Binasını görüntüleme yetkisi var mı?
        /// Güvenlik kontrolü: Kullanıcı sadece kendi Hizmet Binasındaki verileri görebilir
        /// </summary>
        protected bool CanAccessHizmetBinasi(int hizmetBinasiId)
        {
            // Admin kullanıcılar tüm hizmet binalarına erişebilir (isteğe bağlı)
            // TODO: Admin kontrolü eklenebilir

            var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
            return userHizmetBinasiId == hizmetBinasiId;
        }

        #endregion

        #region Field-Level Permissions for Index Pages (Filters için)

        /// <summary>
        /// Index sayfalarındaki field/filter'lar için edit yetkisi kontrolü
        /// IsEditMode kontrolü YAPMAZ (Index sayfaları için)
        /// Permission Key: {PagePermissionKey}.FORMFIELD.{FIELDNAME}
        /// Örnek: SIRAMATIK.BANKO.INDEX.FORMFIELD.HIZMET_BINASI
        /// </summary>
        protected bool CanEditFieldInList(string fieldName)
        {
            return PermissionStateService.CanEdit(GetFieldPermissionKey(fieldName));
        }

        /// <summary>
        /// Index sayfalarındaki field/filter'lar için view yetkisi kontrolü
        /// </summary>
        protected bool CanViewFieldInList(string fieldName)
        {
            return PermissionStateService.CanView(GetFieldPermissionKey(fieldName));
        }

        /// <summary>
        /// Index sayfalarındaki field/filter görünür mü?
        /// None seviyesi = görünmez
        /// </summary>
        protected bool IsFieldVisibleInList(string fieldName)
        {
            var level = PermissionStateService.GetLevel(GetFieldPermissionKey(fieldName));
            return level != YetkiSeviyesi.None;
        }

        /// <summary>
        /// Index sayfalarındaki field/filter disabled olmalı mı?
        /// View seviyesi = disabled, Edit seviyesi = aktif
        /// </summary>
        protected bool IsFieldDisabledInList(string fieldName)
        {
            return !CanEditFieldInList(fieldName);
        }

        #endregion

        #region Lifecycle

        private bool _permissionLifecycleInitialized;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await EnsurePermissionLifecycleInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!_permissionLifecycleInitialized)
            {
                await EnsurePermissionLifecycleInitializedAsync();
            }
        }

        private async Task EnsurePermissionLifecycleInitializedAsync()
        {
            if (_permissionLifecycleInitialized)
                return;

            _permissionLifecycleInitialized = true;

            try
            {
                await PermissionStateService.EnsureLoadedAsync();

                // ⚡ Route → Permission Key mapping'i cache'e yükle (PermissionKeyResolver için)
                var currentPath = GetCurrentRoutePath();
                if (!string.IsNullOrWhiteSpace(currentPath))
                {
                    try
                    {
                        // Async metod cache'i yükler, sync metod kullanabilir
                        await PermissionKeyResolver.ResolveFromRouteAsync(currentPath);
                    }
                    catch (Exception cacheEx)
                    {
                        Logger?.LogWarning(cacheEx, "FieldPermissionPageBase: PermissionKeyResolver cache yüklenemedi");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, "FieldPermissionPageBase: PermissionStateService.EnsureLoadedAsync hata verdi");
            }

            _resolvedPermissionKey = null;
            PermissionStateService.OnChange += HandlePermissionStateChanged;

            if (!IsPermissionsLoaded || !PermissionStateService.RouteMappingsLoaded)
            {
                Logger?.LogWarning("FieldPermissionPageBase: Permission context henüz hazır değil. IsLoaded={IsLoaded}, RouteLoaded={RouteLoaded}",
                    IsPermissionsLoaded, PermissionStateService.RouteMappingsLoaded);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Permission state değiştiğinde sayfayı yeniden render et
        /// </summary>
        protected virtual void HandlePermissionStateChanged()
        {
            // Permission setleri değiştiğinde route mapping yeniden yapılabilsin diye cache'i temizle
            _resolvedPermissionKey = null;
            InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose()
        {
            if (_permissionLifecycleInitialized)
            {
                PermissionStateService.OnChange -= HandlePermissionStateChanged;
            }
        }

        #endregion
    }
}

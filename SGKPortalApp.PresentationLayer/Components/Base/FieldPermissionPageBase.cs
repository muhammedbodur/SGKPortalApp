using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Base
{
    /// <summary>
    /// Field-level ve Action-level permission destekli sayfa base class'ı.
    /// 
    /// Convention-based permission key üretimi:
    /// - Field: {FieldPermissionKeyPrefix}.FIELD.{FIELDNAME}  (örn: PER.PERSONEL.MANAGE.FIELD.EMAIL)
    /// - Action: {PagePermissionKey}.ACTION.{ACTIONNAME}      (örn: PER.PERSONEL.INDEX.ACTION.DETAIL)
    /// 
    /// Kullanım (Manage/Create sayfaları):
    /// 1. Sayfanızda bu class'ı inherit edin
    /// 2. PagePermissionKey property'sini override edin (sayfa yetkisi için)
    /// 3. IsEditMode property'sini override edin (Create/Edit ayrımı için)
    /// 4. Razor'da: @if (IsFieldVisible(nameof(Model.Email))) ve disabled="@(!CanEditField(nameof(Model.Email)))"
    /// 
    /// Kullanım (List/Index sayfaları):
    /// 1. PagePermissionKey = "PER.PERSONEL.INDEX"
    /// 2. Razor'da: @if (CanAction("DETAIL")) { <button @onclick="NavigateToDetail">Detay</button> }
    /// </summary>
    public abstract class FieldPermissionPageBase : BasePageComponent, IDisposable
    {
        [Inject] protected PermissionStateService PermissionStateService { get; set; } = default!;

        /// <summary>
        /// Sayfa için permission key (örn: "PER.PERSONEL.MANAGE")
        /// Create/Edit ayrımı olan sayfalarda dinamik olabilir
        /// </summary>
        protected abstract string PagePermissionKey { get; }

        /// <summary>
        /// Field-level permission key prefix'i (örn: "PER.PERSONEL.MANAGE")
        /// Field permission key'leri bu prefix + ".FIELD." + fieldName şeklinde üretilir
        /// Varsayılan olarak PagePermissionKey kullanılır, gerekirse override edilebilir
        /// </summary>
        protected virtual string FieldPermissionKeyPrefix => PagePermissionKey;

        /// <summary>
        /// Sayfa Edit modunda mı? (Create modunda field-level permission uygulanmaz)
        /// Alt sınıflar tarafından override edilmeli
        /// </summary>
        protected virtual bool IsEditMode => false;

        #region Page-Level Permissions

        /// <summary>
        /// Sayfa görüntüleme yetkisi var mı?
        /// </summary>
        protected bool CanViewPage => PermissionStateService.CanView(PagePermissionKey);

        /// <summary>
        /// Sayfa düzenleme yetkisi var mı?
        /// </summary>
        protected bool CanEditPage => PermissionStateService.CanEdit(PagePermissionKey);

        #endregion

        #region Field-Level Permissions (Convention-based)

        /// <summary>
        /// Convention-based field permission key üretir: {FieldPermissionKeyPrefix}.FIELD.{FIELDNAME}
        /// Örnek: PER.PERSONEL.MANAGE.FIELD.EMAIL
        /// </summary>
        protected string GetFieldPermissionKey(string fieldName)
            => $"{FieldPermissionKeyPrefix}.FIELD.{fieldName.ToUpperInvariant()}";

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
        /// Convention-based action permission key üretir: {PagePermissionKey}.ACTION.{ACTIONNAME}
        /// Örnek: PER.PERSONEL.INDEX.ACTION.DETAIL
        /// </summary>
        protected string GetActionPermissionKey(string actionName)
            => $"{PagePermissionKey}.ACTION.{actionName.ToUpperInvariant()}";

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
            var level = PermissionStateService.GetLevel(GetActionPermissionKey(actionName));
            return level != YetkiSeviyesi.None;
        }

        /// <summary>
        /// Aksiyon düzenleme yetkisi var mı? (Edit seviyesi gerekli)
        /// </summary>
        protected bool CanEditAction(string actionName)
        {
            return PermissionStateService.CanEdit(GetActionPermissionKey(actionName));
        }

        #endregion

        #region Lifecycle

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Permission state'i yükle
            try
            {
                await PermissionStateService.EnsureLoadedAsync();
                PermissionStateService.OnChange += HandlePermissionStateChanged;
            }
            catch (Exception)
            {
                // Sessizce devam et
            }
        }

        /// <summary>
        /// Permission state değiştiğinde sayfayı yeniden render et
        /// </summary>
        protected virtual void HandlePermissionStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose()
        {
            PermissionStateService.OnChange -= HandlePermissionStateChanged;
        }

        #endregion
    }
}

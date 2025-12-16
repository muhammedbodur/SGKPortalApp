using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Permission
{
    /// <summary>
    /// Field-level permission wrapper component.
    /// 
    /// Kullanım 1 (Convention-based - Önerilen):
    /// <PermissionField FieldName="SicilNo" PageKey="PER.PERSONEL.MANAGE" IsEditMode="@IsEditMode">
    ///     <div class="col-md-6 mb-3">
    ///         <label>Sicil No</label>
    ///         <InputNumber @bind-Value="FormModel.SicilNo" disabled="@context.IsDisabled" />
    ///     </div>
    /// </PermissionField>
    /// 
    /// Kullanım 2 (Explicit key):
    /// <PermissionField PermissionKey="PER.PERSONEL.MANAGE.FIELD.EMAIL">
    ///     <ViewTemplate><span>@Model.Email</span></ViewTemplate>
    ///     <EditTemplate><input @bind="Model.Email" /></EditTemplate>
    /// </PermissionField>
    /// </summary>
    public partial class PermissionField : ComponentBase
    {
        [Inject] private PermissionStateService PermissionState { get; set; } = default!;

        #region Parameters - Convention-based (Yeni, Önerilen)

        /// <summary>
        /// Field adı (nameof ile kullanın). PageKey ile birlikte permission key üretir.
        /// Örnek: "SicilNo" -> "PER.PERSONEL.MANAGE.FIELD.SICILNO"
        /// </summary>
        [Parameter] public string? FieldName { get; set; }

        /// <summary>
        /// Sayfa permission key prefix'i. FieldName ile birlikte kullanılır.
        /// CascadingValue ile otomatik alınabilir veya manuel verilebilir.
        /// Örnek: "PER.PERSONEL.MANAGE"
        /// </summary>
        [Parameter] public string? PageKey { get; set; }

        /// <summary>
        /// CascadingValue ile gelen PageKey (otomatik)
        /// </summary>
        [CascadingParameter(Name = "FieldPermissionPageKey")] 
        public string? CascadingPageKey { get; set; }

        /// <summary>
        /// Edit modu mu? Create modunda field permission uygulanmaz (her zaman görünür ve düzenlenebilir).
        /// CascadingValue ile otomatik alınabilir veya manuel verilebilir.
        /// </summary>
        [Parameter] public bool? IsEditMode { get; set; }

        /// <summary>
        /// CascadingValue ile gelen IsEditMode (otomatik)
        /// </summary>
        [CascadingParameter(Name = "FieldPermissionIsEditMode")] 
        public bool CascadingIsEditMode { get; set; } = true;

        /// <summary>
        /// Efektif PageKey (Parameter > CascadingValue)
        /// </summary>
        private string? EffectivePageKey => PageKey ?? CascadingPageKey;

        /// <summary>
        /// Efektif IsEditMode (Parameter > CascadingValue)
        /// </summary>
        private bool EffectiveIsEditMode => IsEditMode ?? CascadingIsEditMode;

        /// <summary>
        /// Convention-based child content. context.IsDisabled ile disabled durumunu alın.
        /// </summary>
        [Parameter] public RenderFragment<PermissionFieldContext>? ChildContent { get; set; }

        #endregion

        #region Parameters - Explicit (Eski, Geriye uyumluluk)

        /// <summary>
        /// Tam permission key (eski kullanım). FieldName+PageKey yerine kullanılabilir.
        /// </summary>
        [Parameter] public string? PermissionKey { get; set; }

        /// <summary>
        /// View modu için template (eski kullanım)
        /// </summary>
        [Parameter] public RenderFragment? ViewTemplate { get; set; }

        /// <summary>
        /// Edit modu için template (eski kullanım)
        /// </summary>
        [Parameter] public RenderFragment? EditTemplate { get; set; }

        /// <summary>
        /// Permission tanımsızsa varsayılan seviye.
        /// None = tanımsız permission'lar gizlenir (güvenli varsayılan)
        /// </summary>
        [Parameter] public YetkiSeviyesi DefaultLevel { get; set; } = YetkiSeviyesi.None;

        #endregion

        private YetkiSeviyesi _level = YetkiSeviyesi.None;
        private PermissionFieldContext _context = new();

        /// <summary>
        /// Hesaplanmış permission key (convention veya explicit)
        /// </summary>
        private string ComputedPermissionKey
        {
            get
            {
                // Explicit key varsa onu kullan
                if (!string.IsNullOrWhiteSpace(PermissionKey))
                    return PermissionKey;

                // Convention-based key üret
                if (!string.IsNullOrWhiteSpace(FieldName) && !string.IsNullOrWhiteSpace(EffectivePageKey))
                    return $"{EffectivePageKey}.FIELD.{FieldName.ToUpperInvariant()}";

                return string.Empty;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdateLevel();
        }

        protected override async Task OnParametersSetAsync()
        {
            await UpdateLevel();
        }

        private async Task UpdateLevel()
        {
            await PermissionState.EnsureLoadedAsync();

            // Create modunda her zaman Edit (field permission uygulanmaz)
            if (!EffectiveIsEditMode)
            {
                _level = YetkiSeviyesi.Edit;
            }
            else if (string.IsNullOrWhiteSpace(ComputedPermissionKey))
            {
                _level = DefaultLevel;
            }
            else
            {
                _level = PermissionState.GetLevel(ComputedPermissionKey);
                
                // Tanımsız permission = varsayılan seviye
                if (_level == YetkiSeviyesi.None && DefaultLevel != YetkiSeviyesi.None)
                    _level = DefaultLevel;
            }

            // Context'i güncelle
            _context = new PermissionFieldContext
            {
                Level = _level,
                IsVisible = _level != YetkiSeviyesi.None,
                IsDisabled = _level < YetkiSeviyesi.Edit,
                CanEdit = _level >= YetkiSeviyesi.Edit,
                CanView = _level >= YetkiSeviyesi.View
            };
        }
    }

    /// <summary>
    /// PermissionField context - child content'e geçirilen bilgiler
    /// </summary>
    public class PermissionFieldContext
    {
        public YetkiSeviyesi Level { get; set; } = YetkiSeviyesi.None;
        public bool IsVisible { get; set; }
        public bool IsDisabled { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
    }
}

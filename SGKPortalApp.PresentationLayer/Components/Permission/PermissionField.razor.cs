using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Permission
{
    public partial class PermissionField : ComponentBase
    {
        [Inject] private PermissionStateService PermissionState { get; set; } = default!;

        [Parameter]
        public string PermissionKey { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment? ViewTemplate { get; set; }

        [Parameter]
        public RenderFragment? EditTemplate { get; set; }

        [Parameter]
        public YetkiSeviyesi DefaultLevel { get; set; } = YetkiSeviyesi.None;

        private YetkiSeviyesi _level = YetkiSeviyesi.None;

        protected override async Task OnInitializedAsync()
        {
            await PermissionState.EnsureLoadedAsync();
            _level = string.IsNullOrWhiteSpace(PermissionKey) 
                ? DefaultLevel 
                : PermissionState.GetLevel(PermissionKey);

            if (_level == YetkiSeviyesi.None && DefaultLevel != YetkiSeviyesi.None)
                _level = DefaultLevel;
        }

        protected override async Task OnParametersSetAsync()
        {
            await PermissionState.EnsureLoadedAsync();
            _level = string.IsNullOrWhiteSpace(PermissionKey) 
                ? DefaultLevel 
                : PermissionState.GetLevel(PermissionKey);

            if (_level == YetkiSeviyesi.None && DefaultLevel != YetkiSeviyesi.None)
                _level = DefaultLevel;
        }
    }
}

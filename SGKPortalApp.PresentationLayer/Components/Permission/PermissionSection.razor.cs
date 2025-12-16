using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Permission
{
    public partial class PermissionSection : ComponentBase
    {
        [Inject] private PermissionStateService PermissionState { get; set; } = default!;

        [Parameter]
        public string PermissionKey { get; set; } = string.Empty;

        [Parameter]
        public YetkiSeviyesi RequiredLevel { get; set; } = YetkiSeviyesi.View;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private bool _hasPermission = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckPermission();
        }

        protected override async Task OnParametersSetAsync()
        {
            await CheckPermission();
        }

        private async Task CheckPermission()
        {
            await PermissionState.EnsureLoadedAsync();

            if (string.IsNullOrWhiteSpace(PermissionKey))
            {
                _hasPermission = true;
                return;
            }

            var level = PermissionState.GetLevel(PermissionKey);
            _hasPermission = level >= RequiredLevel;
        }
    }
}

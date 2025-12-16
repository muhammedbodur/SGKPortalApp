using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;

namespace SGKPortalApp.PresentationLayer.Components.Permission
{
    public partial class PermissionButton : ComponentBase
    {
        [Inject] private PermissionStateService PermissionState { get; set; } = default!;

        [Parameter]
        public string PermissionKey { get; set; } = string.Empty;

        [Parameter]
        public YetkiSeviyesi RequiredLevel { get; set; } = YetkiSeviyesi.View;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string CssClass { get; set; } = "btn btn-primary";

        [Parameter]
        public string ButtonType { get; set; } = "button";

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public bool ShowWhenNoPermission { get; set; } = false;

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

            if (ShowWhenNoPermission)
                _hasPermission = true;
        }

        private async Task HandleClick()
        {
            if (OnClick.HasDelegate)
                await OnClick.InvokeAsync();
        }
    }
}

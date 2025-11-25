using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SGKPortalApp.PresentationLayer.Services.State;

namespace SGKPortalApp.PresentationLayer.Authorization
{
    /// <summary>
    /// Banko modunda erişimi kontrol eden attribute
    /// Banko modundayken sadece banko sayfalarına izin verir
    /// </summary>
    public class BankoModeAuthorizeAttribute : Attribute
    {
        /// <summary>
        /// Banko modunda bu sayfaya erişime izin var mı?
        /// </summary>
        public bool AllowInBankoMode { get; set; } = false;
    }

    /// <summary>
    /// Banko modu kontrolü yapan component wrapper
    /// </summary>
    public class BankoModeGuard : ComponentBase
    {
        [Inject] private BankoModeStateService BankoModeState { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool AllowInBankoMode { get; set; } = false;

        protected override void OnInitialized()
        {
            // Banko modu değişikliklerini dinle
            BankoModeState.OnBankoModeChanged += StateHasChanged;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var tcKimlikNo = HttpContextAccessor.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
            
            // Banko modunda mı kontrol et
            if (BankoModeState.IsInBankoMode && !string.IsNullOrEmpty(tcKimlikNo))
            {
                // Bu personel banko modunda mı?
                if (BankoModeState.IsPersonelInBankoMode(tcKimlikNo))
                {
                    // Bu sayfaya banko modunda erişime izin var mı?
                    if (!AllowInBankoMode)
                    {
                        // İzin yok, banko sayfasına yönlendir
                        var bankoId = BankoModeState.ActiveBankoId;
                        NavigationManager.NavigateTo($"/siramatik/banko/{bankoId}", forceLoad: true);
                        return;
                    }
                }
            }

            // İzin varsa içeriği render et
            builder.AddContent(0, ChildContent);
        }

        public void Dispose()
        {
            BankoModeState.OnBankoModeChanged -= StateHasChanged;
        }
    }
}

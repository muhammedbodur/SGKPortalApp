using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class SiraCagirmaPanel
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] public List<SiraCagirmaResponseDto> SiraListesi { get; set; } = new();
        [Parameter] public EventCallback<int> OnSiraCagir { get; set; }
        [Parameter] public EventCallback<bool> OnPanelStateChanged { get; set; }

        private bool IsVisible { get; set; } = false;
        private bool IsPinned { get; set; } = false;

        private string HeaderBackground => IsPinned
            ? "linear-gradient(135deg, #696cff 0%, #5f61e6 100%)"
            : "linear-gradient(135deg, #8b8dff 0%, #7f81f6 100%)";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await JS.InvokeVoidAsync("SiraCagirmaPanel.init", DotNetObjectReference.Create(this));
                    Console.WriteLine("SiraCagirmaPanel JavaScript initialized");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SiraCagirmaPanel init error: {ex.Message}");
                }
            }
        }

        [JSInvokable]
        public async Task CloseFromJS()
        {
            if (!IsPinned && IsVisible)
            {
                IsVisible = false;
                await OnPanelStateChanged.InvokeAsync(IsVisible);
                StateHasChanged();
            }
        }

        private async Task TogglePanel()
        {
            IsVisible = !IsVisible;
            if (!IsVisible)
            {
                IsPinned = false;
            }

            // JavaScript'e bildir
            try
            {
                if (IsVisible)
                {
                    await JS.InvokeVoidAsync("SiraCagirmaPanel.openPanel");
                }
                else
                {
                    await JS.InvokeVoidAsync("SiraCagirmaPanel.closePanel");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TogglePanel JS error: {ex.Message}");
            }

            await OnPanelStateChanged.InvokeAsync(IsVisible);
        }

        private async Task TogglePin()
        {
            IsPinned = !IsPinned;

            // JavaScript'e pin durumunu bildir
            try
            {
                await JS.InvokeVoidAsync("SiraCagirmaPanel.setPin", IsPinned);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TogglePin JS error: {ex.Message}");
            }

            // Pin kaldırılınca paneli kapat
            if (!IsPinned)
            {
                IsVisible = false;
                await OnPanelStateChanged.InvokeAsync(IsVisible);
                await JS.InvokeVoidAsync("SiraCagirmaPanel.closePanel");
            }
        }

        private async Task SiradakiCagir()
        {
            var siradaki = SiraListesi.FirstOrDefault(x => x.BeklemeDurum == BeklemeDurum.Beklemede);
            if (siradaki != null)
            {
                await OnSiraCagir.InvokeAsync(siradaki.SiraId);
            }
        }

        private async Task SiraSecildi(SiraCagirmaResponseDto sira)
        {
            await OnSiraCagir.InvokeAsync(sira.SiraId);
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class SiraCagirmaPanel : ComponentBase
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
                await LoadStateFromLocalStorage();
                await JS.InvokeVoidAsync("SiraCagirmaPanel.init", DotNetObjectReference.Create(this));
                StateHasChanged();
            }
        }

        [JSInvokable]
        public async Task CloseFromJS()
        {
            if (!IsPinned && IsVisible)
            {
                IsVisible = false;
                SaveStateToLocalStorage();
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
            SaveStateToLocalStorage();
            await OnPanelStateChanged.InvokeAsync(IsVisible);
        }

        private async Task TogglePin()
        {
            IsPinned = !IsPinned;

            // Pin kaldırılınca paneli kapat
            if (!IsPinned)
            {
                IsVisible = false;
                await OnPanelStateChanged.InvokeAsync(IsVisible);
            }

            SaveStateToLocalStorage();
        }

        private async Task SiradakiCagir()
        {
            var siradaki = SiraListesi.FirstOrDefault(x => x.BeklemeDurum == BeklemeDurum.Beklemede);
            if (siradaki != null)
            {
                await OnSiraCagir.InvokeAsync(siradaki.SiraId);
            }
        }

        private async Task LoadStateFromLocalStorage()
        {
            try
            {
                var isVisibleStr = await JS.InvokeAsync<string>("localStorage.getItem", "siraCagirmaPanel_isVisible");
                var isPinnedStr = await JS.InvokeAsync<string>("localStorage.getItem", "siraCagirmaPanel_isPinned");

                IsVisible = isVisibleStr == "true";
                IsPinned = isPinnedStr == "true";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LocalStorage load error: {ex.Message}");
            }
        }

        private async void SaveStateToLocalStorage()
        {
            try
            {
                await JS.InvokeVoidAsync("localStorage.setItem", "siraCagirmaPanel_isVisible", IsVisible.ToString().ToLower());
                await JS.InvokeVoidAsync("localStorage.setItem", "siraCagirmaPanel_isPinned", IsPinned.ToString().ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LocalStorage save error: {ex.Message}");
            }
        }
    }
}

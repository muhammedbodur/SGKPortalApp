using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class SiraCagirmaPanel : IDisposable
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Parameter] public List<SiraCagirmaResponseDto> SiraListesi { get; set; } = new();
        [Parameter] public EventCallback<int> OnSiraCagir { get; set; }
        [Parameter] public EventCallback<bool> OnPanelStateChanged { get; set; }

        private DotNetObjectReference<SiraCagirmaPanel>? dotNetReference;
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
                    dotNetReference = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("SiraCagirmaPanel.init", dotNetReference);

                    // JavaScript'ten mevcut durumu senkronize et
                    await SyncStateFromLocalStorage();

                    Console.WriteLine("✅ SiraCagirmaPanel JavaScript initialized");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ SiraCagirmaPanel init error: {ex.Message}");
                }
            }
        }

        private async Task SyncStateFromLocalStorage()
        {
            try
            {
                // LocalStorage'dan durumu oku
                var isPinnedStr = await JS.InvokeAsync<string>("localStorage.getItem", "callPanelIsPinned");
                var isVisibleStr = await JS.InvokeAsync<string>("localStorage.getItem", "callPanelIsVisible");

                IsPinned = isPinnedStr == "true";
                IsVisible = isVisibleStr == "true";

                StateHasChanged();

                Console.WriteLine($"🔄 State senkronize edildi - Pinned: {IsPinned}, Visible: {IsVisible}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ State sync error: {ex.Message}");
            }
        }

        [JSInvokable]
        public async Task CloseFromJS()
        {
            if (!IsPinned)
            {
                IsVisible = false;
                await OnPanelStateChanged.InvokeAsync(IsVisible);
                StateHasChanged();
                Console.WriteLine("ℹ️ Panel JS tarafından kapatıldı");
            }
        }

        [JSInvokable]
        public void UpdateStateFromJS(bool isVisible, bool isPinned)
        {
            IsVisible = isVisible;
            IsPinned = isPinned;
            StateHasChanged();
            Console.WriteLine($"🔄 State JS'den güncellendi - Visible: {IsVisible}, Pinned: {IsPinned}");
        }

        private async Task TogglePanel()
        {
            try
            {
                await JS.InvokeVoidAsync("SiraCagirmaPanel.togglePanel");

                // State'i güncelle
                IsVisible = !IsVisible;
                await OnPanelStateChanged.InvokeAsync(IsVisible);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TogglePanel error: {ex.Message}");
            }
        }

        private async Task TogglePin()
        {
            try
            {
                IsPinned = !IsPinned;
                await JS.InvokeVoidAsync("SiraCagirmaPanel.setPin", IsPinned);
                StateHasChanged();

                Console.WriteLine($"📌 Pin durumu değişti: {IsPinned}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TogglePin error: {ex.Message}");
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

        public void Dispose()
        {
            try
            {
                dotNetReference?.Dispose();
                JS.InvokeVoidAsync("SiraCagirmaPanel.destroy");
            }
            catch
            {
                // Cleanup hatası önemsiz
            }
        }
    }
}
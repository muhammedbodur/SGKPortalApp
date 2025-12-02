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

        // Yönlendirme modal state
        private bool isYonlendirmeModalOpen;
        private SiraCagirmaResponseDto? yonlendirmeIcinSecilenSira;
        private string? selectedYonlendirmeTipiValue;
        private string? selectedBankoId;
        private string? selectedUzmanPersonelTc;
        private string yonlendirmeNotu = string.Empty;

        private List<SelectOption> yonlendirmeTipiOptions = new();
        private List<SelectOption> bankoOptions = new();
        private List<SelectOption> uzmanPersonelOptions = new();

        private string HeaderBackground => IsPinned
            ? "linear-gradient(135deg, #696cff 0%, #5f61e6 100%)"
            : "linear-gradient(135deg, #8b8dff 0%, #7f81f6 100%)";

        protected override void OnInitialized()
        {
            base.OnInitialized();

            yonlendirmeTipiOptions = Enum.GetValues(typeof(YonlendirmeTipi))
                .Cast<YonlendirmeTipi>()
                .Where(e => e != YonlendirmeTipi.UzmanPersonel)
                .Select(y => new SelectOption
                {
                    Label = y switch
                    {
                        YonlendirmeTipi.BaskaBanko => "Başka Bankoya",
                        YonlendirmeTipi.Sef => "Şef / Yetkili Masasına",
                        YonlendirmeTipi.UzmanPersonel => "Uzman Personel",
                        _ => y.ToString()
                    },
                    Value = ((int)y).ToString()
                })
                .ToList();

            // TODO: Servislerden gerçek veriler bağlanacak. Şimdilik dummy değerler.
            bankoOptions = new List<SelectOption>
            {
                new SelectOption { Label = "Banko 1 - Ahmet Yılmaz", Value = "1" },
                new SelectOption { Label = "Banko 2 - Ayşe Demir", Value = "2" }
            };

            uzmanPersonelOptions = new List<SelectOption>
            {
                new SelectOption { Label = "Uzman - Mehmet Kaya", Value = "11111111111" },
                new SelectOption { Label = "Uzman - Elif Çetin", Value = "22222222222" }
            };
        }

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

        private YonlendirmeTipi? SelectedYonlendirmeTipi
        {
            get
            {
                if (int.TryParse(selectedYonlendirmeTipiValue, out var value) && Enum.IsDefined(typeof(YonlendirmeTipi), value))
                {
                    return (YonlendirmeTipi)value;
                }

                return null;
            }
        }

        private bool CanSubmitYonlendirme => SelectedYonlendirmeTipi switch
        {
            null => false,
            YonlendirmeTipi.BaskaBanko => !string.IsNullOrWhiteSpace(selectedBankoId),
            YonlendirmeTipi.UzmanPersonel => !string.IsNullOrWhiteSpace(selectedUzmanPersonelTc),
            _ => true
        };

        private void OpenYonlendirmeModal(SiraCagirmaResponseDto sira)
        {
            yonlendirmeIcinSecilenSira = sira;
            selectedYonlendirmeTipiValue = null;
            selectedBankoId = null;
            selectedUzmanPersonelTc = null;
            yonlendirmeNotu = string.Empty;
            isYonlendirmeModalOpen = true;
            StateHasChanged();
        }

        private void CloseYonlendirmeModal()
        {
            isYonlendirmeModalOpen = false;
            yonlendirmeIcinSecilenSira = null;
            StateHasChanged();
        }

        private async Task SubmitYonlendirmeAsync()
        {
            if (!CanSubmitYonlendirme || yonlendirmeIcinSecilenSira == null)
            {
                return;
            }

            // TODO: Servise bağlanacak. Şimdilik loglayıp kapatıyoruz.
            Console.WriteLine($"Yonlendirme isteği gönderildi: Sıra #{yonlendirmeIcinSecilenSira.SiraNo}, Tip: {SelectedYonlendirmeTipi}, Banko: {selectedBankoId}, Uzman: {selectedUzmanPersonelTc}, Not: {yonlendirmeNotu}");

            await Task.Delay(100); // UI feedback için küçük gecikme
            CloseYonlendirmeModal();
        }

        private class SelectOption
        {
            public string Label { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
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
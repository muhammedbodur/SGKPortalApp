using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class SiraCagirmaPanel : IDisposable
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISiraYonlendirmeApiService YonlendirmeApiService { get; set; } = default!;

        [Parameter] public List<SiraCagirmaResponseDto> SiraListesi { get; set; } = new();
        [Parameter] public EventCallback<int> OnSiraCagir { get; set; }
        [Parameter] public EventCallback<bool> OnPanelStateChanged { get; set; }
        [Parameter] public int AktifBankoId { get; set; }
        [Parameter] public string PersonelTcKimlikNo { get; set; } = string.Empty;

        private DotNetObjectReference<SiraCagirmaPanel>? dotNetReference;
        private bool IsVisible { get; set; } = false;
        private bool IsPinned { get; set; } = false;

        // Yönlendirme modal state
        private bool isYonlendirmeModalOpen;
        private bool isYonlendirmeSubmitting;
        private bool isLoadingOptions;
        private string? yonlendirmeErrorMessage;
        private SiraCagirmaResponseDto? yonlendirmeIcinSecilenSira;
        private string? selectedYonlendirmeTipiValue;
        private string? selectedBankoId;
        private string? selectedUzmanPersonelTc;
        private string yonlendirmeNotu = string.Empty;

        private List<SelectOption> yonlendirmeTipiOptions = new();
        private List<SelectOption> bankoOptions = new();

        private string HeaderBackground => IsPinned
            ? "linear-gradient(135deg, #696cff 0%, #5f61e6 100%)"
            : "linear-gradient(135deg, #8b8dff 0%, #7f81f6 100%)";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // Seçenekler modal açıldığında dinamik olarak yüklenecek
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

        /// <summary>
        /// Yönlendirme modalını açar ve mevcut seçenekleri API'den dinamik olarak çeker
        /// Bu sayede sadece aktif durumda olan personel/bankolar gösterilir
        /// </summary>
        private async Task OpenYonlendirmeModal(SiraCagirmaResponseDto sira)
        {
            yonlendirmeIcinSecilenSira = sira;
            selectedYonlendirmeTipiValue = null;
            selectedBankoId = null;
            selectedUzmanPersonelTc = null;
            yonlendirmeNotu = string.Empty;
            yonlendirmeErrorMessage = null;
            isYonlendirmeSubmitting = false;
            isLoadingOptions = true;
            isYonlendirmeModalOpen = true;

            // Önceki seçenekleri temizle
            yonlendirmeTipiOptions.Clear();
            bankoOptions.Clear();

            StateHasChanged();

            try
            {
                // API'den mevcut yönlendirme seçeneklerini çek
                var optionsResult = await YonlendirmeApiService.GetYonlendirmeSecenekleriAsync(sira.SiraId, AktifBankoId);

                if (optionsResult.Success && optionsResult.Data != null)
                {
                    var options = optionsResult.Data;

                    // Mevcut tiplere göre yönlendirme tipi seçeneklerini doldur
                    yonlendirmeTipiOptions = options.AvailableTypes
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

                    // Banko seçeneklerini doldur
                    bankoOptions = options.Bankolar
                        .Select(b => new SelectOption
                        {
                            Label = b.DisplayText,
                            Value = b.BankoId.ToString()
                        })
                        .ToList();

                    // Hiç seçenek yoksa kullanıcıya bildir
                    if (!yonlendirmeTipiOptions.Any())
                    {
                        yonlendirmeErrorMessage = "Bu sıra için yönlendirme seçeneği bulunmuyor. Aktif personel/banko bulunamadı.";
                    }

                    Console.WriteLine($"✅ Yönlendirme seçenekleri yüklendi - {yonlendirmeTipiOptions.Count} tip, {bankoOptions.Count} banko");
                }
                else
                {
                    yonlendirmeErrorMessage = optionsResult.Message ?? "Yönlendirme seçenekleri yüklenemedi";
                    Console.WriteLine($"❌ Yönlendirme seçenekleri hatası: {yonlendirmeErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                yonlendirmeErrorMessage = $"Seçenekler yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"❌ Yönlendirme seçenekleri exception: {ex.Message}");
            }
            finally
            {
                isLoadingOptions = false;
                StateHasChanged();
            }
        }

        private void CloseYonlendirmeModal()
        {
            isYonlendirmeModalOpen = false;
            yonlendirmeIcinSecilenSira = null;
            StateHasChanged();
        }

        private async Task SubmitYonlendirmeAsync()
        {
            if (!CanSubmitYonlendirme || yonlendirmeIcinSecilenSira == null || SelectedYonlendirmeTipi == null)
            {
                return;
            }

            isYonlendirmeSubmitting = true;
            yonlendirmeErrorMessage = null;
            StateHasChanged();

            try
            {
                // Hedef banko ID'yi belirle
                int hedefBankoId;
                if (SelectedYonlendirmeTipi == YonlendirmeTipi.BaskaBanko)
                {
                    if (!int.TryParse(selectedBankoId, out hedefBankoId))
                    {
                        yonlendirmeErrorMessage = "Geçersiz banko seçimi";
                        return;
                    }
                }
                else
                {
                    // Şef veya Uzman Personel için hedef banko ID şimdilik 0
                    // TODO: Gerçek senaryoda Şef/Uzman masalarının banko ID'leri kullanılabilir
                    hedefBankoId = AktifBankoId;
                }

                var request = new SiraYonlendirmeDto
                {
                    SiraId = yonlendirmeIcinSecilenSira.SiraId,
                    YonlendirenPersonelTc = PersonelTcKimlikNo,
                    YonlendirenBankoId = AktifBankoId,
                    HedefBankoId = hedefBankoId,
                    YonlendirmeTipi = SelectedYonlendirmeTipi.Value,
                    YonlendirmeNedeni = string.IsNullOrWhiteSpace(yonlendirmeNotu) ? null : yonlendirmeNotu
                };

                var result = await YonlendirmeApiService.YonlendirSiraAsync(request);

                if (result.Success)
                {
                    Console.WriteLine($"✅ Sıra başarıyla yönlendirildi: #{yonlendirmeIcinSecilenSira.SiraNo}");
                    CloseYonlendirmeModal();

                    // Parent component'i güncelle (sıra listesini yenile)
                    await OnPanelStateChanged.InvokeAsync(true);
                }
                else
                {
                    yonlendirmeErrorMessage = result.Message ?? "Yönlendirme işlemi başarısız oldu";
                    Console.WriteLine($"❌ Yönlendirme hatası: {yonlendirmeErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                yonlendirmeErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"❌ Yönlendirme exception: {ex.Message}");
            }
            finally
            {
                isYonlendirmeSubmitting = false;
                StateHasChanged();
            }
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
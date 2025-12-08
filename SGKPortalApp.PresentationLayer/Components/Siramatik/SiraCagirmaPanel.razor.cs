using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System.Linq;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class SiraCagirmaPanel : IDisposable
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISiraYonlendirmeApiService YonlendirmeApiService { get; set; } = default!;
        [Inject] private ISiraCagirmaApiService SiraCagirmaApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

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
        private bool isCallingNext;

        private string HeaderBackground => IsPinned
            ? "linear-gradient(135deg, #696cff 0%, #5f61e6 100%)"
            : "linear-gradient(135deg, #8b8dff 0%, #7f81f6 100%)";

        private static string GetUzmanlikBadgeClass(PersonelUzmanlik uzmanlik) => uzmanlik switch
        {
            PersonelUzmanlik.Sef => "bg-danger-subtle text-white",
            PersonelUzmanlik.Uzman => "bg-success-subtle text-white",
            PersonelUzmanlik.YrdUzman => "bg-info-subtle text-white",
            _ => "bg-secondary text-white"
        };

        private SiraCagirmaResponseDto? FirstCallableSira => SiraListesi.FirstOrDefault(IsCallableSira);
        private int? FirstCallableSiraId => FirstCallableSira?.SiraId;

        private static bool IsCallableSira(SiraCagirmaResponseDto? sira)
            => sira != null && (sira.BeklemeDurum == BeklemeDurum.Yonlendirildi || sira.BeklemeDurum == BeklemeDurum.Beklemede);

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

        /// <summary>
        /// SignalR'dan gelen sıra güncellemelerini işle (JS'den çağrılır)
        /// UpdateType: 1=Append, 2=Remove, 3=Insert, 5=Update
        /// </summary>
        [JSInvokable]
        public async Task OnSiraUpdateFromSignalR(object payload)
        {
            try
            {
                Console.WriteLine($"📥 OnSiraUpdateFromSignalR çağrıldı: {payload}");

                // Payload'ı parse et
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(payload.ToString() ?? "{}");
                
                var updateType = jsonElement.TryGetProperty("updateType", out var updateTypeProp) 
                    ? updateTypeProp.GetInt32() 
                    : 0;

                // Açıklama (yönlendirme için)
                var aciklama = jsonElement.TryGetProperty("aciklama", out var aciklamaProp) 
                    ? aciklamaProp.GetString() 
                    : null;

                if (jsonElement.TryGetProperty("sira", out var siraProp))
                {
                    var siraJson = siraProp.GetRawText();
                    var yeniSira = System.Text.Json.JsonSerializer.Deserialize<SiraCagirmaResponseDto>(siraJson, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (yeniSira != null)
                    {
                        // UpdateType: 1 = Append (yeni sıra ekle - sona)
                        if (updateType == 1)
                        {
                            // Aynı sıra zaten listede var mı kontrol et
                            if (!SiraListesi.Any(s => s.SiraId == yeniSira.SiraId))
                            {
                                SiraListesi.Add(yeniSira);
                                Console.WriteLine($"✅ Yeni sıra eklendi: #{yeniSira.SiraNo} (ID: {yeniSira.SiraId})");
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Sıra zaten listede: #{yeniSira.SiraNo}");
                            }
                        }
                        // UpdateType: 2 = Remove (sırayı kaldır)
                        else if (updateType == 2)
                        {
                            var silinecek = SiraListesi.FirstOrDefault(s => s.SiraId == yeniSira.SiraId);
                            if (silinecek != null)
                            {
                                SiraListesi.Remove(silinecek);
                                Console.WriteLine($"✅ Sıra kaldırıldı: #{yeniSira.SiraNo}");
                            }
                        }
                        // UpdateType: 3 = Insert (belirli pozisyona ekle - yönlendirme)
                        else if (updateType == 3)
                        {
                            // Aynı sıra zaten listede var mı kontrol et
                            if (!SiraListesi.Any(s => s.SiraId == yeniSira.SiraId))
                            {
                                // Komşu sıra ID'lerini al
                                var previousSiraId = jsonElement.TryGetProperty("previousSiraId", out var prevProp) && prevProp.ValueKind != System.Text.Json.JsonValueKind.Null
                                    ? prevProp.GetInt32() : (int?)null;
                                var nextSiraId = jsonElement.TryGetProperty("nextSiraId", out var nextProp) && nextProp.ValueKind != System.Text.Json.JsonValueKind.Null
                                    ? nextProp.GetInt32() : (int?)null;
                                var position = jsonElement.TryGetProperty("position", out var posProp) ? posProp.GetInt32() : 0;

                                // Pozisyon belirleme - tüm ihtimaller
                                int insertIndex = CalculateInsertIndex(previousSiraId, nextSiraId, position);

                                // Güvenli ekleme
                                insertIndex = Math.Max(0, Math.Min(insertIndex, SiraListesi.Count));
                                SiraListesi.Insert(insertIndex, yeniSira);
                                
                                Console.WriteLine($"✅ Yönlendirilmiş sıra eklendi: #{yeniSira.SiraNo} (Index: {insertIndex}, Prev: {previousSiraId}, Next: {nextSiraId})");

                                // Toast bildirimi göster
                                await ToastService.ShowInfoAsync($"Sıra #{yeniSira.SiraNo} size yönlendirildi", "Yeni Yönlendirme");
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ Sıra zaten listede: #{yeniSira.SiraNo}");
                            }
                        }
                        // UpdateType: 5 = Update (mevcut sırayı güncelle)
                        else if (updateType == 5)
                        {
                            var mevcutSira = SiraListesi.FirstOrDefault(s => s.SiraId == yeniSira.SiraId);
                            if (mevcutSira != null)
                            {
                                var index = SiraListesi.IndexOf(mevcutSira);
                                SiraListesi[index] = yeniSira;
                                Console.WriteLine($"✅ Sıra güncellendi: #{yeniSira.SiraNo}");
                            }
                        }

                        await InvokeAsync(StateHasChanged);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OnSiraUpdateFromSignalR error: {ex.Message}");
            }
        }

        /// <summary>
        /// ⭐ Banko Panel Sıra Güncellemesi (Kiosk sıra alma veya yönlendirme sonrası)
        /// Sadece yeni/değişen sıra ve pozisyon bilgisi gelir (tüm liste değil!)
        /// Payload: { siraId, personelTc, sira: {...}, pozisyon: int, toplamSiraSayisi: int, timestamp }
        /// </summary>
        [JSInvokable]
        public async Task OnBankoPanelGuncellemesiFromSignalR(object payload)
        {
            try
            {
                Console.WriteLine($"📥 OnBankoPanelGuncellemesiFromSignalR çağrıldı");

                // Payload'ı parse et
                var jsonElement = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(payload.ToString() ?? "{}");

                // Sıra ID ve pozisyon bilgilerini al
                var siraId = jsonElement.TryGetProperty("siraId", out var siraIdProp) ? siraIdProp.GetInt32() : 0;
                var pozisyon = jsonElement.TryGetProperty("pozisyon", out var pozisyonProp) ? pozisyonProp.GetInt32() : -1;
                var toplamSiraSayisi = jsonElement.TryGetProperty("toplamSiraSayisi", out var toplamProp) ? toplamProp.GetInt32() : 0;

                Console.WriteLine($"📋 SiraId: {siraId}, Pozisyon: {pozisyon}, Toplam: {toplamSiraSayisi}, Mevcut: {SiraListesi.Count}");

                // Sıra bilgisini parse et
                if (jsonElement.TryGetProperty("sira", out var siraProp) && siraProp.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    var siraJson = siraProp.GetRawText();
                    var yeniSira = System.Text.Json.JsonSerializer.Deserialize<SiraCagirmaResponseDto>(siraJson, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (yeniSira != null)
                    {
                        // Mevcut listede bu sıra var mı kontrol et
                        var mevcutIndex = SiraListesi.FindIndex(s => s.SiraId == yeniSira.SiraId);

                        if (mevcutIndex >= 0)
                        {
                            // ⭐ Sıra zaten var - güncelle (durum değişmiş olabilir)
                            SiraListesi[mevcutIndex] = yeniSira;
                            Console.WriteLine($"🔄 Sıra güncellendi: #{yeniSira.SiraNo}");
                        }
                        else
                        {
                            // ⭐ Yeni sıra - doğru pozisyona ekle
                            if (pozisyon >= 0 && pozisyon <= SiraListesi.Count)
                            {
                                SiraListesi.Insert(pozisyon, yeniSira);
                                Console.WriteLine($"✅ Yeni sıra eklendi: #{yeniSira.SiraNo} (pozisyon: {pozisyon})");
                            }
                            else
                            {
                                // Pozisyon geçersizse sona ekle
                                SiraListesi.Add(yeniSira);
                                Console.WriteLine($"✅ Yeni sıra sona eklendi: #{yeniSira.SiraNo}");
                            }
                        }

                        await InvokeAsync(StateHasChanged);
                    }
                }
                else
                {
                    // Sıra null geldi - muhtemelen kaldırılması gerekiyor
                    var silinecek = SiraListesi.FirstOrDefault(s => s.SiraId == siraId);
                    if (silinecek != null)
                    {
                        SiraListesi.Remove(silinecek);
                        Console.WriteLine($"🗑️ Sıra kaldırıldı: #{silinecek.SiraNo}");
                        await InvokeAsync(StateHasChanged);
                    }
                }

                Console.WriteLine($"✅ Liste güncellendi. Yeni sıra sayısı: {SiraListesi.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ OnBankoPanelGuncellemesiFromSignalR error: {ex.Message}");
            }
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
            if (isCallingNext)
            {
                return;
            }

            var paneldekiIlkSira = FirstCallableSira;

            if (paneldekiIlkSira == null)
            {
                await ToastService.ShowInfoAsync("Çağrılacak bekleyen sıra bulunamadı.", "Sıra Çağırma");
                return;
            }

            isCallingNext = true;
            StateHasChanged();

            try
            {
                // ⭐ ADIM 1: Backend'den SADECE ilk çağrılabilir sırayı al (performans için)
                var backendIlkSira = await SiraCagirmaApiService.GetIlkCagrilabilirSiraAsync(PersonelTcKimlikNo);

                // Backend'deki ilk çağrılabilir sıra ile paneldeki farklı mı?
                if (backendIlkSira == null)
                {
                    // Backend'de çağrılabilir sıra yok - TÜM listeyi çek ve paneli güncelle
                    var guncelListe = await SiraCagirmaApiService.GetBankoPanelSiralarAsync(PersonelTcKimlikNo);
                    await RefreshPanelAsync(guncelListe);
                    await ToastService.ShowInfoAsync("Çağrılacak bekleyen sıra bulunamadı.", "Sıra Çağırma");
                    return;
                }

                if (backendIlkSira.SiraId != paneldekiIlkSira.SiraId)
                {
                    // ⭐ ADIM 2: Sıralar uyuşmuyor - TÜM listeyi çek ve paneli güncelle
                    Console.WriteLine($"⚠️ Sıra uyuşmazlığı! Panel: #{paneldekiIlkSira.SiraNo}, Backend: #{backendIlkSira.SiraNo}");
                    var guncelListe = await SiraCagirmaApiService.GetBankoPanelSiralarAsync(PersonelTcKimlikNo);
                    await RefreshPanelAsync(guncelListe);
                    await ToastService.ShowWarningAsync(
                        $"Sıra listesi güncellendi. Yeni ilk sıra: #{backendIlkSira.SiraNo}", 
                        "Sıra Güncellendi");
                    return;
                }

                // ⭐ ADIM 3: Sıralar uyuşuyor - çağırma işlemini yap
                var response = await SiraCagirmaApiService.SiradakiCagirAsync(
                    backendIlkSira.SiraId, 
                    PersonelTcKimlikNo, 
                    AktifBankoId,
                    null,
                    backendIlkSira.SiraId);

                if (response != null)
                {
                    // ⭐ Önceki çağrılmış sıraları listeden kaldır (artık Bitti durumunda)
                    var oncekiCagrilanlar = SiraListesi
                        .Where(s => s.BeklemeDurum == BeklemeDurum.Cagrildi && s.SiraId != backendIlkSira.SiraId)
                        .ToList();
                    foreach (var onceki in oncekiCagrilanlar)
                    {
                        SiraListesi.Remove(onceki);
                        Console.WriteLine($"✅ Önceki çağrılan sıra listeden kaldırıldı: #{onceki.SiraNo}");
                    }

                    // ⭐ Yeni çağrılan sıranın durumunu güncelle (listede kalsın, sadece durum değişsin)
                    var cagrilanSira = SiraListesi.FirstOrDefault(s => s.SiraId == backendIlkSira.SiraId);
                    if (cagrilanSira != null)
                    {
                        cagrilanSira.BeklemeDurum = BeklemeDurum.Cagrildi;
                        Console.WriteLine($"✅ Sıra durumu güncellendi: #{cagrilanSira.SiraNo} -> Çağrıldı");
                    }

                    await OnSiraCagir.InvokeAsync(backendIlkSira.SiraId);
                    await ToastService.ShowSuccessAsync($"Sıra #{response.SiraNo} çağrıldı.", "Sıra Çağırma");
                }
                else
                {
                    await ToastService.ShowErrorAsync("Sıra çağırma işlemi başarısız oldu.", "Sıra Çağırma");
                }
            }
            catch (InvalidOperationException ex)
            {
                // Concurrency hatası - TÜM listeyi çek ve paneli yenile
                var guncelListe = await SiraCagirmaApiService.GetBankoPanelSiralarAsync(PersonelTcKimlikNo);
                await RefreshPanelAsync(guncelListe);
                
                var message = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Sıra listesi güncellendi."
                    : ex.Message;
                await ToastService.ShowWarningAsync(message, "Sıra Çağırma");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SiradakiCagir error: {ex.Message}");
                await ToastService.ShowErrorAsync("Sıra çağırılırken beklenmeyen bir hata oluştu.", "Sıra Çağırma");
            }
            finally
            {
                isCallingNext = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Paneli backend'den gelen güncel liste ile yeniler
        /// </summary>
        private async Task RefreshPanelAsync(List<SiraCagirmaResponseDto> guncelListe)
        {
            SiraListesi.Clear();
            SiraListesi.AddRange(guncelListe);
            await InvokeAsync(StateHasChanged);
            Console.WriteLine($"🔄 Panel yenilendi. Yeni sıra sayısı: {SiraListesi.Count}");
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

        /// <summary>
        /// Yönlendirilen sıranın ekleneceği pozisyonu hesaplar.
        /// Tüm ihtimalleri ele alır:
        /// 1. previousSiraId var, nextSiraId var → İkisinin arasına
        /// 2. previousSiraId var, nextSiraId yok → previousSiraId'nin sonrasına
        /// 3. previousSiraId yok, nextSiraId var → nextSiraId'nin öncesine
        /// 4. İkisi de yok → position değerine göre
        /// 5. previousSiraId var ama listede yok → nextSiraId'ye bak
        /// 6. nextSiraId var ama listede yok → previousSiraId'ye bak
        /// 7. İkisi de listede yok → position değerine göre
        /// </summary>
        private int CalculateInsertIndex(int? previousSiraId, int? nextSiraId, int fallbackPosition)
        {
            int prevIndex = -1;
            int nextIndex = -1;

            // Komşu sıraların mevcut listedeki indexlerini bul
            if (previousSiraId.HasValue)
            {
                prevIndex = SiraListesi.FindIndex(s => s.SiraId == previousSiraId.Value);
            }
            if (nextSiraId.HasValue)
            {
                nextIndex = SiraListesi.FindIndex(s => s.SiraId == nextSiraId.Value);
            }

            // Senaryo 1: Her iki komşu da listede var
            if (prevIndex >= 0 && nextIndex >= 0)
            {
                // İkisinin arasına ekle (prev'in hemen sonrasına)
                Console.WriteLine($"📍 Senaryo 1: İkisi de var. Prev={prevIndex}, Next={nextIndex}");
                return prevIndex + 1;
            }

            // Senaryo 2: Sadece nextSiraId listede var
            if (nextIndex >= 0)
            {
                Console.WriteLine($"📍 Senaryo 3: Sadece next var. Next={nextIndex}");
                return nextIndex;
            }

            // Senaryo 3: Sadece previousSiraId listede var
            if (prevIndex >= 0)
            {
                Console.WriteLine($"📍 Senaryo 2: Sadece prev var. Prev={prevIndex}");
                return prevIndex + 1;
            }

            // Senaryo 4: İkisi de yok veya listede bulunamadı - fallback position kullan
            Console.WriteLine($"📍 Senaryo 4: Hiçbiri yok. Fallback position={fallbackPosition}");
            return Math.Min(fallbackPosition, SiraListesi.Count);
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
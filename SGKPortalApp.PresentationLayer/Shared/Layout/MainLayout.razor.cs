using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using SGKPortalApp.PresentationLayer.Services.State;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class MainLayout : IAsyncDisposable, IDisposable
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IBankoModeService? BankoModeService { get; set; }
        [Inject] private IHttpContextAccessor? HttpContextAccessor { get; set; }
        [Inject] private BankoModeStateService BankoModeState { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IUserInfoService UserInfoService { get; set; } = default!;
        [Inject] private IUserApiService UserApiService { get; set; } = default!;

        private List<SiraCagirmaResponseDto> siraListesi = new();
        private bool siraPanelAcik = false;
        private DotNetObjectReference<MainLayout>? dotNetHelper;

        protected override void OnInitialized()
        {
            // TODO: Gerçek uygulamada service/SignalR'dan gelecek
            OrnekSiraVerileriYukle();
            
            // URL değişikliklerini dinle (Banko modu kontrolü için)
            NavigationManager.LocationChanged += OnLocationChanged;
            
            // Banko modu state değişikliklerini dinle
            BankoModeState.OnBankoModeChanged += OnBankoModeStateChanged;
            
            // İlk yüklemede kontrol et
            CheckBankoModeAccess();
        }
        
        private void OnBankoModeStateChanged()
        {
            // State değiştiğinde UI'ı güncelle
            InvokeAsync(StateHasChanged);
        }

        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            CheckBankoModeAccess();
            InvokeAsync(CheckSessionValidityAsync); // Session kontrolü yap (async olarak)
        }

        private async Task CheckSessionValidityAsync()
        {
            try
            {
                var currentSessionId = UserInfoService.GetSessionId();
                var tcKimlikNo = UserInfoService.GetTcKimlikNo();

                if (string.IsNullOrEmpty(currentSessionId) || string.IsNullOrEmpty(tcKimlikNo))
                    return;

                // Database'den kullanıcının aktif session ID'sini al
                var userResult = await UserApiService.GetByTcKimlikNoAsync(tcKimlikNo);

                if (userResult.Success && userResult.Data != null)
                {
                    var dbSessionId = userResult.Data.SessionID;

                    // Session ID'ler farklıysa başka bir cihazdan login olunmuş demektir
                    if (!string.IsNullOrEmpty(dbSessionId) && dbSessionId != currentSessionId)
                    {
                        Console.WriteLine($"⚠️ Session uyuşmazlığı! Cookie: {currentSessionId}, DB: {dbSessionId}");
                        
                        // Hem Blazor hem JavaScript ile yönlendir (çift güvence)
                        NavigationManager.NavigateTo("/auth/login?sessionExpired=true", forceLoad: true);
                        
                        // JavaScript ile de yönlendir (fallback)
                        try
                        {
                            await JS.InvokeVoidAsync("eval", "setTimeout(() => window.location.href = '/auth/login?sessionExpired=true', 100);");
                        }
                        catch { /* Ignore JS errors */ }
                        
                        return; // Metodu sonlandır
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Session kontrolü hatası: {ex.Message}");
            }
        }

        private void CheckBankoModeAccess()
        {
            var currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
            
            // Banko modunda mı?
            if (BankoModeState.IsInBankoMode && !string.IsNullOrEmpty(tcKimlikNo))
            {
                // Bu personel banko modunda mı?
                if (BankoModeState.IsPersonelInBankoMode(tcKimlikNo))
                {
                    // Dashboard dışında bir sayfaya gitmeye çalışıyor mu?
                    if (!currentUrl.Equals("", StringComparison.OrdinalIgnoreCase) && 
                        !currentUrl.Equals("siramatik/dashboard", StringComparison.OrdinalIgnoreCase))
                    {
                        // İzin yok, Dashboard'a geri yönlendir
                        NavigationManager.NavigateTo("/siramatik/dashboard", forceLoad: true);
                    }
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(500);

                try
                {
                    await JS.InvokeVoidAsync("initSneatMenu");
                    
                    // SignalR event handler'larını kur
                    dotNetHelper = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("bankoMode.setupEventHandlers", dotNetHelper);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MainLayout JS hatası: {ex.Message}");
                }
            }
        }

        private void OrnekSiraVerileriYukle()
        {
            // Örnek veri - Gerçek uygulamada service'den gelecek
            siraListesi = new List<SiraCagirmaResponseDto>
            {
                new() { SiraId = 1, SiraNo = 1, KanalAltAdi = "Emeklilik İşlemleri", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 2, SiraNo = 2, KanalAltAdi = "SGK Kayıt", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 3, SiraNo = 3, KanalAltAdi = "Sağlık Raporu", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" },
                new() { SiraId = 4, SiraNo = 4, KanalAltAdi = "Borç Sorgulama", BeklemeDurum = BeklemeDurum.Beklemede, SiraAlisZamani = DateTime.Now, HizmetBinasiId = 1, HizmetBinasiAdi = "İzmir SGK" }
            };
        }

        private void HandleSiraCagir(int siraId)
        {
            // Sırayı çağır
            var sira = siraListesi.FirstOrDefault(x => x.SiraId == siraId);
            if (sira != null)
            {
                // Önceki çağrılmış sırayı beklemede yap
                var oncekiCagrilan = siraListesi.FirstOrDefault(x => x.BeklemeDurum == BeklemeDurum.Cagrildi);
                if (oncekiCagrilan != null)
                {
                    oncekiCagrilan.BeklemeDurum = BeklemeDurum.Beklemede;
                }

                // Yeni sırayı çağır
                sira.BeklemeDurum = BeklemeDurum.Cagrildi;
                sira.IslemBaslamaZamani = DateTime.Now;

                // TODO: Gerçek uygulamada API'ye çağrı yapılacak
                // await SiraService.CagirAsync(siraId);

                StateHasChanged();
            }
        }

        private void HandlePanelStateChanged(bool isVisible)
        {
            siraPanelAcik = isVisible;
            StateHasChanged();
        }

        // ═══════════════════════════════════════════════════════
        // BANKO MODE - JavaScript'ten çağrılacak metodlar
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Banko moduna geç (C# tarafından çağrılır)
        /// </summary>
        public async Task<bool> EnterBankoModeAsync(int bankoId)
        {
            try
            {
                var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Console.WriteLine("❌ Kullanıcı bilgisi bulunamadı");
                    return false;
                }

                // 1. Kontroller (C# tarafında)
                if (BankoModeService != null)
                {
                    var bankoInUse = await BankoModeService.IsBankoInUseAsync(bankoId);
                    if (bankoInUse)
                    {
                        var activePersonel = await BankoModeService.GetBankoActivePersonelNameAsync(bankoId);
                        Console.WriteLine($"❌ Banko#{bankoId} kullanımda: {activePersonel}");
                        return false;
                    }
                }

                // NOT: Bu metod artık kullanılmıyor
                // BankoModeWidget direkt C# ile işlem yapıyor
                Console.WriteLine("⚠️ EnterBankoModeAsync çağrıldı ama artık kullanılmıyor");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EnterBankoModeAsync hatası: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Banko modundan çık (C# tarafından çağrılır)
        /// </summary>
        public async Task<bool> ExitBankoModeAsync()
        {
            // NOT: Bu metod artık kullanılmıyor
            // BankoModeWidget direkt C# ile işlem yapıyor
            Console.WriteLine("⚠️ ExitBankoModeAsync çağrıldı ama artık kullanılmıyor");
            await Task.CompletedTask;
            return false;
        }

        /// <summary>
        /// JavaScript'ten çağrılır: Banko modu aktif oldu
        /// </summary>
        [JSInvokable]
        public async Task OnBankoModeActivated(int bankoId)
        {
            Console.WriteLine($"✅ MainLayout - Banko modu aktif: Banko#{bankoId}");
            await Task.CompletedTask;
            // UI güncellemesi gerekirse StateHasChanged() çağrılabilir
        }

        /// <summary>
        /// JavaScript'ten çağrılır: Banko modu deaktif oldu
        /// </summary>
        [JSInvokable]
        public async Task OnBankoModeDeactivated()
        {
            Console.WriteLine("✅ MainLayout - Banko modu deaktif");
            await Task.CompletedTask;
            // UI güncellemesi gerekirse StateHasChanged() çağrılabilir
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            BankoModeState.OnBankoModeChanged -= OnBankoModeStateChanged;
        }

        public async ValueTask DisposeAsync()
        {
            dotNetHelper?.Dispose();
            Dispose();
            await Task.CompletedTask;
        }
    }
}

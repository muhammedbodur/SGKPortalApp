using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using SGKPortalApp.PresentationLayer.Services.State;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System;
using System.Threading;

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
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private ILogger<MainLayout> Logger { get; set; } = default!;

        // ✅ CascadingParameter kullan (AuthorizeRouteView'dan gelir)
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        private List<SiraCagirmaResponseDto> siraListesi = new();
        private bool siraPanelAcik = false;
        private DotNetObjectReference<MainLayout>? dotNetHelper;

        // ✅ Session check için cache
        private DateTime _lastSessionCheck = DateTime.MinValue;
        private readonly TimeSpan _sessionCheckInterval = TimeSpan.FromSeconds(30); // 30 saniyede bir kontrol et

        // ✅ CancellationToken
        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _cts = new CancellationTokenSource();

                Logger.LogDebug("🔵 MainLayout.OnInitializedAsync başladı");

                // 1. Authentication kontrolü (CascadingParameter'dan)
                if (!await IsAuthenticatedAsync())
                {
                    Logger.LogWarning("⚠️ Kullanıcı authenticated değil - login'e yönlendiriliyor");
                    NavigationManager.NavigateTo("/auth/login", forceLoad: true);
                    return;
                }

                // 2. İlk session kontrolü
                await CheckSessionValidityThrottledAsync();

                // 3. Diğer initialization'lar
                OrnekSiraVerileriYukle();

                // 4. Event listener'ları kaydet
                NavigationManager.LocationChanged += OnLocationChanged;
                BankoModeState.OnBankoModeChanged += OnBankoModeStateChanged;

                // 5. İlk kontroller
                CheckBankoModeAccess();

                Logger.LogInformation("✅ MainLayout başarıyla initialize edildi");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ MainLayout initialization hatası");
                NavigationManager.NavigateTo("/auth/login", forceLoad: true);
            }
        }

        /// <summary>
        /// ✅ CascadingParameter kullanarak authentication kontrolü
        /// AuthorizeRouteView zaten kontrol ediyor, biz sadece doğruluyoruz
        /// </summary>
        private async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                // CascadingParameter varsa kullan, yoksa provider'dan al
                var authState = AuthenticationState != null
                    ? await AuthenticationState
                    : await AuthStateProvider.GetAuthenticationStateAsync();

                var user = authState.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    Logger.LogWarning("⚠️ User authenticated değil");
                    return false;
                }

                // TcKimlikNo claim kontrolü
                var tcKimlikNo = user.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogWarning("⚠️ TcKimlikNo claim'i bulunamadı");
                    return false;
                }

                var adSoyad = user.FindFirst("AdSoyad")?.Value ?? "Unknown";
                Logger.LogDebug("✅ Kullanıcı authenticated: {AdSoyad} ({TcKimlikNo})", adSoyad, tcKimlikNo);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ IsAuthenticatedAsync hatası");
                return false;
            }
        }

        private void OnBankoModeStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ✅ Navigation event'i - Throttled kontrol
        /// </summary>
        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            Logger.LogDebug("🔵 Location changed: {Location}", e.Location);

            // ✅ InvokeAsync ile Blazor thread'inde çalıştır + CancellationToken
            _ = InvokeAsync(async () =>
            {
                try
                {
                    // CancellationToken kontrolü
                    if (_cts?.Token.IsCancellationRequested == true)
                        return;

                    // 1. Authentication kontrolü
                    if (!await IsAuthenticatedAsync())
                    {
                        Logger.LogWarning("⚠️ Navigation sırasında authentication kontrolü başarısız");
                        NavigationManager.NavigateTo("/auth/login", forceLoad: true);
                        return; // Early return
                    }

                    // 2. Session kontrolü (throttled - her 30 saniyede bir)
                    await CheckSessionValidityThrottledAsync();

                    // 3. Banko modu kontrolü
                    CheckBankoModeAccess();

                    // 4. UI güncelle
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "❌ OnLocationChanged hatası");
                }
            });
        }

        /// <summary>
        /// ✅ Throttled session check - Her 30 saniyede bir kontrol eder
        /// </summary>
        private async Task CheckSessionValidityThrottledAsync()
        {
            try
            {
                var now = DateTime.UtcNow;

                // Son kontrolden 30 saniye geçmemişse skip
                if (now - _lastSessionCheck < _sessionCheckInterval)
                {
                    Logger.LogDebug("⏭️ Session check skipped (throttled)");
                    return;
                }

                _lastSessionCheck = now;

                // Gerçek session kontrolü
                await CheckSessionValidityAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ CheckSessionValidityThrottledAsync hatası");
            }
        }

        /// <summary>
        /// Session ID doğrulama - Başka cihazdan login kontrolü
        /// </summary>
        private async Task CheckSessionValidityAsync()
        {
            try
            {
                var currentSessionId = UserInfoService.GetSessionId();
                var tcKimlikNo = UserInfoService.GetTcKimlikNo();

                if (string.IsNullOrEmpty(currentSessionId) || string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogWarning("⚠️ Session bilgileri eksik");
                    NavigationManager.NavigateTo("/auth/login?sessionExpired=true", forceLoad: true);
                    return;
                }

                // Database'den kullanıcının aktif session ID'sini al
                var userResult = await UserApiService.GetByTcKimlikNoAsync(tcKimlikNo);

                if (userResult.Success && userResult.Data != null)
                {
                    var dbSessionId = userResult.Data.SessionID;

                    // Session ID'ler farklıysa başka bir cihazdan login olunmuş demektir
                    if (!string.IsNullOrEmpty(dbSessionId) && dbSessionId != currentSessionId)
                    {
                        Logger.LogWarning("⚠️ Session uyuşmazlığı! Cookie: {CurrentSessionId}, DB: {DbSessionId}",
                            currentSessionId, dbSessionId);

                        // Blazor navigation
                        NavigationManager.NavigateTo("/auth/login?sessionExpired=true", forceLoad: true);

                        // JavaScript fallback
                        try
                        {
                            await JS.InvokeVoidAsync("eval",
                                "setTimeout(() => window.location.href = '/auth/login?sessionExpired=true', 100);");
                        }
                        catch { /* Ignore JS errors */ }

                        return;
                    }

                    Logger.LogDebug("✅ Session ID eşleşti - Kullanıcı geçerli");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ Session kontrolü hatası");
                // Hata durumunda güvenli tarafta kal
                NavigationManager.NavigateTo("/auth/login?error=true", forceLoad: true);
            }
        }

        private void CheckBankoModeAccess()
        {
            var currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;

            if (BankoModeState.IsInBankoMode && !string.IsNullOrEmpty(tcKimlikNo))
            {
                if (BankoModeState.IsPersonelInBankoMode(tcKimlikNo))
                {
                    if (!currentUrl.Equals("", StringComparison.OrdinalIgnoreCase) &&
                        !currentUrl.Equals("siramatik/dashboard", StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.LogWarning("⚠️ Banko modunda başka sayfaya erişim engellendi");
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

                    dotNetHelper = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("bankoMode.setupEventHandlers", dotNetHelper);

                    // SignalR ForceLogout event listener'ı ekle
                    await JS.InvokeVoidAsync("signalRManager.registerForceLogoutHandler", dotNetHelper);

                    Logger.LogDebug("✅ MainLayout JS initialization tamamlandı");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "❌ MainLayout JS initialization hatası");
                }
            }
        }

        /// <summary>
        /// JavaScript'ten çağrılır - SignalR ForceLogout event'i
        /// </summary>
        [JSInvokable]
        public void HandleForceLogout(string message)
        {
            Logger.LogWarning($"🚨 ForceLogout event alındı: {message}");
            
            // Tam sayfa yenileme ile login'e yönlendir
            NavigationManager.NavigateTo("/auth/login", forceLoad: true);
        }

        private void OrnekSiraVerileriYukle()
        {
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
            var sira = siraListesi.FirstOrDefault(x => x.SiraId == siraId);
            if (sira != null)
            {
                var oncekiCagrilan = siraListesi.FirstOrDefault(x => x.BeklemeDurum == BeklemeDurum.Cagrildi);
                if (oncekiCagrilan != null)
                {
                    oncekiCagrilan.BeklemeDurum = BeklemeDurum.Beklemede;
                }

                sira.BeklemeDurum = BeklemeDurum.Cagrildi;
                sira.IslemBaslamaZamani = DateTime.Now;

                StateHasChanged();
            }
        }

        private void HandlePanelStateChanged(bool isVisible)
        {
            siraPanelAcik = isVisible;
            StateHasChanged();
        }

        // Banko mode metodları - değişiklik yok
        public async Task<bool> EnterBankoModeAsync(int bankoId)
        {
            try
            {
                var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogWarning("❌ Kullanıcı bilgisi bulunamadı");
                    return false;
                }

                if (BankoModeService != null)
                {
                    var bankoInUse = await BankoModeService.IsBankoInUseAsync(bankoId);
                    if (bankoInUse)
                    {
                        var activePersonel = await BankoModeService.GetBankoActivePersonelNameAsync(bankoId);
                        Logger.LogWarning("❌ Banko#{BankoId} kullanımda: {ActivePersonel}", bankoId, activePersonel);
                        return false;
                    }
                }

                Logger.LogDebug("⚠️ EnterBankoModeAsync - BankoModeWidget kullanılmalı");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ EnterBankoModeAsync hatası");
                return false;
            }
        }

        public async Task<bool> ExitBankoModeAsync()
        {
            Logger.LogDebug("⚠️ ExitBankoModeAsync - BankoModeWidget kullanılmalı");
            await Task.CompletedTask;
            return false;
        }

        [JSInvokable]
        public async Task OnBankoModeActivated(int bankoId)
        {
            Logger.LogInformation("✅ MainLayout - Banko modu aktif: Banko#{BankoId}", bankoId);
            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task OnBankoModeDeactivated()
        {
            Logger.LogInformation("✅ MainLayout - Banko modu deaktif");
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            BankoModeState.OnBankoModeChanged -= OnBankoModeStateChanged;
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            dotNetHelper?.Dispose();
            Dispose();
            await Task.CompletedTask;
        }
    }
}
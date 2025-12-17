using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.State;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;

using System;
using System.Threading;

namespace SGKPortalApp.PresentationLayer.Shared.Layout
{
    public partial class MainLayout : IAsyncDisposable, IDisposable
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IHttpContextAccessor? HttpContextAccessor { get; set; }
        [Inject] private BankoModeStateService BankoModeState { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IUserInfoService UserInfoService { get; set; } = default!;
        [Inject] private IUserApiService UserApiService { get; set; } = default!;
        [Inject] private ISiraCagirmaApiService SiraCagirmaApiService { get; set; } = default!;
        [Inject] private PermissionStateService PermissionStateService { get; set; } = default!;

        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private ILogger<MainLayout> Logger { get; set; } = default!;

        // ✅ CascadingParameter kullan (AuthorizeRouteView'dan gelir)
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        private List<SiraCagirmaResponseDto> siraListesi = new();
        private bool siraPanelAcik = false;
        private DotNetObjectReference<MainLayout>? dotNetHelper;

        // ⭐ Kullanıcı TC ve Banko modu kontrolü (AsyncLocal'a bağımlı olmayan)
        private string? _tcKimlikNo;
        private bool IsInBankoMode => !string.IsNullOrEmpty(_tcKimlikNo) && BankoModeState.IsPersonelInBankoMode(_tcKimlikNo);

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

                // 1.5 ⭐ Kullanıcı TC'sini al ve sakla
                _tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (!string.IsNullOrEmpty(_tcKimlikNo))
                {
                    BankoModeState.SetCurrentUser(_tcKimlikNo);
                    // NOT: Banko modu senkronizasyonu API üzerinden yapılacak
                }

                // 2. İlk session kontrolü
                await CheckSessionValidityThrottledAsync();

                // 3. Panel verisini yalnızca banko modundaysa yükle
                await LoadBankoPanelSiralarAsync();

                // 🔑 Permission cache'i ilk açılışta yükle
                try
                {
                    await PermissionStateService.EnsureLoadedAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "PermissionStateService initial load hatası");
                }

                // 4. Event listener'ları kaydet
                NavigationManager.LocationChanged += OnLocationChanged;
                // ⭐ Kullanıcı bazlı event subscription (AsyncLocal'a bağımlı değil!)
                if (!string.IsNullOrEmpty(_tcKimlikNo))
                {
                    BankoModeState.SubscribeToUserChanges(_tcKimlikNo, OnBankoModeStateChanged);
                }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    // ⭐ JS initialization - SignalR event handler'ları kur
                    await JS.InvokeVoidAsync("initSneatMenu");

                    dotNetHelper = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("bankoMode.setupEventHandlers", dotNetHelper);

                    // NOT: ForceLogout handler'ı signalr-app-initializer.js içinde zaten kuruluyor

                    Logger.LogInformation("✅ MainLayout JS initialization tamamlandı (OnAfterRenderAsync)");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "❌ MainLayout JS initialization hatası");
                }
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
            _ = InvokeAsync(async () =>
            {
                await LoadBankoPanelSiralarAsync();
                StateHasChanged();
            });
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
                // Hata durumunda güvenli tarafta kal - login'e yönlendir
                NavigationManager.NavigateTo("/auth/login?error=true", forceLoad: true);
            }
        }

        private void CheckBankoModeAccess()
        {
            var currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var relativeUrl = string.IsNullOrEmpty(currentUrl) ? "/" : $"/{currentUrl}";
            var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;

            if (BankoModeState.IsInBankoMode && !string.IsNullOrEmpty(tcKimlikNo))
            {
                if (BankoModeState.IsPersonelInBankoMode(tcKimlikNo))
                {
                    // ⭐ Banko modunda yalnız whitelist URL'lere izin ver
                    if (!BankoModeState.IsUrlAllowedInBankoMode(relativeUrl))
                    {
                        Logger.LogWarning("⚠️ Banko modunda yasaklı URL: {Url} - ana sayfaya yönlendiriliyor", relativeUrl);
                        NavigationManager.NavigateTo("/", forceLoad: true);
                    }
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

        [JSInvokable]
        public async Task OnPermissionsChanged()
        {
            try
            {
                await PermissionStateService.RefreshAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "OnPermissionsChanged hatası");
            }
        }

        [JSInvokable]
        public async Task OnPermissionDefinitionsChanged()
        {
            try
            {
                await PermissionStateService.RefreshDefinitionsAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "OnPermissionDefinitionsChanged hatası");
            }
        }

        private async Task LoadBankoPanelSiralarAsync()
        {
            try
            {
                var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value
                    ?? UserInfoService.GetTcKimlikNo();

                if (string.IsNullOrWhiteSpace(tcKimlikNo))
                {
                    Logger.LogWarning("⚠️ TcKimlikNo bulunamadı, banko panel verisi yüklenemedi");
                    siraListesi = new();
                    return;
                }

                var bankoModundaMi = BankoModeState.IsInBankoMode && BankoModeState.IsPersonelInBankoMode(tcKimlikNo);
                if (!bankoModundaMi)
                {
                    Logger.LogDebug("ℹ️ Banko modu aktif değil, panel verisi yüklenmedi");
                    siraListesi = new();
                    return;
                }

                var response = await SiraCagirmaApiService.GetBankoPanelSiralarAsync(tcKimlikNo);
                siraListesi = response;
                Logger.LogInformation("✅ Banko panel sıraları yüklendi: {Count}", siraListesi.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "❌ Banko panel sıraları yüklenemedi, fallback kullanılacak");
                siraListesi = new();
            }
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
            Logger.LogDebug("⚠️ EnterBankoModeAsync - BankoModeWidget kullanılmalı");
            await Task.CompletedTask;
            return false;
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

            // ⭐ BankoModeState'i güncelle
            var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
            if (!string.IsNullOrEmpty(tcKimlikNo))
            {
                BankoModeState.ActivateBankoMode(bankoId, tcKimlikNo);
                Logger.LogInformation("🏦 BankoModeState aktif edildi: Banko#{BankoId}, TcKimlikNo={TcKimlikNo}", bankoId, tcKimlikNo);

                // ⭐ Sıra listesini direkt yükle (state kontrolü bypass)
                try
                {
                    var response = await SiraCagirmaApiService.GetBankoPanelSiralarAsync(tcKimlikNo);
                    siraListesi = response;
                    Logger.LogInformation("📋 Banko panel sıraları yüklendi: {Count} sıra", siraListesi.Count);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "❌ Banko panel sıraları yüklenemedi");
                    siraListesi = new();
                }

                // UI güncelle
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                Logger.LogWarning("⚠️ OnBankoModeActivated: TcKimlikNo bulunamadı!");
            }
        }

        [JSInvokable]
        public async Task OnBankoModeDeactivated()
        {
            Logger.LogInformation("✅ MainLayout - Banko modu deaktif");

            // ⭐ BankoModeState'i güncelle
            var tcKimlikNo = HttpContextAccessor?.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
            if (!string.IsNullOrEmpty(tcKimlikNo))
            {
                BankoModeState.DeactivateBankoMode(tcKimlikNo);
                Logger.LogInformation("🚪 BankoModeState deaktif edildi: TcKimlikNo={TcKimlikNo}", tcKimlikNo);

                // UI güncelle
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                Logger.LogWarning("⚠️ OnBankoModeDeactivated: TcKimlikNo bulunamadı!");
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            // ⭐ Kullanıcı bazlı event unsubscription
            if (!string.IsNullOrEmpty(_tcKimlikNo))
            {
                BankoModeState.UnsubscribeFromUserChanges(_tcKimlikNo, OnBankoModeStateChanged);
            }
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
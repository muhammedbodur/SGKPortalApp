using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.State;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class BankoModeWidget : ComponentBase, IDisposable
    {
        [Inject] private IBankoApiService BankoApiService { get; set; } = default!;
        [Inject] private IUserApiService UserApiService { get; set; } = default!;
        [Inject] private BankoModeStateService BankoModeState { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        [Inject] private ILogger<BankoModeWidget> Logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private BankoResponseDto? assignedBanko;
        private bool isInBankoMode = false;
        private bool bankoInUse = false;
        private string? activePersonelName;
        private bool isLoading = false;
        
        // ⭐ Kullanıcı TC (AsyncLocal'a bağımlı olmamak için)
        private string? _tcKimlikNo;

        protected override async Task OnInitializedAsync()
        {
            // Kullanıcı TC'sini al ve sakla
            _tcKimlikNo = HttpContextAccessor.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
            
            // ⭐ Kullanıcı bazlı event subscription (AsyncLocal'a bağımlı değil!)
            if (!string.IsNullOrEmpty(_tcKimlikNo))
            {
                BankoModeState.SubscribeToUserChanges(_tcKimlikNo, OnBankoModeStateChanged);
                isInBankoMode = BankoModeState.IsPersonelInBankoMode(_tcKimlikNo);
            }
            
            await LoadData();
        }
        
        private void OnBankoModeStateChanged()
        {
            // State değiştiğinde UI'ı güncelle (tcKimlikNo ile kontrol et)
            if (!string.IsNullOrEmpty(_tcKimlikNo))
            {
                isInBankoMode = BankoModeState.IsPersonelInBankoMode(_tcKimlikNo);
                Logger.LogInformation("🔔 BankoModeWidget event alındı: {TcKimlikNo} - isInBankoMode: {IsActive}", 
                    _tcKimlikNo, isInBankoMode);
            }
            InvokeAsync(StateHasChanged);
        }
        
        public void Dispose()
        {
            // ⭐ Kullanıcı bazlı event unsubscription
            if (!string.IsNullOrEmpty(_tcKimlikNo))
            {
                BankoModeState.UnsubscribeFromUserChanges(_tcKimlikNo, OnBankoModeStateChanged);
            }
        }

        private async Task LoadData()
        {
            try
            {
                var tcKimlikNo = HttpContextAccessor.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    return;
                }

                // ⭐ BankoModeState için mevcut kullanıcıyı set et
                BankoModeState.SetCurrentUser(tcKimlikNo);

                // Personelin atanmış bankosunu getir (API üzerinden)
                var bankoResult = await BankoApiService.GetPersonelCurrentBankoAsync(tcKimlikNo);
                if (bankoResult.Success && bankoResult.Data != null)
                {
                    assignedBanko = bankoResult.Data;
                    
                    // ⭐ DB'den gerçek Banko modu durumunu kontrol et (User tablosu)
                    var bankoModeResult = await UserApiService.IsBankoModeActiveAsync(tcKimlikNo);
                    var dbBankoModeActive = bankoModeResult.Success && bankoModeResult.Data;
                    
                    // In-memory state ile DB durumunu karşılaştır (tcKimlikNo ile kontrol et)
                    var stateIsActive = BankoModeState.IsPersonelInBankoMode(tcKimlikNo);
                    
                    Logger.LogInformation(
                        "🔍 BankoModeWidget LoadData: {TcKimlikNo} - DB Banko Modu: {DbActive}, State Banko Modu: {StateActive}", 
                        tcKimlikNo, dbBankoModeActive, stateIsActive);

                    // ⭐ SADECE DB'de aktifse ve state'te değilse -> State'i senkronize et
                    // (Sayfa yenileme veya yeni sekme durumunda DB'den state'i geri yükle)
                    // NOT: DB'de pasif ama state'te aktifse DOKUNMA - SignalR event'i henüz DB'ye yazılmamış olabilir
                    if (dbBankoModeActive && !stateIsActive)
                    {
                        Logger.LogInformation(
                            "🔄 BankoModeState senkronize ediliyor: Banko#{BankoId} aktif", 
                            assignedBanko.BankoId);
                        BankoModeState.ActivateBankoMode(assignedBanko.BankoId, tcKimlikNo);
                    }
                    
                    // Son durumu al (tcKimlikNo ile kontrol et)
                    isInBankoMode = BankoModeState.IsPersonelInBankoMode(tcKimlikNo);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "BankoModeWidget LoadData hatası");
            }
        }

        private async Task EnterBankoMode()
        {
            if (assignedBanko == null || isLoading) return;

            try
            {
                isLoading = true;
                StateHasChanged();

                // ⭐ YENİ: SignalR Hub üzerinden banko moduna geç (Sayfa yenileme YOK!)
                await JSRuntime.InvokeVoidAsync("bankoMode.enter", assignedBanko.BankoId);
                
                Logger.LogInformation("✅ Banko moduna geçiş isteği gönderildi: Banko#{BankoNo}", assignedBanko.BankoNo);
                
                // UI güncellemesi SignalR event'i ile gelecek (BankoModeActivated)
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Banko moduna giriş hatası");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ExitBankoMode()
        {
            if (isLoading) return;

            try
            {
                isLoading = true;
                StateHasChanged();

                var tcKimlikNo = HttpContextAccessor.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogError("Kullanıcı bilgisi bulunamadı");
                    return;
                }

                // 1. Sıra Çağırma Paneli'nin pin'ini kaldır ve paneli kapat
                try
                {
                    await JSRuntime.InvokeVoidAsync("SiraCagirmaPanel.setPin", false);
                    await JSRuntime.InvokeVoidAsync("SiraCagirmaPanel.closePanel");
                    Logger.LogInformation("📌 Sıra Çağırma Paneli pin'i kaldırıldı ve kapatıldı");
                }
                catch (Exception jsEx)
                {
                    Logger.LogWarning(jsEx, "⚠️ Sıra Çağırma Paneli kapatılırken hata (panel yüklenmemiş olabilir)");
                }

                // 2. ⭐ YENİ: SignalR Hub üzerinden banko modundan çık
                await JSRuntime.InvokeVoidAsync("bankoMode.exit");
                
                Logger.LogInformation("✅ Banko modundan çıkış isteği gönderildi");
                
                // UI güncellemesi SignalR event'i ile gelecek (BankoModeDeactivated)
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Banko modundan çıkış hatası");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

    }
}

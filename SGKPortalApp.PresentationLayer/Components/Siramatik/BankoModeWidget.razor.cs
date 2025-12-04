using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.State;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class BankoModeWidget : ComponentBase, IDisposable
    {
        [Inject] private IBankoApiService BankoApiService { get; set; } = default!;
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

        protected override async Task OnInitializedAsync()
        {
            // State değişikliklerini dinle
            BankoModeState.OnBankoModeChanged += OnBankoModeStateChanged;
            isInBankoMode = BankoModeState.IsInBankoMode;
            await LoadData();
        }
        
        private void OnBankoModeStateChanged()
        {
            // State değiştiğinde UI'ı güncelle
            isInBankoMode = BankoModeState.IsInBankoMode;
            InvokeAsync(StateHasChanged);
        }
        
        public void Dispose()
        {
            BankoModeState.OnBankoModeChanged -= OnBankoModeStateChanged;
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
                    
                    // Banko modunda mı kontrol et
                    isInBankoMode = BankoModeState.IsInBankoMode;

                    Logger.LogInformation("🔍 BankoModeWidget LoadData: {TcKimlikNo} - Banko Modu: {IsInBankoMode}", tcKimlikNo, isInBankoMode);

                    if (isInBankoMode)
                    {
                        BankoModeState.ActivateBankoMode(assignedBanko.BankoId, tcKimlikNo);
                    }
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

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using SGKPortalApp.PresentationLayer.Services.State;

namespace SGKPortalApp.PresentationLayer.Components.Siramatik
{
    public partial class BankoModeWidget : ComponentBase, IDisposable
    {
        [Inject] private IBankoModeService BankoModeService { get; set; } = default!;
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
            
            await LoadData();
        }
        
        private void OnBankoModeStateChanged()
        {
            // State değiştiğinde UI'ı güncelle
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

                // Personelin atanmış bankosunu getir
                assignedBanko = await BankoModeService.GetPersonelAssignedBankoAsync(tcKimlikNo);
                
                if (assignedBanko != null)
                {
                    // Banko modunda mı kontrol et (User tablosundan - sayfa yenilendiğinde de çalışır)
                    isInBankoMode = await BankoModeService.IsPersonelInBankoModeAsync(tcKimlikNo);
                    
                    Logger.LogInformation($"🔍 BankoModeWidget LoadData: {tcKimlikNo} - Banko Modu: {isInBankoMode}");
                    
                    // Banko kullanımda mı kontrol et
                    if (!isInBankoMode)
                    {
                        bankoInUse = await BankoModeService.IsBankoInUseAsync(assignedBanko.BankoId);
                        if (bankoInUse)
                        {
                            activePersonelName = await BankoModeService.GetBankoActivePersonelNameAsync(assignedBanko.BankoId);
                        }
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

                var tcKimlikNo = HttpContextAccessor.HttpContext?.User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    Logger.LogError("Kullanıcı bilgisi bulunamadı");
                    return;
                }

                // 1. Banko moduna geç
                var success = await BankoModeService.EnterBankoModeAsync(tcKimlikNo, assignedBanko.BankoId);
                
                if (success)
                {
                    Logger.LogInformation("✅ Banko moduna geçildi. Banko: {BankoNo} - Sayfa yenileniyor...", assignedBanko.BankoNo);
                    
                    // 2. Sayfa yenileniyor - yeni ConnectionId ile banko modunda açılacak
                    // Eski bağlantılar otomatik kapatılacak
                    NavigationManager.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    Logger.LogWarning("❌ Banko moduna geçilemedi");
                }
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

                // Banko modundan çık
                var success = await BankoModeService.ExitBankoModeAsync(tcKimlikNo);
                
                if (success)
                {
                    isInBankoMode = false;
                    BankoModeState.DeactivateBankoMode();
                    Logger.LogInformation("✅ Banko modundan çıkıldı - User tablosu güncellendi");
                    
                    // Banko kullanım durumunu yeniden kontrol et
                    await LoadData();
                }
                else
                {
                    Logger.LogWarning("❌ Banko modundan çıkılamadı");
                }
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

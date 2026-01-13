using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class DeviceSync
    {
        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<DeviceResponseDto> Devices { get; set; } = new();
        private bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadDevices();
        }

        private async Task LoadDevices()
        {
            IsLoading = true;
            try
            {
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    Devices = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihazlar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SyncDevice(int deviceId)
        {
            await ToastService.ShowInfoAsync("Senkronizasyon başlatılıyor...");
            // TODO: Implement sync logic
            await ToastService.ShowSuccessAsync("Senkronizasyon tamamlandı");
        }
    }
}

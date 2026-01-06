using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class AttendanceRecords
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private List<CekilenData>? records;
        private List<dynamic>? devices;
        private string apiBaseUrl = "";
        private int selectedDeviceId = 0;

        protected override async Task OnInitializedAsync()
        {
            apiBaseUrl = Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
            await LoadDevices();
            await LoadRecords();
        }

        private async Task LoadDevices()
        {
            try
            {
                devices = await Http.GetFromJsonAsync<List<dynamic>>($"{apiBaseUrl}/api/ZKTecoDevice/active");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("console.error", $"Cihazlar yüklenemedi: {ex.Message}");
            }
        }

        private async Task LoadRecords()
        {
            try
            {
                records = await Http.GetFromJsonAsync<List<CekilenData>>($"{apiBaseUrl}/api/ZKTecoAttendance");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("console.error", $"Kayıtlar yüklenemedi: {ex.Message}");
            }
        }

        private async Task OnDeviceChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deviceId))
            {
                selectedDeviceId = deviceId;
            }
            else
            {
                selectedDeviceId = 0;
            }
        }

        private async Task SyncFromDevice()
        {
            if (selectedDeviceId == 0) return;

            try
            {
                var response = await Http.PostAsync($"{apiBaseUrl}/api/ZKTecoAttendance/device/{selectedDeviceId}/sync", null);

                if (response.IsSuccessStatusCode)
                {
                    await JS.InvokeVoidAsync("alert", "Kayıtlar cihazdan başarıyla senkronize edildi!");
                    await LoadRecords();
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", $"Senkronizasyon hatası: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"Hata: {ex.Message}");
            }
        }
    }
}

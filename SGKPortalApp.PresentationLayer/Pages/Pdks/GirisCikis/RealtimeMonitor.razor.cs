using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.PresentationLayer.Models.FormModels.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.GirisCikis
{
    public partial class RealtimeMonitor
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;
        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;

        private List<RealtimeEventModel> Events = new();
        private bool IsConnected = false;
        private int MaxEvents = 100;
        private List<DeviceResponseDto> MonitoringDevices = new();
        private bool IsLoadingDevices = true;

        // İstatistikler
        private int TotalEvents => Events.Count;
        private int CheckInCount => Events.Count(e => e.InOutMode == 0);
        private int CheckOutCount => Events.Count(e => e.InOutMode == 1);
        private string LastEventTime => Events.Any() ? Events.Max(e => e.EventTime).ToString("HH:mm:ss") : "-";

        protected override async Task OnInitializedAsync()
        {
            await LoadMonitoringDevicesAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("🔵 RealtimeMonitor: OnAfterRenderAsync - firstRender = true");
                
                // JavaScript kütüphanelerinin yüklenmesi için gecikme (Blazor framework yüklendikten sonra)
                await Task.Delay(500);
                
                Console.WriteLine("🔵 RealtimeMonitor: Calling InitializeSignalRAsync...");
                await InitializeSignalRAsync();
                Console.WriteLine("🔵 RealtimeMonitor: InitializeSignalRAsync completed");
            }
        }

        private async Task LoadMonitoringDevicesAsync()
        {
            try
            {
                IsLoadingDevices = true;
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    MonitoringDevices = result.Data.Where(d => d.IsMonitoring).ToList();
                }
                else
                {
                    MonitoringDevices = new List<DeviceResponseDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Monitoring devices yüklenirken hata: {ex.Message}");
                MonitoringDevices = new List<DeviceResponseDto>();
            }
            finally
            {
                IsLoadingDevices = false;
                StateHasChanged();
            }
        }

        private async Task InitializeSignalRAsync()
        {
            int maxRetries = 3;
            int retryCount = 0;
            int delayMs = 1000;

            while (retryCount < maxRetries)
            {
                try
                {
                    var apiUrl = Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
                    var hubUrl = $"{apiUrl}/hubs/pdks";

                    Console.WriteLine($"SignalR initialization attempt {retryCount + 1}/{maxRetries}: {hubUrl}");

                    await JS.InvokeVoidAsync("PdksRealtimeMonitor.initialize", hubUrl,
                        DotNetObjectReference.Create(this));

                    Console.WriteLine("✅ SignalR initialized successfully");
                    return; // Başarılı, çık
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"❌ SignalR initialization error (attempt {retryCount}/{maxRetries}): {ex.Message}");

                    if (retryCount < maxRetries)
                    {
                        Console.WriteLine($"Retrying in {delayMs}ms...");
                        await Task.Delay(delayMs);
                        delayMs *= 2; // Exponential backoff
                    }
                    else
                    {
                        Console.WriteLine("❌ SignalR initialization failed after all retries. Check:");
                        Console.WriteLine("  1. pdks-realtime-monitor.js dosyası yüklendi mi?");
                        Console.WriteLine("  2. SignalR library yüklendi mi?");
                        Console.WriteLine("  3. PdksHub API'si çalışıyor mu?");
                    }
                }
            }
        }

        [JSInvokable]
        public async Task OnRealtimeEvent(RealtimeEventModel evt)
        {
            Events.Insert(0, evt);

            // Max event sayısını aşınca eski kayıtları sil
            if (Events.Count > MaxEvents)
            {
                Events = Events.Take(MaxEvents).ToList();
            }

            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public async Task OnConnectionStateChanged(bool isConnected)
        {
            IsConnected = isConnected;
            await InvokeAsync(StateHasChanged);
        }

        private void ClearEvents()
        {
            Events.Clear();
            StateHasChanged();
        }

        private string GetVerifyMethodBadge(int verifyMethod)
        {
            return verifyMethod switch
            {
                0 => "<span class='badge bg-secondary'>Şifre</span>",
                1 => "<span class='badge bg-primary'>Parmak İzi</span>",
                3 => "<span class='badge bg-info'>Yüz Tanıma</span>",
                15 => "<span class='badge bg-success'>Kart</span>",
                _ => $"<span class='badge bg-warning'>{verifyMethod}</span>"
            };
        }

        private string GetInOutModeBadge(int inOutMode)
        {
            return inOutMode switch
            {
                0 => "<span class='badge bg-success'>Giriş</span>",
                1 => "<span class='badge bg-warning'>Çıkış</span>",
                2 => "<span class='badge bg-info'>Mola Başlangıç</span>",
                3 => "<span class='badge bg-info'>Mola Bitiş</span>",
                4 => "<span class='badge bg-secondary'>Mesai Başlangıç</span>",
                5 => "<span class='badge bg-secondary'>Mesai Bitiş</span>",
                _ => $"<span class='badge bg-dark'>{inOutMode}</span>"
            };
        }
    }
}

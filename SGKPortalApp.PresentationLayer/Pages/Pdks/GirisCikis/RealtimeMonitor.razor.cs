using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.GirisCikis
{
    public partial class RealtimeMonitor
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;

        private List<RealtimeEventModel> Events = new();
        private bool IsConnected = false;
        private int MaxEvents = 100;

        // İstatistikler
        private int TotalEvents => Events.Count;
        private int CheckInCount => Events.Count(e => e.InOutMode == 0);
        private int CheckOutCount => Events.Count(e => e.InOutMode == 1);
        private string LastEventTime => Events.Any() ? Events.Max(e => e.EventTime).ToString("HH:mm:ss") : "-";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // JavaScript kütüphanelerinin yüklenmesi için kısa gecikme
                await Task.Delay(300);
                await InitializeSignalRAsync();
            }
        }

        private async Task InitializeSignalRAsync()
        {
            try
            {
                var apiUrl = Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
                var hubUrl = $"{apiUrl}/hubs/pdks";

                await JS.InvokeVoidAsync("PdksRealtimeMonitor.initialize", hubUrl,
                    DotNetObjectReference.Create(this));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR initialization error: {ex.Message}");
                // Tekrar dene (JavaScript dosyası henüz yüklenmemiş olabilir)
                await Task.Delay(1000);
                try
                {
                    var apiUrl = Configuration["AppSettings:ApiUrl"] ?? "https://localhost:9080";
                    var hubUrl = $"{apiUrl}/hubs/pdks";
                    await JS.InvokeVoidAsync("PdksRealtimeMonitor.initialize", hubUrl,
                        DotNetObjectReference.Create(this));
                }
                catch
                {
                    Console.WriteLine("SignalR initialization failed after retry. Check browser console.");
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

        public class RealtimeEventModel
        {
            public string EnrollNumber { get; set; } = string.Empty;
            public DateTime EventTime { get; set; }
            public int VerifyMethod { get; set; }
            public int InOutMode { get; set; }
            public int WorkCode { get; set; }
            public string DeviceIp { get; set; } = string.Empty;
            public bool IsValid { get; set; }
            public long? CardNumber { get; set; }
        }
    }
}

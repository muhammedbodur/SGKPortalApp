using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class RealtimeMonitor : FieldPermissionPageBase, IAsyncDisposable
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<DeviceResponseDto> devices = new List<DeviceResponseDto>();
        private List<RealtimeEventDto> events = new List<RealtimeEventDto>();
        private int selectedDeviceId = 0;
        private bool isMonitoring = false;
        private bool isConnected = false;

        private HubConnection? hubConnection;
        private string apiUrl = string.Empty;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            
            // API URL'i configuration'dan al - zorunlu
            apiUrl = Configuration["AppSettings:ApiUrl"] 
                ?? throw new InvalidOperationException("AppSettings:ApiUrl configuration is required!");

            await LoadDevices();
            await InitializeSignalR();
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    devices = result.Data;
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
        }

        // ═══════════════════════════════════════════════════════
        // SIGNALR CONNECTION
        // ═══════════════════════════════════════════════════════

        private async Task InitializeSignalR()
        {
            try
            {
                // SignalR Hub URL - ApiLayer'daki PdksHub (/hubs/pdks)
                var hubUrl = $"{apiUrl.TrimEnd('/')}/hubs/pdks";

                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                // Realtime event handler - PdksHub'dan "OnRealtimeEvent" event'ini dinle
                hubConnection.On<RealtimeEventDto>("OnRealtimeEvent", (eventDto) =>
                {
                    events.Insert(0, eventDto); // En yeni event en üstte
                    
                    // Max 100 event tut (performans için)
                    if (events.Count > 100)
                    {
                        events.RemoveAt(events.Count - 1);
                    }

                    InvokeAsync(StateHasChanged);
                });

                hubConnection.Closed += async (error) =>
                {
                    isConnected = false;
                    await InvokeAsync(StateHasChanged);
                };

                hubConnection.Reconnecting += async (error) =>
                {
                    isConnected = false;
                    await InvokeAsync(StateHasChanged);
                };

                hubConnection.Reconnected += async (connectionId) =>
                {
                    isConnected = true;
                    await ToastService.ShowSuccessAsync("SignalR yeniden bağlandı");
                    await InvokeAsync(StateHasChanged);
                };

                await hubConnection.StartAsync();
                isConnected = true;
                await ToastService.ShowSuccessAsync("SignalR bağlantısı kuruldu");
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"SignalR bağlantı hatası: {ex.Message}");
                isConnected = false;
            }
        }

        // ═══════════════════════════════════════════════════════
        // MONITORING OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task StartMonitoring()
        {
            if (selectedDeviceId == 0)
            {
                await ToastService.ShowWarningAsync("Lütfen bir cihaz seçin");
                return;
            }

            if (!isConnected)
            {
                await ToastService.ShowErrorAsync("SignalR bağlantısı yok. Sayfa yenilenecek.");
                Navigation.Refresh(true);
                return;
            }

            try
            {
                // Business katmanı üzerinden monitoring başlat
                var result = await DeviceApiService.StartRealtimeMonitoringAsync(selectedDeviceId);
                if (result.Success && result.Data)
                {
                    isMonitoring = true;
                    await ToastService.ShowSuccessAsync("Gerçek zamanlı izleme başlatıldı");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İzleme başlatılamadı");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task StopMonitoring()
        {
            if (selectedDeviceId == 0) return;

            try
            {
                var result = await DeviceApiService.StopRealtimeMonitoringAsync(selectedDeviceId);
                if (result.Success && result.Data)
                {
                    isMonitoring = false;
                    await ToastService.ShowSuccessAsync("Gerçek zamanlı izleme durduruldu");
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "İzleme durdurulamadı");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private void ClearEvents()
        {
            events.Clear();
            StateHasChanged();
        }
    }
}

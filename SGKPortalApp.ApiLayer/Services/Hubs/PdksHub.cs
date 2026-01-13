using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SGKPortalApp.ApiLayer.Services.Hubs.Base;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Services.Hubs
{
    /// <summary>
    /// PDKS (Personel Devam Kontrol Sistemi) için SignalR Hub
    /// ZKTeco cihazlardan gelen realtime event'leri frontend'e broadcast eder
    /// </summary>
    public class PdksHub : BaseHub
    {
        // Bağlı kullanıcıları takip et (Monitoring sayfası açık olanlar)
        private static readonly ConcurrentDictionary<string, DateTime> ActiveMonitors = new();

        public PdksHub(ILogger<PdksHub> logger) : base(logger)
        {
        }

        #region Connection Management

        /// <summary>
        /// Client bağlandığında
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var connectionId = Context.ConnectionId;
            ActiveMonitors.TryAdd(connectionId, DateTime.Now);

            _logger.LogInformation($"PDKS Hub - Client connected: {connectionId}. Total monitors: {ActiveMonitors.Count}");

            // Bağlanan client'a hoş geldin mesajı
            await Clients.Caller.SendAsync("OnConnected", new
            {
                Message = "PDKS Realtime Monitoring'e bağlandınız",
                ConnectedAt = DateTime.Now,
                TotalMonitors = ActiveMonitors.Count
            });
        }

        /// <summary>
        /// Client bağlantısı koptuğunda
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            ActiveMonitors.TryRemove(connectionId, out _);

            _logger.LogInformation($"PDKS Hub - Client disconnected: {connectionId}. Remaining monitors: {ActiveMonitors.Count}");

            if (exception != null)
            {
                _logger.LogError(exception, $"PDKS Hub - Connection closed with error: {connectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Realtime Event Broadcasting

        /// <summary>
        /// ZKTeco realtime event'ini tüm bağlı client'lara broadcast et
        /// Bu metod Background Service (ZKTecoRealtimeListenerService) tarafından çağrılır
        /// </summary>
        /// <param name="eventDto">Realtime event data</param>
        public async Task BroadcastRealtimeEvent(RealtimeEventDto eventDto)
        {
            try
            {
                _logger.LogInformation(
                    $"PDKS Hub - Broadcasting event: EnrollNumber={eventDto.EnrollNumber}, " +
                    $"EventTime={eventDto.EventTime}, Device={eventDto.DeviceIp}");

                // Tüm bağlı client'lara gönder
                await Clients.All.SendAsync("OnRealtimeEvent", eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDKS Hub - Error broadcasting realtime event");
            }
        }

        /// <summary>
        /// Belirli bir cihazdan gelen event'i broadcast et
        /// </summary>
        public async Task BroadcastDeviceEvent(string deviceIp, RealtimeEventDto eventDto)
        {
            try
            {
                // Device-specific group varsa ona da gönder
                await Clients.Group($"Device_{deviceIp}").SendAsync("OnDeviceEvent", eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PDKS Hub - Error broadcasting device event: {deviceIp}");
            }
        }

        #endregion

        #region Client-Callable Methods

        /// <summary>
        /// Client'ın aktif monitör sayısını sorgulaması
        /// </summary>
        public int GetActiveMonitorCount()
        {
            return ActiveMonitors.Count;
        }

        /// <summary>
        /// Belirli bir cihazı dinlemeye başla (Group'a katıl)
        /// </summary>
        public async Task SubscribeToDevice(string deviceIp)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Device_{deviceIp}");
                _logger.LogInformation($"PDKS Hub - Client {Context.ConnectionId} subscribed to device: {deviceIp}");

                await Clients.Caller.SendAsync("OnDeviceSubscribed", new
                {
                    DeviceIp = deviceIp,
                    Message = $"Cihaz {deviceIp} için event'leri dinlemeye başladınız"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PDKS Hub - Error subscribing to device: {deviceIp}");
            }
        }

        /// <summary>
        /// Cihaz dinlemeyi durdur (Group'tan çık)
        /// </summary>
        public async Task UnsubscribeFromDevice(string deviceIp)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Device_{deviceIp}");
                _logger.LogInformation($"PDKS Hub - Client {Context.ConnectionId} unsubscribed from device: {deviceIp}");

                await Clients.Caller.SendAsync("OnDeviceUnsubscribed", new
                {
                    DeviceIp = deviceIp,
                    Message = $"Cihaz {deviceIp} için event dinleme durduruldu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PDKS Hub - Error unsubscribing from device: {deviceIp}");
            }
        }

        /// <summary>
        /// Ping - Bağlantı testi için
        /// </summary>
        public async Task<string> Ping()
        {
            await Task.CompletedTask;
            return "Pong";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Aktif monitör listesini temizle (eski bağlantılar)
        /// </summary>
        public static void CleanupInactiveMonitors(TimeSpan timeout)
        {
            var now = DateTime.Now;
            var expiredConnections = ActiveMonitors
                .Where(kvp => now - kvp.Value > timeout)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var connectionId in expiredConnections)
            {
                ActiveMonitors.TryRemove(connectionId, out _);
            }
        }

        #endregion
    }
}

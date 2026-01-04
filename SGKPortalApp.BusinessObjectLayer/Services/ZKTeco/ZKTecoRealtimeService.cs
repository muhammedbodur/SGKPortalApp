using Microsoft.AspNet.SignalR.Client;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    /// <summary>
    /// ZKTecoApi SignalR Hub client interface
    /// Realtime event'leri dinler
    /// </summary>
    public interface IZKTecoRealtimeService
    {
        /// <summary>
        /// SignalR bağlantısını başlat
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// SignalR bağlantısını durdur
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// Belirli bir cihaza abone ol (SignalR hub üzerinden)
        /// </summary>
        Task SubscribeToDeviceAsync(string deviceIp);

        /// <summary>
        /// Cihaz aboneliğinden çık
        /// </summary>
        Task UnsubscribeFromDeviceAsync(string deviceIp);

        /// <summary>
        /// Realtime event geldiğinde tetiklenir
        /// </summary>
        event EventHandler<RealtimeEventDto>? OnRealtimeEvent;

        /// <summary>
        /// Bağlantı durumu
        /// </summary>
        bool IsConnected { get; }
    }

    /// <summary>
    /// ZKTecoApi SignalR Hub client implementation
    /// ZKTecoApi'nin RealtimeEventHub'ına bağlanır ve event'leri dinler
    /// </summary>
    public class ZKTecoRealtimeService : IZKTecoRealtimeService
    {
        private readonly ILogger<ZKTecoRealtimeService> _logger;
        private readonly string _signalRUrl;
        private HubConnection? _connection;
        private IHubProxy? _hub;

        public event EventHandler<RealtimeEventDto>? OnRealtimeEvent;

        public bool IsConnected => _connection?.State == ConnectionState.Connected;

        public ZKTecoRealtimeService(ILogger<ZKTecoRealtimeService> logger, string signalRUrl)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signalRUrl = signalRUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(signalRUrl));
        }

        public async Task StartAsync()
        {
            try
            {
                _logger.LogInformation($"Starting SignalR connection to: {_signalRUrl}");

                _connection = new HubConnection(_signalRUrl);
                _hub = _connection.CreateHubProxy("RealtimeEventHub");

                // Event handler'ı kaydet - ZKTecoApi'den gelen realtime event
                _hub.On<RealtimeEventDto>("onRealtimeEvent", (evt) =>
                {
                    _logger.LogInformation(
                        $"Realtime event received: EnrollNumber={evt.EnrollNumber}, " +
                        $"EventTime={evt.EventTime}, Device={evt.DeviceIp}, " +
                        $"VerifyMethod={evt.VerifyMethod}, InOutMode={evt.InOutMode}");

                    // Event'i dışarıya bildir
                    OnRealtimeEvent?.Invoke(this, evt);
                });

                // Cihaza abone olunduğunda
                _hub.On<string>("onSubscribed", (deviceIp) =>
                {
                    _logger.LogInformation($"Successfully subscribed to device: {deviceIp}");
                });

                // Cihaz aboneliğinden çıkıldığında
                _hub.On<string>("onUnsubscribed", (deviceIp) =>
                {
                    _logger.LogInformation($"Successfully unsubscribed from device: {deviceIp}");
                });

                // Bağlantı koptuğunda
                _connection.Closed += () =>
                {
                    _logger.LogWarning("SignalR connection closed. Attempting to reconnect...");
                    Task.Run(async () => await ReconnectAsync());
                };

                // Bağlantıyı başlat
                await _connection.Start();
                _logger.LogInformation($"SignalR connection started successfully. State: {_connection.State}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start SignalR connection");
                throw;
            }
        }

        public async Task StopAsync()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Stop();
                    _connection.Dispose();
                    _logger.LogInformation("SignalR connection stopped");
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping SignalR connection");
            }
        }

        public async Task SubscribeToDeviceAsync(string deviceIp)
        {
            try
            {
                if (_hub == null)
                    throw new InvalidOperationException("SignalR connection not started. Call StartAsync first.");

                if (!IsConnected)
                    throw new InvalidOperationException("SignalR connection not connected.");

                await _hub.Invoke("SubscribeToDevice", deviceIp);
                _logger.LogInformation($"Requested subscription to device: {deviceIp}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to subscribe to device: {deviceIp}");
                throw;
            }
        }

        public async Task UnsubscribeFromDeviceAsync(string deviceIp)
        {
            try
            {
                if (_hub == null)
                    throw new InvalidOperationException("SignalR connection not started.");

                if (!IsConnected)
                {
                    _logger.LogWarning($"Cannot unsubscribe from {deviceIp} - not connected");
                    return;
                }

                await _hub.Invoke("UnsubscribeFromDevice", deviceIp);
                _logger.LogInformation($"Requested unsubscription from device: {deviceIp}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to unsubscribe from device: {deviceIp}");
                throw;
            }
        }

        /// <summary>
        /// Bağlantı koptuğunda otomatik yeniden bağlan
        /// </summary>
        private async Task ReconnectAsync()
        {
            const int maxRetries = 5;
            const int delaySeconds = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    _logger.LogInformation($"Reconnection attempt {i + 1}/{maxRetries}...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                    if (_connection != null)
                    {
                        await _connection.Start();
                        _logger.LogInformation("Reconnected successfully");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Reconnection attempt {i + 1} failed");
                }
            }

            _logger.LogError($"Failed to reconnect after {maxRetries} attempts");
        }
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
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
}

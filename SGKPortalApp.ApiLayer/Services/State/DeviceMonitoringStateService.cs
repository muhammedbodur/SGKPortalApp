using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SGKPortalApp.ApiLayer.Services.State
{
    /// <summary>
    /// Realtime monitoring yapılan cihazları takip eder
    /// Singleton service olarak kullanılır
    /// </summary>
    public class DeviceMonitoringStateService
    {
        // Thread-safe collection: DeviceId -> Monitoring başlatma zamanı
        private readonly ConcurrentDictionary<int, DateTime> _monitoringDevices = new();

        /// <summary>
        /// Cihazı monitoring listesine ekle
        /// </summary>
        public void StartMonitoring(int deviceId)
        {
            _monitoringDevices.TryAdd(deviceId, DateTime.Now);
        }

        /// <summary>
        /// Cihazı monitoring listesinden çıkar
        /// </summary>
        public void StopMonitoring(int deviceId)
        {
            _monitoringDevices.TryRemove(deviceId, out _);
        }

        /// <summary>
        /// Cihaz monitoring durumunda mı?
        /// </summary>
        public bool IsMonitoring(int deviceId)
        {
            return _monitoringDevices.ContainsKey(deviceId);
        }

        /// <summary>
        /// Tüm monitoring cihazları
        /// </summary>
        public List<int> GetMonitoringDevices()
        {
            return _monitoringDevices.Keys.ToList();
        }

        /// <summary>
        /// Monitoring cihaz sayısı
        /// </summary>
        public int GetMonitoringDeviceCount()
        {
            return _monitoringDevices.Count;
        }

        /// <summary>
        /// Cihazın monitoring başlatma zamanı
        /// </summary>
        public DateTime? GetMonitoringStartTime(int deviceId)
        {
            return _monitoringDevices.TryGetValue(deviceId, out var startTime) ? startTime : null;
        }

        /// <summary>
        /// Tüm monitoring durumunu temizle (uygulama yeniden başladığında)
        /// </summary>
        public void ClearAll()
        {
            _monitoringDevices.Clear();
        }
    }
}

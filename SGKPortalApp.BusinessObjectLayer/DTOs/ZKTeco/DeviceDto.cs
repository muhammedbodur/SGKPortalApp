using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz zaman bilgisi DTO
    /// </summary>
    public class DeviceTimeDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public DateTime DeviceTime { get; set; }
        public DateTime ServerTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Cihaz ve sunucu arasındaki zaman farkı (saniye)
        /// </summary>
        public double TimeDifferenceSeconds => (DeviceTime - ServerTime).TotalSeconds;

        /// <summary>
        /// Zaman senkronize mi? (5 saniye tolerans)
        /// </summary>
        public bool IsSynchronized => Math.Abs(TimeDifferenceSeconds) <= 5;
    }

    /// <summary>
    /// Cihaz zaman güncelleme DTO
    /// </summary>
    public class UpdateDeviceTimeDto
    {
        public DateTime NewTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Cihaz işlem sonucu DTO
    /// </summary>
    public class DeviceOperationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public int Port { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.Now;
    }
}

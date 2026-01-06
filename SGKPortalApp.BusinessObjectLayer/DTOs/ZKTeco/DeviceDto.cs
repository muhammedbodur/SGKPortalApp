using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz durum bilgisi DTO
    /// ZKTecoApi GET /api/device/{ip}/status yanıtı
    /// </summary>
    public class DeviceStatusDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsConnected { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? SerialNumber { get; set; }
        public string? DeviceName { get; set; }
        public string? Platform { get; set; }
        public int UserCount { get; set; }
        public int AttendanceLogCount { get; set; }
        public int UserCapacity { get; set; }
        public int AttLogCapacity { get; set; }
        public int FaceCount { get; set; }
        public int FaceCapacity { get; set; }
        public int FingerPrintCount { get; set; }
        public int FingerPrintCapacity { get; set; }
        public DateTime? DeviceTime { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Cihaz doluluk oranı (%)
        /// </summary>
        public double UserUsagePercentage => UserCapacity > 0 ? (double)UserCount / UserCapacity * 100 : 0;
        public double AttendanceUsagePercentage => AttLogCapacity > 0 ? (double)AttendanceLogCount / AttLogCapacity * 100 : 0;
    }

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

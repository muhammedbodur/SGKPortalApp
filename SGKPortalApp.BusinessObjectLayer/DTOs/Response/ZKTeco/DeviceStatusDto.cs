using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz durumu DTO
    /// ZKTecoApi → DeviceStatusResponse mapping
    /// </summary>
    public class DeviceStatusDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool IsConnected { get; set; }
        public string? Platform { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? SerialNumber { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceName { get; set; }
        public int UserCount { get; set; }
        public int LogCount { get; set; }
        public int AttendanceLogCount { get; set; }
        public int UserCapacity { get; set; }
        public int LogCapacity { get; set; }
        public int AttLogCapacity { get; set; }
        public int FaceCount { get; set; }
        public int FaceCapacity { get; set; }
        public int FingerPrintCount { get; set; }
        public int FingerPrintCapacity { get; set; }
        public DateTime? DeviceTime { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Cihaz doluluk oranı (%)
        /// </summary>
        public double UserUsagePercentage => UserCapacity > 0 ? (double)UserCount / UserCapacity * 100 : 0;
        public double AttendanceUsagePercentage => AttLogCapacity > 0 ? (double)AttendanceLogCount / AttLogCapacity * 100 : 0;
    }
}

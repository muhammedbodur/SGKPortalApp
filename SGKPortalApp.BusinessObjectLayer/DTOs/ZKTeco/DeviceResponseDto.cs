using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco Device Response DTO
    /// </summary>
    public class DeviceResponseDto
    {
        public int DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Port { get; set; } = "4370";
        public string? DeviceCode { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsActive { get; set; }
        public int? HizmetBinasiId { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public int SyncCount { get; set; }
        public bool? LastSyncSuccess { get; set; }
        public string? LastSyncStatus { get; set; }
        public DateTime? LastHealthCheckTime { get; set; }
        public int HealthCheckCount { get; set; }
        public bool? LastHealthCheckSuccess { get; set; }
        public string? LastHealthCheckStatus { get; set; }
    }
}

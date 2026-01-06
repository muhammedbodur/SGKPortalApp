using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco Device Response DTO
    /// </summary>
    public class DeviceResponseDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Port { get; set; } = "4370";
        public string? SerialNumber { get; set; }
        public string? DeviceModel { get; set; }
        public bool IsActive { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public bool? LastSyncSuccess { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz zaman bilgisi DTO
    /// ZKTecoApi → DeviceTimeResponse mapping
    /// </summary>
    public class DeviceTimeDto
    {
        public DateTime DeviceTime { get; set; }
        public DateTime ServerTime { get; set; }
        public int TimeDifferenceSeconds { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}

using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco attendance log
    /// ZKTecoApi → AttendanceLogResponse mapping
    /// </summary>
    public class AttendanceLogDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int VerifyMethod { get; set; } // 0=Password, 1=Fingerprint, 15=Card
        public int InOutMode { get; set; } // 0=Check-In, 1=Check-Out
        public int WorkCode { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
    }
}

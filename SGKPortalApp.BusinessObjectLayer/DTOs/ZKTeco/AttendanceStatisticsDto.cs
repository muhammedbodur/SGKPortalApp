using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// Attendance istatistik DTO
    /// </summary>
    public class AttendanceStatisticsDto
    {
        public int TotalRecords { get; set; }
        public DateTime? FirstRecord { get; set; }
        public DateTime? LastRecord { get; set; }
        public int UniqueUserCount { get; set; }
        public int CheckInCount { get; set; }
        public int CheckOutCount { get; set; }
        public int BreakCount { get; set; }
        public Dictionary<int, int> VerifyMethodCounts { get; set; } = new();
        public Dictionary<string, int> DeviceRecordCounts { get; set; } = new();
    }
}

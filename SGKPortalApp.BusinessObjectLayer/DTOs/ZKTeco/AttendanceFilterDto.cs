using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// Attendance filtreleme parametreleri DTO
    /// </summary>
    public class AttendanceFilterDto
    {
        public string? EnrollNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? VerifyMethod { get; set; }
        public int? InOutMode { get; set; }
        public string? DeviceIp { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}

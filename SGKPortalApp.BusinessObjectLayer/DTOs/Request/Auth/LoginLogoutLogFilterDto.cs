using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth
{
    public class LoginLogoutLogFilterDto
    {
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? OnlyActiveSession { get; set; }
        public bool? OnlyFailedLogins { get; set; }
        public string? IpAddress { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}

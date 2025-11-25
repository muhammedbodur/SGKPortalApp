using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// SignalR bağlantı bilgileri
    /// </summary>
    public class ConnectionInfoDto
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public DateTime ConnectedAt { get; set; }
    }
}

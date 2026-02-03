namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    public class DeviceSyncDetail
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceIp { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

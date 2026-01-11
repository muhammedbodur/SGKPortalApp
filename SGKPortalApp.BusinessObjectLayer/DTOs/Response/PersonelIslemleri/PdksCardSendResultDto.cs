namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PdksCardSendResultDto
    {
        public int TotalDevices { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<DeviceOperationResult> DeviceResults { get; set; } = new();
    }

    public class DeviceOperationResult
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

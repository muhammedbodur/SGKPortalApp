namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PdksCardSendResultDto
    {
        public int TotalDevices { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<DeviceOperationResult> DeviceResults { get; set; } = new();
    }
}

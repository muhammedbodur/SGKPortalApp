namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret tarih çakışma kontrolü sonucu
    /// </summary>
    public class OverlapCheckResponseDto
    {
        public bool HasOverlap { get; set; }
        public string? Message { get; set; }
    }
}

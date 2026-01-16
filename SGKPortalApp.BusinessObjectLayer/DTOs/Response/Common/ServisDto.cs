namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Servis Response DTO (Dropdown/Select için basitleştirilmiş)
    /// </summary>
    public class ServisDto
    {
        public int ServisId { get; set; }
        public string ServisAdi { get; set; } = string.Empty;
    }
}

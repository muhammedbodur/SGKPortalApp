namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Departman Response DTO (Dropdown/Select için basitleştirilmiş)
    /// </summary>
    public class DepartmanDto
    {
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
    }
}

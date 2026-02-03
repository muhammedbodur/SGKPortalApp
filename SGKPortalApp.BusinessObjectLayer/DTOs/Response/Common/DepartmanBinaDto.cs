namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Bina-Departman ilişkisi için basit DTO
    /// </summary>
    public class DepartmanBinaDto
    {
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public bool AnaBina { get; set; }
    }
}

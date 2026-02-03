namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Dropdown için basitleştirilmiş DepartmanHizmetBinasi DTO
    /// </summary>
    public class DepartmanHizmetBinasiDto
    {
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public string DisplayText => $"{DepartmanAdi} - {HizmetBinasiAdi}";
    }
}

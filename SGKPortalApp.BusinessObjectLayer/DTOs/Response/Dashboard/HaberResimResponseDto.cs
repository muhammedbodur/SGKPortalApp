namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard
{
    public class HaberResimResponseDto
    {
        public int DuyuruResimId { get; set; }
        public string ResimUrl { get; set; } = string.Empty;
        public bool IsVitrin { get; set; }
        public int Sira { get; set; }
    }
}

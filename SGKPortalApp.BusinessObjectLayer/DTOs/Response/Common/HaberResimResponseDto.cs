namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class HaberResimResponseDto
    {
        public int HaberResimId { get; set; }
        public string ResimUrl { get; set; } = string.Empty;
        public bool IsVitrin { get; set; }
        public int Sira { get; set; }
    }
}

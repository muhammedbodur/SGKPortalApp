using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class OnemliLinkResponseDto
    {
        public int LinkId { get; set; }
        public string LinkAdi { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}

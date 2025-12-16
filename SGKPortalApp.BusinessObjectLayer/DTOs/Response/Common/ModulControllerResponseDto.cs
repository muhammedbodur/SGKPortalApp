namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class ModulControllerResponseDto
    {
        public int ModulControllerId { get; set; }
        public string ModulControllerAdi { get; set; } = string.Empty;
        public int ModulId { get; set; }
        public string ModulAdi { get; set; } = string.Empty;
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        public int IslemCount { get; set; }
    }
}

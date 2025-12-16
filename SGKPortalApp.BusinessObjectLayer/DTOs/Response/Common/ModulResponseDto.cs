namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class ModulResponseDto
    {
        public int ModulId { get; set; }
        public string ModulAdi { get; set; } = string.Empty;
        public string ModulKodu { get; set; } = string.Empty;
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        public int ControllerCount { get; set; }
    }
}

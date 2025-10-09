namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class IlResponseDto
    {
        public int IlId { get; set; }
        public string IlAdi { get; set; } = string.Empty;
        public int IlceSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}

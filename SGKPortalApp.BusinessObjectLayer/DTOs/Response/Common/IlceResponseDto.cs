namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class IlceResponseDto
    {
        public int IlceId { get; set; }
        public int IlId { get; set; }
        public string IlAdi { get; set; } = string.Empty;
        public string IlceAdi { get; set; } = string.Empty;
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class YetkiResponseDto
    {
        public int YetkiId { get; set; }
        public string YetkiAdi { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;

        public int? UstYetkiId { get; set; }
        public string? ControllerAdi { get; set; }
        public string? ActionAdi { get; set; }

        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}

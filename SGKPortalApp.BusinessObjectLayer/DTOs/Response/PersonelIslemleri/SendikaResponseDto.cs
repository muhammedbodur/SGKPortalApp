using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class SendikaResponseDto
    {
        public int SendikaId { get; set; }
        public string SendikaAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public int PersonelSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}

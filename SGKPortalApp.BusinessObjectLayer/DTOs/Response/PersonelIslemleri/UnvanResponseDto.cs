using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class UnvanResponseDto
    {
        public int UnvanId { get; set; }
        public string UnvanAdi { get; set; } = string.Empty;
        public Aktiflik Aktiflik { get; set; }
        public int PersonelSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
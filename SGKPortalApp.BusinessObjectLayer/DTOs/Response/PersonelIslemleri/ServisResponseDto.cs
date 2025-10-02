using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class ServisResponseDto
    {
        public int ServisId { get; set; }
        public string ServisAdi { get; set; } = string.Empty;
        public Aktiflik ServisAktiflik { get; set; }
        public int PersonelSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class DepartmanResponseDto
    {
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public Aktiflik DepartmanAktiflik { get; set; }
        public int PersonelSayisi { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
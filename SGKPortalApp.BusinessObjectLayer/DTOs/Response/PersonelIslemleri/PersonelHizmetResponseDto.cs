using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelHizmetResponseDto
    {
        public int PersonelHizmetId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public int DepartmanId { get; set; }
        public string? DepartmanAdi { get; set; }
        public int ServisId { get; set; }
        public string? ServisAdi { get; set; }
        public DateTime GorevBaslamaTarihi { get; set; }
        public DateTime? GorevAyrilmaTarihi { get; set; }
        public HizmetAtamaTipleri HizmetAtamaTipi { get; set; }
        public string? Sebep { get; set; }
    }
}

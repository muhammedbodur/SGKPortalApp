using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelCocukResponseDto
    {
        public int PersonelCocukId { get; set; }
        public string PersonelTcKimlikNo { get; set; } = string.Empty;
        public string CocukAdi { get; set; } = string.Empty;
        public DateOnly CocukDogumTarihi { get; set; }
        public OgrenimDurumu OgrenimDurumu { get; set; }
        public int Yas => DateTime.Now.Year - CocukDogumTarihi.Year;
    }
}

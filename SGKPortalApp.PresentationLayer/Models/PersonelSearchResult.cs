using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Models
{
    public class PersonelSearchResult
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string? SicilNo { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? UnvanAdi { get; set; }
        public string? Resim { get; set; }
        public PersonelAktiflikDurum PersonelAktiflikDurum { get; set; }
    }
}

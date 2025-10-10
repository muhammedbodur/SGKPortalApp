namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelImzaYetkisiResponseDto
    {
        public int PersonelImzaYetkisiId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public int DepartmanId { get; set; }
        public string? DepartmanAdi { get; set; }
        public int ServisId { get; set; }
        public string? ServisAdi { get; set; }
        public string? GorevDegisimSebebi { get; set; }
        public DateTime ImzaYetkisiBaslamaTarihi { get; set; }
        public DateTime? ImzaYetkisiBitisTarihi { get; set; }
        public string? Aciklama { get; set; }
    }
}

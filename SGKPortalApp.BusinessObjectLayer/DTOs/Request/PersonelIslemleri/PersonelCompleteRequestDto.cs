namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri
{
    public class PersonelCompleteRequestDto
    {
        public string? RequestorTcKimlikNo { get; set; }
        public string? RequestorSessionId { get; set; }

        public PersonelCreateRequestDto Personel { get; set; } = new();
        public List<PersonelCocukCreateRequestDto>? Cocuklar { get; set; }
        public List<PersonelHizmetCreateRequestDto>? Hizmetler { get; set; }
        public List<PersonelEgitimCreateRequestDto>? Egitimler { get; set; }
        public List<PersonelImzaYetkisiCreateRequestDto>? ImzaYetkileri { get; set; }
        public List<PersonelCezaCreateRequestDto>? Cezalar { get; set; }
        public List<PersonelEngelCreateRequestDto>? Engeller { get; set; }
    }
}

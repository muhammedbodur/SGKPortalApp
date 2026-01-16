namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri
{
    /// <summary>
    /// Personel Listesi Filtre Request DTO
    /// </summary>
    public class PersonelListFilterRequestDto
    {
        public int? SgmId { get; set; }
        public int? DepartmanId { get; set; }
        public int? ServisId { get; set; }
        public bool? SadeceAktifler { get; set; } = true;
        public string? AramaMetni { get; set; } // Ad soyad veya sicil no ile arama
    }
}

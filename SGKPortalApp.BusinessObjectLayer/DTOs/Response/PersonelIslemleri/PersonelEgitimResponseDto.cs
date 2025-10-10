namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelEgitimResponseDto
    {
        public int PersonelEgitimId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string EgitimAdi { get; set; } = string.Empty;
        public DateTime EgitimBaslangicTarihi { get; set; }
        public DateTime? EgitimBitisTarihi { get; set; }
        public string? Aciklama { get; set; }
    }
}

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Bugün doğan personel bilgisi
    /// </summary>
    public class BugunDoganResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string? Resim { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? ServisAdi { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public int Yas => DogumTarihi.HasValue ? DateTime.Today.Year - DogumTarihi.Value.Year : 0;
    }
}

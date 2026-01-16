namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Personel Listesi Response DTO
    /// </summary>
    public class PersonelListResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public int PersonelKayitNo { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public bool Aktif { get; set; }
        public string? Email { get; set; }
        public string? CepTelefonu { get; set; }
    }
}

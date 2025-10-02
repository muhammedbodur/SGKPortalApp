namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    /// <summary>
    /// Liste görünümü için basitleştirilmiş personel DTO
    /// </summary>
    public class PersonelListResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmanAdi { get; set; } = string.Empty;
        public string ServisAdi { get; set; } = string.Empty;
        public string UnvanAdi { get; set; } = string.Empty;
        public string? CepTelefonu { get; set; }
        public int Dahili { get; set; }
        public string PersonelAktiflikDurum { get; set; } = string.Empty;
    }
}
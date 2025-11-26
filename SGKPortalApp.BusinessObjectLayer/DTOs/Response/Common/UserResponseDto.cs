namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class UserResponseDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        
        // Dinamik Veriler (User tablosundan)
        public bool AktifMi { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public int BasarisizGirisSayisi { get; set; }
        public DateTime? HesapKilitTarihi { get; set; }
        
        // Personel Bilgileri (İlişkili - Statik veriler)
        public string? PersonelAdSoyad { get; set; }
        public string? Email { get; set; }
        public string? CepTelefonu { get; set; }
        public int? SicilNo { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? ServisAdi { get; set; }
        public string? SessionID { get; set; }
        
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}

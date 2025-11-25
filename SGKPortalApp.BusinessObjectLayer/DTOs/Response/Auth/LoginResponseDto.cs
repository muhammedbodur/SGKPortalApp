namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    /// <summary>
    /// Login başarılı olduğunda dönen response
    /// </summary>
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        // Kullanıcı bilgileri
        public string TcKimlikNo { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int ServisId { get; set; }
        public string ServisAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public string? Resim { get; set; }
        
        // Session bilgisi
        public string SessionId { get; set; } = string.Empty;
        
        // JWT Token (API çağrıları için)
        public string? JwtToken { get; set; }
        
        // Kullanıcı tipi (Personel | TvUser)
        public string? UserType { get; set; }
        
        // Yönlendirme URL'i
        public string? RedirectUrl { get; set; }
    }
}

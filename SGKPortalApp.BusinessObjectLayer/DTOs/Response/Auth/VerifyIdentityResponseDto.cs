namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    /// <summary>
    /// Kimlik doğrulama sonucu
    /// </summary>
    public class VerifyIdentityResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Kimlik doğrulandıysa, kullanıcının TC Kimlik No'su
        /// Şifre sıfırlama için kullanılacak
        /// </summary>
        public string? TcKimlikNo { get; set; }
        
        /// <summary>
        /// Kullanıcının adı soyadı (doğrulama mesajında göstermek için)
        /// </summary>
        public string? AdSoyad { get; set; }
    }
}

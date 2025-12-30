namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    /// <summary>
    /// Active Directory validation sonucu
    /// </summary>
    public class ActiveDirectoryValidationResult
    {
        /// <summary>
        /// AD validation başarılı mı?
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Hata/Başarı mesajı
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Bulunan TC Kimlik No (başarılıysa)
        /// </summary>
        public string? TcKimlikNo { get; set; }

        /// <summary>
        /// Bulunan kullanıcı adı ve soyadı (başarılıysa)
        /// </summary>
        public string? AdSoyad { get; set; }

        /// <summary>
        /// Email adresi (başarılıysa)
        /// </summary>
        public string? Email { get; set; }
    }
}

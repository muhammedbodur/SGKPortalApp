namespace SGKPortalApp.Common.Interfaces
{
    /// <summary>
    /// Mevcut kullanıcı bilgilerini sağlar (audit logging için)
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Mevcut kullanıcının TC Kimlik No'sunu döndürür
        /// </summary>
        string? GetTcKimlikNo();

        /// <summary>
        /// Mevcut kullanıcının IP adresini döndürür
        /// </summary>
        string? GetIpAddress();

        /// <summary>
        /// Mevcut kullanıcının User Agent bilgisini döndürür
        /// </summary>
        string? GetUserAgent();

        /// <summary>
        /// Kullanıcı authenticated mi?
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Mevcut kullanıcının Departman ID'sini döndürür
        /// </summary>
        int? GetDepartmanId();

        /// <summary>
        /// Mevcut kullanıcının Servis ID'sini döndürür
        /// </summary>
        int? GetServisId();
    }
}

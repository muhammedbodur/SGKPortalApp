namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Auth
{
    /// <summary>
    /// Windows kullanıcı adını almak için servis
    /// Hem domain ortamında hem de lokal ortamda çalışır
    /// </summary>
    public interface IWindowsUsernameService
    {
        /// <summary>
        /// Windows kullanıcı adını al
        /// Domain ortamında: DOMAIN\username
        /// Lokal ortamda: username
        /// </summary>
        string? GetWindowsUsername();

        /// <summary>
        /// Sadece username kısmını al (domain olmadan)
        /// </summary>
        string? GetUsernameOnly();

        /// <summary>
        /// Bilgisayar domain altında mı kontrol et
        /// </summary>
        bool IsDomainJoined();

        /// <summary>
        /// Domain adını al (domain altındaysa)
        /// </summary>
        string? GetDomainName();
    }
}

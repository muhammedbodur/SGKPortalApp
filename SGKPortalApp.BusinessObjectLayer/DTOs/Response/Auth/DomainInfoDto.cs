namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth
{
    /// <summary>
    /// Domain bilgisi DTO
    /// </summary>
    public class DomainInfoDto
    {
        /// <summary>
        /// Bilgisayar domain altında mı?
        /// </summary>
        public bool IsDomainJoined { get; set; }

        /// <summary>
        /// Domain adı (varsa)
        /// </summary>
        public string? DomainName { get; set; }

        /// <summary>
        /// Windows kullanıcı adı (debug için)
        /// </summary>
        public string? WindowsUsername { get; set; }
    }
}

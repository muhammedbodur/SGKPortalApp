namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Account
{
    /// <summary>
    /// Son oturum bilgisi
    /// </summary>
    public class SessionInfoDto
    {
        public string Browser { get; set; } = string.Empty;
        public string BrowserIcon { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }
}

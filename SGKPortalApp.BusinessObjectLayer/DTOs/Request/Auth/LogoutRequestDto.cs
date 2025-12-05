namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth
{
    /// <summary>
    /// Logout request DTO (opsiyonel - TcKimlikNo gönderebilir)
    /// </summary>
    public class LogoutRequestDto
    {
        public string? TcKimlikNo { get; set; }
    }
}

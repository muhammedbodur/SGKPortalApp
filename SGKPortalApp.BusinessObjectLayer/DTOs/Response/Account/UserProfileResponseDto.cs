namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Account
{
    /// <summary>
    /// Kullanıcı profil bilgileri
    /// </summary>
    public class UserProfileResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}

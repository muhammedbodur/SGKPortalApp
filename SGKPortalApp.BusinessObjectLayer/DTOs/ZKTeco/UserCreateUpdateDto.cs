namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco cihazına kullanıcı ekle/güncelle
    /// ZKTecoApi → UserCreateRequest mapping
    /// </summary>
    public class UserCreateUpdateDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public long? CardNumber { get; set; }
        public int Privilege { get; set; } = 0; // Default: Normal user
        public bool Enabled { get; set; } = true;
    }
}

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz kullanıcı bilgisi
    /// ZKTecoApi → UserInfoResponse mapping
    /// </summary>
    public class UserInfoDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public long? CardNumber { get; set; }
        public int Privilege { get; set; } // 0=User, 1=Enroller, 2=Manager, 3=SuperAdmin
        public bool Enabled { get; set; } = true;
    }
}

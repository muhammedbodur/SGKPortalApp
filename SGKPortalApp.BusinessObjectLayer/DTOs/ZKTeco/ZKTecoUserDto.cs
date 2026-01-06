using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// ZKTeco kullanıcı bilgileri DTO (Listeleme)
    /// </summary>
    public class ZKTecoUserDto
    {
        public int Id { get; set; }
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public long? CardNumber { get; set; }
        public int Privilege { get; set; }
        public bool Enabled { get; set; }
        public int? DeviceId { get; set; }
        public string? DeviceIp { get; set; }
        public string? DeviceName { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Yetki seviyesi metni
        /// </summary>
        public string PrivilegeText => Privilege switch
        {
            0 => "Kullanıcı",
            1 => "Kayıt Yetkilisi",
            2 => "Yönetici",
            3 => "Süper Admin",
            _ => "Bilinmiyor"
        };

        /// <summary>
        /// Durum metni
        /// </summary>
        public string StatusText => Enabled ? "Aktif" : "Pasif";
    }

    /// <summary>
    /// ZKTeco kullanıcı oluşturma DTO
    /// </summary>
    public class CreateZKTecoUserDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public long? CardNumber { get; set; }
        public int Privilege { get; set; } = 0;
        public bool Enabled { get; set; } = true;
        public int? DeviceId { get; set; }
    }

    /// <summary>
    /// ZKTeco kullanıcı güncelleme DTO
    /// </summary>
    public class UpdateZKTecoUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public long? CardNumber { get; set; }
        public int Privilege { get; set; }
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// ZKTecoApi'den gelen kullanıcı bilgisi (external)
    /// </summary>
    public class ZKTecoApiUserDto
    {
        public string EnrollNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public long? CardNumber { get; set; }
        public int Privilege { get; set; }
        public bool Enabled { get; set; }
    }
}

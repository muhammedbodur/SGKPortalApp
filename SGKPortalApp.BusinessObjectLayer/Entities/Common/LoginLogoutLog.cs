using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    [Table("CMN_LoginLogoutLogs")]
    public class LoginLogoutLog : BaseEntity
    {
        [Key]
        [Column("Id")]
        public int LoginLogoutLogId { get; set; }

        [Column("TcKimlikNo")]
        [StringLength(11)]
        public string? TcKimlikNo { get; set; }

        // User ile Many-to-One ilişki
        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("LoginLogoutLogs")]
        public User? User { get; set; }

        [Column("AdSoyad")]
        [StringLength(200)]
        public string? AdSoyad { get; set; }

        [Column("LoginTime")]
        public DateTime LoginTime { get; set; }

        [Column("LogoutTime")]
        public DateTime? LogoutTime { get; set; }

        [Column("SessionID")]
        [StringLength(100)]
        public string? SessionID { get; set; }

        [Column("IpAddress")]
        [StringLength(50)]
        public string? IpAddress { get; set; }

        [Column("UserAgent")]
        [StringLength(500)]
        public string? UserAgent { get; set; }

        [Column("Browser")]
        [StringLength(100)]
        public string? Browser { get; set; }

        [Column("OperatingSystem")]
        [StringLength(100)]
        public string? OperatingSystem { get; set; }

        [Column("DeviceType")]
        [StringLength(50)]
        public string? DeviceType { get; set; }

        [Column("WindowsUsername")]
        [StringLength(100)]
        public string? WindowsUsername { get; set; }

        [Column("LoginSuccessful")]
        public bool LoginSuccessful { get; set; } = true;

        [Column("FailureReason")]
        [StringLength(500)]
        public string? FailureReason { get; set; }
    }
}

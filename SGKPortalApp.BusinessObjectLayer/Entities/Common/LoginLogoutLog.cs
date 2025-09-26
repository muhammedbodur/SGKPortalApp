using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class LoginLogoutLog : BaseEntity
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("TcKimlikNo")]
        public string? TcKimlikNo { get; set; }

        [Column("LoginTime")]
        public DateTime LoginTime { get; set; }

        [Column("LogoutTime")]
        public DateTime? LogoutTime { get; set; }

        [Column("SessionID")]
        public string? SessionID { get; set; }
    }
}

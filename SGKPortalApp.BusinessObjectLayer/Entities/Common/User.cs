using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class User : AuditableEntity
    {
        [Key]
        public string TcKimlikNo { get; set; }
        [Required]
        public string KullaniciAdi { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string SifreHash { get; set; }
        public string? TelefonNo { get; set; }
        public bool AktifMi { get; set; } = true;
        public DateTime? SonGirisTarihi { get; set; }
        public int BasarisizGirisSayisi { get; set; } = 0;
        public DateTime? HesapKilitTarihi { get; set; }
    }
}
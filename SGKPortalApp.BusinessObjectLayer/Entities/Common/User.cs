using System;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class User : AuditableEntity
    {
        [Key]
        public string TcKimlikNo { get; set; }
        [Required]
        public bool AktifMi { get; set; } = false;
        public DateTime? SonGirisTarihi { get; set; }
        public int BasarisizGirisSayisi { get; set; } = 0;
        public DateTime? HesapKilitTarihi { get; set; }
    }
}
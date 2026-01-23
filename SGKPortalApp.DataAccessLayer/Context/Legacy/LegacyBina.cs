using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.DataAccessLayer.Context.Legacy
{
    [Table("binalar")]
    public class LegacyBina
    {
        [Key]
        [Column("kod")]
        public int Kod { get; set; }

        [Column("bina_adi")]
        [StringLength(100)]
        public string BinaAdi { get; set; } = string.Empty;

        [Column("adres")]
        [StringLength(500)]
        public string? Adres { get; set; }
    }
}

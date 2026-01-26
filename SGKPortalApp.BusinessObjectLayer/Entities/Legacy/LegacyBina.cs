using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Legacy
{
    [Table("binalar")]
    public class LegacyBina
    {
        [Key]
        [Column("KOD")]
        public int Kod { get; set; }

        [Column("BINAAD")]
        [StringLength(30)]
        public string BinaAdi { get; set; } = string.Empty;
    }
}

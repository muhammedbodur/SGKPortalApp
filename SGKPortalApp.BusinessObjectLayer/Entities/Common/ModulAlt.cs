using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class ModulAlt : BaseEntity
    {
        [Key]
        public int ModulAltId { get; set; }

        [Required]
        public required string ModulAltAdi { get; set; }

        public int ModulId { get; set; }
        [ForeignKey("ModulId")]
        [InverseProperty("ModulAltlari")]
        public required Modul Modul { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class Modul : AuditableEntity
    {
        [Key]
        public int ModulId { get; set; }

        [Required]
        public required string ModulAdi { get; set; }

        [InverseProperty("Modul")]
        public ICollection<ModulAlt> ModulAltlari { get; set; } = new List<ModulAlt>();

        [InverseProperty("Modul")]
        public ICollection<ModulController> ModulControllers { get; set; } = new List<ModulController>();
    }
}
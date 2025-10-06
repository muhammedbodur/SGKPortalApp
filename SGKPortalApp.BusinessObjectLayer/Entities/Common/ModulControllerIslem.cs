using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class ModulControllerIslem : AuditableEntity
    {
        [Key]
        public int ModulControllerIslemId { get; set; }

        [Required]
        public required string ModulControllerIslemAdi { get; set; }

        public int ModulControllerId { get; set; }
        [ForeignKey("ModulControllerId")]
        [InverseProperty("ModulControllerIslemler")]
        public required ModulController ModulController { get; set; }

        [InverseProperty("ModulControllerIslem")]
        public ICollection<PersonelYetki> PersonelYetkileri { get; set; } = new List<PersonelYetki>();
    }
}
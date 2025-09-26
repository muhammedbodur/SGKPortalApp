using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class ModulController : BaseEntity
    {
        [Key]
        public int ModulControllerId { get; set; }

        [Required]
        public required string ModulControllerAdi { get; set; }

        public int ModulId { get; set; }
        [ForeignKey("ModulId")]
        [InverseProperty("ModulControllers")]
        public required Modul Modul { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("ModulController")]
        public ICollection<ModulControllerIslem> ModulControllerIslemler { get; set; } = new List<ModulControllerIslem>();
    }
}
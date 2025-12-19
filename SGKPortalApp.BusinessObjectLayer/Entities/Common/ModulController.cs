using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class ModulController : AuditableEntity
    {
        [Key]
        public int ModulControllerId { get; set; }

        [Required]
        public required string ModulControllerAdi { get; set; }

        public int ModulId { get; set; }
        [ForeignKey("ModulId")]
        [InverseProperty("ModulControllers")]
        public required Modul Modul { get; set; }

        /// <summary>
        /// Üst controller (parent) - Hiyerarşik yapı için
        /// NULL ise root controller'dır
        /// </summary>
        public int? UstModulControllerId { get; set; }
        [ForeignKey("UstModulControllerId")]
        [InverseProperty("AltModulControllers")]
        public ModulController? UstModulController { get; set; }

        /// <summary>
        /// Alt controller'lar (children) - Hiyerarşik yapı için
        /// </summary>
        [InverseProperty("UstModulController")]
        public ICollection<ModulController> AltModulControllers { get; set; } = new List<ModulController>();

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("ModulController")]
        public ICollection<ModulControllerIslem> ModulControllerIslemler { get; set; } = new List<ModulControllerIslem>();
    }
}
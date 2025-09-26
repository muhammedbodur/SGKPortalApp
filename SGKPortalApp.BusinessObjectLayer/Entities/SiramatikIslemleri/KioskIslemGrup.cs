using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KioskIslemGrup : AuditableEntity
    {
        [Key]
        public int KioskIslemGrupId { get; set; }

        public int KioskGrupId { get; set; }
        [ForeignKey("KioskGrupId")]
        [InverseProperty("KioskIslemGruplari")]
        public required KioskGrup KioskGrup { get; set; }

        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public int KioskIslemGrupSira { get; set; }
        public Aktiflik KioskIslemGrupAktiflik { get; set; } = Aktiflik.Aktif;

        public int KanalAltId { get; set; }
        [ForeignKey("KanalAltId")]
        public required KanalAlt KanalAlt { get; set; }

        [InverseProperty("KioskIslemGrup")]
        public ICollection<KanalAltIslem>? KanalAltIslemleri { get; set; } = new List<KanalAltIslem>();
    }
}
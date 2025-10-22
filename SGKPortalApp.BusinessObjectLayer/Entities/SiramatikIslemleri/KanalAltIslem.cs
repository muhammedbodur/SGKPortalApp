using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KanalAltIslem : AuditableEntity
    {
        [Key]
        public int KanalAltIslemId { get; set; }

        public int KanalAltId { get; set; }
        [ForeignKey("KanalAltId")]
        [InverseProperty("KanalAltIslemleri")]
        public required KanalAlt KanalAlt { get; set; }

        public int HizmetBinasiId { get; set; }
        [ForeignKey("HizmetBinasiId")]
        public required HizmetBinasi HizmetBinasi { get; set; }

        public int KanalIslemId { get; set; }
        [ForeignKey("KanalIslemId")]
        [InverseProperty("KanalAltIslemleri")]
        public required KanalIslem KanalIslem { get; set; }

        public int? KioskIslemGrupId { get; set; }
        [ForeignKey("KioskIslemGrupId")]
        public KioskIslemGrup? KioskIslemGrup { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("KanalAltIslem")]
        public ICollection<Sira>? Siralar { get; set; } = new List<Sira>();

        [InverseProperty("KanalAltIslem")]
        public ICollection<KanalPersonel>? KanalPersonelleri { get; set; } = new List<KanalPersonel>();
    }
}
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KanalAlt : AuditableEntity
    {
        [Key]
        public int KanalAltId { get; set; }

        public required string KanalAltAdi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        public int KanalId { get; set; }
        [ForeignKey("KanalId")]
        [InverseProperty("KanalAltlari")]
        public required Kanal Kanal { get; set; }

        [InverseProperty("KanalAlt")]
        public ICollection<KanalAltIslem>? KanalAltIslemleri { get; set; } = new List<KanalAltIslem>();

        [InverseProperty("KanalAlt")]
        public ICollection<KioskMenuIslem>? KioskMenuIslemleri { get; set; } = new List<KioskMenuIslem>();
    }
}
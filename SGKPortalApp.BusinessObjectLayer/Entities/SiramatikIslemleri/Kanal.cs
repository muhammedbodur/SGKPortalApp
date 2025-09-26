using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Kanal : AuditableEntity
    {
        [Key]
        public int KanalId { get; set; }

        public required string KanalAdi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Kanal")]
        public ICollection<KanalAlt>? KanalAltlari { get; set; } = new List<KanalAlt>();

        [InverseProperty("Kanal")]
        public ICollection<KanalIslem>? KanalIslemleri { get; set; } = new List<KanalIslem>();
    }
}
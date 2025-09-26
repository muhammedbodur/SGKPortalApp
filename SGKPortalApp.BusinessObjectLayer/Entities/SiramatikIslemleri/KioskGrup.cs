using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KioskGrup : AuditableEntity
    {
        [Key]
        public int KioskGrupId { get; set; }

        public required string KioskGrupAdi { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("KioskGrup")]
        public ICollection<KioskIslemGrup>? KioskIslemGruplari { get; set; } = new List<KioskIslemGrup>();
    }
}
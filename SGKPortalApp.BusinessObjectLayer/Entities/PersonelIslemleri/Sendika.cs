using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class Sendika : AuditableEntity
    {
        [Key]
        public int SendikaId { get; set; }

        public required string SendikaAdi { get; set; }
        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Sendika")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
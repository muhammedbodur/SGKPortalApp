using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class Unvan : AuditableEntity
    {
        [Key]
        public int UnvanId { get; set; }

        public required string UnvanAdi { get; set; }

        public int? LegacyKod { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Unvan")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
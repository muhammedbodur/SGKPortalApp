using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class Servis : AuditableEntity
    {
        [Key]
        public int ServisId { get; set; }

        public required string ServisAdi { get; set; }

        public Aktiflik ServisAktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Servis")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
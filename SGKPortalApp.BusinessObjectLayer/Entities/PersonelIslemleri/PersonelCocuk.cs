using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelCocuk : AuditableEntity
    {
        [Key]
        public int PersonelCocukId { get; set; }

        [Required]
        public string PersonelTcKimlikNo { get; set; } = string.Empty;

        [ForeignKey("PersonelTcKimlikNo")]
        [InverseProperty("PersonelCocuklari")]
        public Personel? Personel { get; set; }

        [Required]
        public required string CocukAdi { get; set; }

        public DateOnly CocukDogumTarihi { get; set; }

        public OgrenimDurumu OgrenimDurumu { get; set; }
    }
}
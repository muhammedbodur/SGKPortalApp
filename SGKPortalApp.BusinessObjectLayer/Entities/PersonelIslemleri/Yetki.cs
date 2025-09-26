using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class Yetki : BaseEntity
    {
        [Key]
        public int YetkiId { get; set; }

        // Yetki türü (Ana, Orta, Alt Yetki)
        public required YetkiTurleri YetkiTuru { get; set; }

        [Required]
        [StringLength(100)]
        public string YetkiAdi { get; set; } = null!;

        [StringLength(500)]
        public string Aciklama { get; set; } = null!;

        /*
         UstYetkiId, yetkinin üst yetkisini temsil eder.
         Eğer AnaYetki ise UstYetkiId null olabilir.
        */
        public int? UstYetkiId { get; set; }

        [ForeignKey(nameof(UstYetkiId))]
        [InverseProperty("AltYetkiler")]
        public Yetki? UstYetki { get; set; }

        [InverseProperty("UstYetki")]
        public ICollection<Yetki> AltYetkiler { get; set; } = new List<Yetki>();

        [StringLength(100)]
        public string? ControllerAdi { get; set; }

        [StringLength(100)]
        public string? ActionAdi { get; set; }

        [InverseProperty("Yetki")]
        public ICollection<PersonelYetki> PersonelYetkileri { get; set; } = new List<PersonelYetki>();
    }
}
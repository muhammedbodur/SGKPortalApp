using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class Departman : AuditableEntity
    {
        [Key]
        public int DepartmanId { get; set; }

        [Required]
        [StringLength(150)]
        public required string DepartmanAdi { get; set; }

        public string? DepartmanAdiKisa { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Departman")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();

        [InverseProperty("Departman")]
        public ICollection<HizmetBinasi>? HizmetBinalari { get; set; } = new List<HizmetBinasi>();
    }
}
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

        /// <summary>
        /// Many-to-many ilişki: Bir departman birden fazla binada olabilir
        /// </summary>
        [InverseProperty("Departman")]
        public ICollection<DepartmanHizmetBinasi> DepartmanHizmetBinalari { get; set; } = new List<DepartmanHizmetBinasi>();
    }
}
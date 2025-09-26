using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class AtanmaNedenleri : BaseEntity
    {
        [Key]
        public int AtanmaNedeniId { get; set; }

        [Required]
        [StringLength(200)]
        public required string AtanmaNedeni { get; set; }

        // Navigation Properties
        [InverseProperty("AtanmaNedeni")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
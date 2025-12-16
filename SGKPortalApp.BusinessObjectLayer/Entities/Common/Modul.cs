using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class Modul : AuditableEntity
    {
        [Key]
        public int ModulId { get; set; }

        /// <summary>
        /// Modül tam adı (örn: Personel İşlemleri, Sıramatik İşlemleri)
        /// </summary>
        [Required]
        public required string ModulAdi { get; set; }

        /// <summary>
        /// Modül kısa kodu - Permission key'lerde kullanılır (örn: PER, SIRA, COMMON)
        /// </summary>
        [Required]
        [StringLength(10)]
        public required string ModulKodu { get; set; }

        [InverseProperty("Modul")]
        public ICollection<ModulController> ModulControllers { get; set; } = new List<ModulController>();
    }
}
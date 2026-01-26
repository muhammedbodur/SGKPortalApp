using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class HizmetBinasi : AuditableEntity
    {
        [Key]
        public int HizmetBinasiId { get; set; }

        [Required]
        [StringLength(200)]
        public required string HizmetBinasiAdi { get; set; }

        /// <summary>
        /// MySQL legacy sistemdeki KOD değeri (senkronizasyon için)
        /// </summary>
        public int? LegacyKod { get; set; }

        public string Adres { get; set; } = string.Empty;

        public Aktiflik Aktiflik { get; set; }

        /// <summary>
        /// Many-to-many ilişki: Bir binada birden fazla departman olabilir
        /// </summary>
        [InverseProperty("HizmetBinasi")]
        public ICollection<DepartmanHizmetBinasi> DepartmanHizmetBinalari { get; set; } = new List<DepartmanHizmetBinasi>();

        // Not: Personeller artık doğrudan HizmetBinasi'na bağlı değil.
        // Personel -> DepartmanHizmetBinasi -> HizmetBinasi şeklinde dolaylı ilişki var.

        public ICollection<Device> Devices { get; set; } = new List<Device>();
        
        [InverseProperty("HizmetBinasi")]
        public ICollection<SpecialCard> SpecialCards { get; set; } = new List<SpecialCard>();
    }
}
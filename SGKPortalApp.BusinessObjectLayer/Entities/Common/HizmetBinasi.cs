using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    public class HizmetBinasi
    {
        [Key]
        public int HizmetBinasiId { get; set; }

        [Required]
        [StringLength(200)]
        public required string HizmetBinasiAdi { get; set; }

        public int DepartmanId { get; set; }
        [ForeignKey("DepartmanId")]
        [InverseProperty("HizmetBinalari")]
        public Departman? Departman { get; set; }

        public Aktiflik HizmetBinasiAktiflik { get; set; }

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public DateTime DuzenlenmeTarihi { get; set; } = DateTime.Now;

        [InverseProperty("HizmetBinasi")]
        public ICollection<Banko>? Bankolar { get; set; } = new List<Banko>();

        [InverseProperty("HizmetBinasi")]
        public ICollection<Tv>? Tvler { get; set; } = new List<Tv>();

        [InverseProperty("HizmetBinasi")]
        public ICollection<Personel>? Personeller { get; set; } = new List<Personel>();
    }
}
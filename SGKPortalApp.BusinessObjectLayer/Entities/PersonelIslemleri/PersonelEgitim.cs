using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelEgitim : AuditableEntity
    {
        [Key]
        public int PersonelEgitimId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelEgitimleri")]
        public Personel? Personel { get; set; }

        [Required]
        [StringLength(200)]
        public string EgitimAdi { get; set; } = string.Empty;

        [Required]
        public DateTime EgitimBaslangicTarihi { get; set; }

        public DateTime? EgitimBitisTarihi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

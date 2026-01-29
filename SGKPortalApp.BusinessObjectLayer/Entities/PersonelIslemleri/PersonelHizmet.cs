using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelHizmet : AuditableEntity
    {
        [Key]
        public int PersonelHizmetId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelHizmetleri")]
        public Personel? Personel { get; set; }

        [Required]
        public int DepartmanId { get; set; }
        [ForeignKey(nameof(DepartmanId))]
        public Departman? Departman { get; set; }

        [Required]
        public int ServisId { get; set; }
        [ForeignKey(nameof(ServisId))]
        public Servis? Servis { get; set; }

        [Required]
        public DateTime GorevBaslamaTarihi { get; set; }

        public DateTime? GorevAyrilmaTarihi { get; set; }

        public HizmetAtamaTipleri HizmetAtamaTipi { get; set; }

        [StringLength(500)]
        public string? Sebep { get; set; }
    }
}

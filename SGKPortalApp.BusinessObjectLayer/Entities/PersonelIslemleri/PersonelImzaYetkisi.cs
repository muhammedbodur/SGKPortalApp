using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelImzaYetkisi : AuditableEntity
    {
        [Key]
        public int PersonelImzaYetkisiId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelImzaYetkileri")]
        public Personel? Personel { get; set; }

        [Required]
        public int DepartmanId { get; set; }
        [ForeignKey(nameof(DepartmanId))]
        public Departman? Departman { get; set; }

        [Required]
        public int ServisId { get; set; }
        [ForeignKey(nameof(ServisId))]
        public Servis? Servis { get; set; }

        [StringLength(200)]
        public string? GorevDegisimSebebi { get; set; }

        [Required]
        public DateTime ImzaYetkisiBaslamaTarihi { get; set; }

        public DateTime? ImzaYetkisiBitisTarihi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

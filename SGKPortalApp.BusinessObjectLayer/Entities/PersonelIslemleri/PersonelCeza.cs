using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelCeza : AuditableEntity
    {
        [Key]
        public int PersonelCezaId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelCezalari")]
        public Personel? Personel { get; set; }

        [Required]
        [StringLength(200)]
        public string CezaSebebi { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AltBendi { get; set; }

        [Required]
        public DateTime CezaTarihi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

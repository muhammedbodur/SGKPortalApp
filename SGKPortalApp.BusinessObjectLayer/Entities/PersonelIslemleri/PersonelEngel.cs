using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri
{
    public class PersonelEngel : AuditableEntity
    {
        [Key]
        public int PersonelEngelId { get; set; }

        [Required]
        [StringLength(11)]
        public string TcKimlikNo { get; set; } = string.Empty;

        [ForeignKey(nameof(TcKimlikNo))]
        [InverseProperty("PersonelEngelleri")]
        public Personel? Personel { get; set; }

        [Required]
        public EngelDerecesi EngelDerecesi { get; set; }

        [StringLength(200)]
        public string? EngelNedeni1 { get; set; }

        [StringLength(200)]
        public string? EngelNedeni2 { get; set; }

        [StringLength(200)]
        public string? EngelNedeni3 { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }
    }
}

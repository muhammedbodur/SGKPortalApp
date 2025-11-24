using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    [Table("COM_OnemliLinkler")]
    public class OnemliLink : AuditableEntity
    {
        [Key]
        public int LinkId { get; set; }

        [Required]
        [StringLength(100)]
        public string LinkAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required]
        public int Sira { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

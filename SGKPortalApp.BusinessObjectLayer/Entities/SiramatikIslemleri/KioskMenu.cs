using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class KioskMenu : AuditableEntity
    {
        [Key]
        public int KioskMenuId { get; set; }

        [Required]
        [StringLength(150)]
        public required string MenuAdi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Required]
        public int MenuSira { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("KioskMenu")]
        public ICollection<KioskMenuIslem>? KioskMenuIslemler { get; set; } = new List<KioskMenuIslem>();

        [InverseProperty("KioskMenu")]
        public ICollection<KioskMenuAtama>? MenuAtamalari { get; set; } = new List<KioskMenuAtama>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    public class Kiosk : AuditableEntity
    {
        [Key]
        public int KioskId { get; set; }

        [Required]
        public int HizmetBinasiId { get; set; }
        [ForeignKey(nameof(HizmetBinasiId))]
        public required HizmetBinasi HizmetBinasi { get; set; }

        [Required]
        [StringLength(100)]
        public required string KioskAdi { get; set; }

        [StringLength(16)]
        public string? KioskIp { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;

        [InverseProperty("Kiosk")]
        public ICollection<KioskMenuAtama>? MenuAtamalari { get; set; } = new List<KioskMenuAtama>();
    }
}

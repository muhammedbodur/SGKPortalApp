using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// Menü içeriği ve alt kanal eşleştirmesi
    /// Örnek: "4/B İŞLEMLERİ" menüsü içindeki "4/A HİZMET DÖKÜMÜ" işlemi
    /// </summary>
    public class KioskMenuIslem : AuditableEntity
    {
        [Key]
        public int KioskMenuIslemId { get; set; }

        [Required]
        public int KioskMenuId { get; set; }
        [ForeignKey(nameof(KioskMenuId))]
        public required KioskMenu KioskMenu { get; set; }

        [Required]
        public int KanalAltId { get; set; }
        [ForeignKey(nameof(KanalAltId))]
        public required KanalAlt KanalAlt { get; set; }

        [Required]
        public int MenuSira { get; set; }

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

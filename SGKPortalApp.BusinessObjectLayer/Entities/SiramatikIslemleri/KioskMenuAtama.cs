using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri
{
    /// <summary>
    /// Fiziksel kiosk cihazına menü atama
    /// Örnek: "Zemin Kat" kiosk'una "4/B İŞLEMLERİ" menüsü atanır
    /// </summary>
    public class KioskMenuAtama : AuditableEntity
    {
        [Key]
        public int KioskMenuAtamaId { get; set; }

        [Required]
        public int KioskId { get; set; }
        [ForeignKey(nameof(KioskId))]
        public required Kiosk Kiosk { get; set; }

        [Required]
        public int KioskMenuId { get; set; }
        [ForeignKey(nameof(KioskMenuId))]
        public required KioskMenu KioskMenu { get; set; }

        public DateTime AtamaTarihi { get; set; } = DateTime.Now;

        public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
    }
}

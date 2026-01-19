using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Resmi Tatil Entity
    /// Tablo adı: CMN_ResmiTatiller (Configuration'da tanımlanacak)
    /// </summary>
    public class ResmiTatil : AuditableEntity
    {
        [Key]
        public int TatilId { get; set; }

        /// <summary>
        /// Tatil adı (örn: "Ramazan Bayramı 1. Gün", "Cumhuriyet Bayramı")
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TatilAdi { get; set; } = string.Empty;

        /// <summary>
        /// Tatil tarihi
        /// </summary>
        [Required]
        public DateTime Tarih { get; set; }

        /// <summary>
        /// Tatil yılı (indeksleme için)
        /// </summary>
        [Required]
        public int Yil { get; set; }

        /// <summary>
        /// Tatil tipi (Sabit: 23 Nisan, Dini: Ramazan Bayramı)
        /// </summary>
        [Required]
        public TatilTipi TatilTipi { get; set; }

        /// <summary>
        /// Yarım gün tatil mi? (Arefe günleri için)
        /// </summary>
        public bool YariGun { get; set; } = false;

        /// <summary>
        /// Açıklama/Not
        /// </summary>
        [StringLength(500)]
        public string? Aciklama { get; set; }

        /// <summary>
        /// Nager.Date'ten otomatik senkronize edildi mi?
        /// </summary>
        public bool OtomatikSenkronize { get; set; } = false;
    }
}

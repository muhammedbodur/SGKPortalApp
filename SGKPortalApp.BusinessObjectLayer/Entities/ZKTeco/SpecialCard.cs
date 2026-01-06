using System;
using System.ComponentModel.DataAnnotations;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco özel kartları (Geçici kartlar)
    /// Saatlik izin, vizite, görev kartı gibi kartlar için kullanılır
    /// Tablo adı: ZKTeco_SpecialCard
    /// </summary>
    public class SpecialCard : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Kart tipi (Vizite, Saatlik İzin, Görev, vb.)
        /// </summary>
        [Required]
        public CardType CardType { get; set; }

        /// <summary>
        /// Kart numarası (RFID card number)
        /// </summary>
        [Required]
        public long CardNumber { get; set; }

        /// <summary>
        /// Kart adı/açıklaması
        /// Örn: "İzin Kartı #1", "Görev Kartı A"
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CardName { get; set; } = string.Empty;

        /// <summary>
        /// ZKTeco EnrollNumber (Cihaza kayıt için unique ID)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EnrollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Notlar (kart hakkında ek bilgiler)
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}

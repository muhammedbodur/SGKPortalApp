using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco özel kartları (Personele özel olmayan geçici kartlar)
    /// Saatlik izin, vizite, görev kartı gibi kartlar için kullanılır
    /// Tablo adı: ZKTeco_SpecialCard
    /// </summary>
    [Table("ZKTeco_SpecialCard")]
    public class ZKTecoSpecialCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Kart tipi
        /// Örn: "Saatlik İzin", "Vizite", "Görev Kartı", "Misafir Kartı"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CardType { get; set; } = string.Empty;

        /// <summary>
        /// Kart numarası (RFID card number)
        /// ZKTeco cihazındaki CardNumber ile eşleşir
        /// </summary>
        [Required]
        public long CardNumber { get; set; }

        /// <summary>
        /// Kart adı/açıklaması
        /// Örn: "İzin Kartı #1", "Ziyaretçi Kartı A"
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CardName { get; set; } = string.Empty;

        /// <summary>
        /// ZKTeco EnrollNumber
        /// Cihaza kayıt için unique ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EnrollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Kart aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Geçerlilik başlangıç tarihi (opsiyonel)
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Geçerlilik bitiş tarihi (opsiyonel)
        /// </summary>
        public DateTime? ValidUntil { get; set; }

        /// <summary>
        /// Şu anda bu kartı kullanan personel (opsiyonel)
        /// Kart teslim edildiğinde set edilir
        /// </summary>
        public int? CurrentUserSicilNo { get; set; }

        /// <summary>
        /// Navigation property - Şu anki kullanıcı
        /// </summary>
        [ForeignKey("CurrentUserSicilNo")]
        public virtual PersonelIslemleri.Personel? CurrentUser { get; set; }

        /// <summary>
        /// Kartı kullanan kişinin adı (personel değilse)
        /// Örn: Ziyaretçinin adı
        /// </summary>
        [StringLength(100)]
        public string? TemporaryUserName { get; set; }

        /// <summary>
        /// Notlar (kart hakkında ek bilgiler)
        /// </summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Son cihazlara gönderilme zamanı
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        // ========== Audit ==========

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Kartın kim tarafından oluşturulduğu (opsiyonel)
        /// </summary>
        [StringLength(11)]
        public string? CreatedByTcKimlikNo { get; set; }

        /// <summary>
        /// Geçerli mi kontrol et
        /// </summary>
        [NotMapped]
        public bool IsValid
        {
            get
            {
                if (!IsActive) return false;

                var now = DateTime.Now;

                if (ValidFrom.HasValue && now < ValidFrom.Value)
                    return false;

                if (ValidUntil.HasValue && now > ValidUntil.Value)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Kart durumu metni
        /// </summary>
        [NotMapped]
        public string StatusText
        {
            get
            {
                if (!IsActive) return "Pasif";
                if (!IsValid) return "Süresi Dolmuş";
                if (CurrentUserSicilNo.HasValue || !string.IsNullOrEmpty(TemporaryUserName))
                    return "Kullanımda";
                return "Müsait";
            }
        }
    }
}

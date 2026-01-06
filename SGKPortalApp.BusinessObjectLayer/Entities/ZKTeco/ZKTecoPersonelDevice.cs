using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// Personel-Cihaz mapping tablosu
    /// Hangi personelin hangi cihazda kayıtlı olduğunu takip eder
    /// Tablo adı: ZKTeco_PersonelDevice
    /// </summary>
    [Table("ZKTeco_PersonelDevice")]
    public class ZKTecoPersonelDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Personel Sicil No (FK)
        /// </summary>
        [Required]
        public int PersonelSicilNo { get; set; }

        /// <summary>
        /// Navigation property - İlişkili personel
        /// Personel.PersonelKayitNo → EnrollNumber
        /// Personel.KartNo → CardNumber
        /// </summary>
        [ForeignKey("PersonelSicilNo")]
        public virtual PersonelIslemleri.Personel? Personel { get; set; }

        /// <summary>
        /// Cihaz ID (FK)
        /// </summary>
        [Required]
        public int DeviceId { get; set; }

        /// <summary>
        /// Navigation property - İlişkili cihaz
        /// </summary>
        [ForeignKey("DeviceId")]
        public virtual ZKTecoDevice? Device { get; set; }

        /// <summary>
        /// Bu kayıt aktif mi?
        /// Personel cihazdan silindiğinde false yapılır
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Son senkronizasyon zamanı
        /// Personel en son ne zaman bu cihaza gönderildi
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        /// <summary>
        /// Son senkronizasyon başarılı mı?
        /// </summary>
        public bool? LastSyncSuccess { get; set; }

        /// <summary>
        /// Son senkronizasyon hata mesajı (varsa)
        /// </summary>
        [StringLength(500)]
        public string? LastSyncErrorMessage { get; set; }

        /// <summary>
        /// Senkronizasyon sayısı
        /// Bu personel bu cihaza kaç kez gönderildi
        /// </summary>
        public int SyncCount { get; set; } = 0;

        // ========== Audit ==========

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Durum metni
        /// </summary>
        [NotMapped]
        public string StatusText => IsActive ? "Aktif" : "Silinmiş";

        /// <summary>
        /// Son işlem başarı metni
        /// </summary>
        [NotMapped]
        public string SyncStatusText
        {
            get
            {
                if (!LastSyncSuccess.HasValue) return "Henüz Gönderilmedi";
                return LastSyncSuccess.Value ? "Başarılı" : "Hatalı";
            }
        }
    }
}

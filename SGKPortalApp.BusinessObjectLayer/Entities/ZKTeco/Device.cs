using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz bilgileri
    /// </summary>
    public class Device : AuditableEntity
    {
        [Key]
        public int DeviceId { get; set; }

        [StringLength(250)]
        public string? DeviceName { get; set; }

        /// <summary>
        /// Cihaz IP adresi
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Cihaz port (default: 4370)
        /// </summary>
        [StringLength(50)]
        public string? Port { get; set; } = "4370";

        /// <summary>
        /// Cihaz kodu (opsiyonel unique kod)
        /// </summary>
        [StringLength(50)]
        public string? DeviceCode { get; set; }

        /// <summary>
        /// Cihaz bilgisi (model, firmware vb.)
        /// </summary>
        [StringLength(255)]
        public string? DeviceInfo { get; set; }

        /// <summary>
        /// Cihaz aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Hizmet Binası ID (FK)
        /// Cihazın hangi hizmet binasında olduğu
        /// </summary>
        public int HizmetBinasiId { get; set; }
        [ForeignKey(nameof(HizmetBinasiId))]
        [InverseProperty("Devices")]
        public HizmetBinasi? HizmetBinasi { get; set; }
        // ========== Son İşlem Bilgileri (Attendance çekme) ==========

        /// <summary>
        /// Son attendance çekme zamanı
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        /// <summary>
        /// Toplam attendance çekme sayısı
        /// </summary>
        public int SyncCount { get; set; } = 0;

        /// <summary>
        /// Son attendance çekme başarılı mı?
        /// </summary>
        public bool? LastSyncSuccess { get; set; }

        /// <summary>
        /// Son attendance çekme durumu (mesaj, hata)
        /// </summary>
        public string? LastSyncStatus { get; set; }

        // ========== Cihaz Kontrol Bilgileri (Health check) ==========

        /// <summary>
        /// Son cihaz kontrol zamanı
        /// </summary>
        public DateTime? LastHealthCheckTime { get; set; }

        /// <summary>
        /// Toplam kontrol sayısı
        /// </summary>
        public int HealthCheckCount { get; set; } = 0;

        /// <summary>
        /// Son kontrol başarılı mı?
        /// </summary>
        public bool? LastHealthCheckSuccess { get; set; }

        /// <summary>
        /// Son kontrol durumu
        /// </summary>
        public string? LastHealthCheckStatus { get; set; }

        // ========== Navigation Properties ==========

        /// <summary>
        /// Bu cihazdan çekilen attendance dataları
        /// </summary>
        [InverseProperty("Device")]
        public ICollection<CekilenData> CekilenData { get; set; } = new List<CekilenData>();
    }
}

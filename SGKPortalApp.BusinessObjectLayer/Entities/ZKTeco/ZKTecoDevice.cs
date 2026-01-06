using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco
{
    /// <summary>
    /// ZKTeco cihaz bilgileri
    /// PDKS.Net4.8 tbSgmInfos ile uyumlu
    /// Tablo adı: ZKTeco_Device (Configuration'da tanımlı)
    /// </summary>
    public class ZKTecoDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Cihaz adı (örn: "Ana Giriş", "Yemekhane")
        /// PDKS.Net4.8: sgmAdi
        /// </summary>
        [StringLength(250)]
        public string? DeviceName { get; set; }

        /// <summary>
        /// Cihaz IP adresi
        /// PDKS.Net4.8: sgmIpAdresi
        /// </summary>
        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Cihaz port (default: 4370)
        /// PDKS.Net4.8: sgmPort
        /// </summary>
        [StringLength(50)]
        public string? Port { get; set; } = "4370";

        /// <summary>
        /// Cihaz kodu (opsiyonel unique kod)
        /// PDKS.Net4.8: sgmKodu
        /// </summary>
        [StringLength(50)]
        public string? DeviceCode { get; set; }

        /// <summary>
        /// Cihaz bilgisi (model, firmware vb.)
        /// PDKS.Net4.8: cihaz_bilgi
        /// </summary>
        [StringLength(255)]
        public string? DeviceInfo { get; set; }

        /// <summary>
        /// Cihaz aktif mi?
        /// PDKS.Net4.8: aktif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Hizmet Binası ID (FK)
        /// Cihazın hangi hizmet binasında olduğu
        /// </summary>
        public int? HizmetBinasiId { get; set; }

        /// <summary>
        /// Navigation property - İlişkili hizmet binası
        /// </summary>
        [ForeignKey("HizmetBinasiId")]
        public virtual Common.HizmetBinasi? HizmetBinasi { get; set; }

        // ========== Son İşlem Bilgileri (Attendance çekme) ==========

        /// <summary>
        /// Son attendance çekme zamanı
        /// PDKS.Net4.8: islemZamani
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        /// <summary>
        /// Toplam attendance çekme sayısı
        /// PDKS.Net4.8: islemSayisi
        /// </summary>
        public int SyncCount { get; set; } = 0;

        /// <summary>
        /// Son attendance çekme başarılı mı?
        /// PDKS.Net4.8: islemBasari
        /// </summary>
        public bool? LastSyncSuccess { get; set; }

        /// <summary>
        /// Son attendance çekme durumu (mesaj, hata)
        /// PDKS.Net4.8: durum
        /// </summary>
        public string? LastSyncStatus { get; set; }

        // ========== Cihaz Kontrol Bilgileri (Health check) ==========

        /// <summary>
        /// Son cihaz kontrol zamanı
        /// PDKS.Net4.8: kontrolZamani
        /// </summary>
        public DateTime? LastHealthCheckTime { get; set; }

        /// <summary>
        /// Toplam kontrol sayısı
        /// PDKS.Net4.8: kontrolSayisi
        /// </summary>
        public int HealthCheckCount { get; set; } = 0;

        /// <summary>
        /// Son kontrol başarılı mı?
        /// PDKS.Net4.8: kontrolBasari
        /// </summary>
        public bool? LastHealthCheckSuccess { get; set; }

        /// <summary>
        /// Son kontrol durumu
        /// PDKS.Net4.8: kontrolDurum
        /// </summary>
        public string? LastHealthCheckStatus { get; set; }

        // ========== Audit ==========

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

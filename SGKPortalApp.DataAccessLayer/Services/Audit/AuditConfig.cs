using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Bir entity için audit logging konfigürasyonu
    /// </summary>
    public class AuditConfig
    {
        /// <summary>
        /// Disabled config (NoAuditLog attribute için)
        /// </summary>
        public static AuditConfig Disabled => new AuditConfig { IsEnabled = false };

        /// <summary>
        /// Audit logging aktif mi?
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// INSERT işlemlerini logla
        /// </summary>
        public bool LogInsert { get; set; } = true;

        /// <summary>
        /// UPDATE işlemlerini logla
        /// </summary>
        public bool LogUpdate { get; set; } = true;

        /// <summary>
        /// DELETE işlemlerini logla
        /// </summary>
        public bool LogDelete { get; set; } = true;

        /// <summary>
        /// Storage stratejisi
        /// </summary>
        public StorageStrategy StorageStrategy { get; set; } = StorageStrategy.SmartHybrid;

        /// <summary>
        /// Hybrid threshold (byte)
        /// </summary>
        public int HybridThresholdBytes { get; set; } = 1024;

        /// <summary>
        /// Bulk threshold (kaç kayıttan fazlası summary log olsun)
        /// </summary>
        public int BulkThreshold { get; set; } = 50;

        /// <summary>
        /// İlişkili değişiklikleri grupla
        /// </summary>
        public bool GroupRelatedChanges { get; set; } = true;

        /// <summary>
        /// Hassas veri içeren property'ler
        /// </summary>
        public HashSet<string> SensitiveProperties { get; set; } = new();

        /// <summary>
        /// Hassas veri konfigürasyonları (property name → config)
        /// </summary>
        public Dictionary<string, SensitiveDataConfig> SensitiveDataConfigs { get; set; } = new();
    }

    /// <summary>
    /// Hassas veri için konfigürasyon
    /// </summary>
    public class SensitiveDataConfig
    {
        public string MaskFormat { get; set; } = "***";
        public bool ExcludeFromLog { get; set; } = false;
        public int? ShowFirstChars { get; set; }
        public int? ShowLastChars { get; set; }
    }
}

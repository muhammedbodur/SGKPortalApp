using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.Options
{
    /// <summary>
    /// Audit logging system konfigürasyonu (appsettings.json'dan gelir)
    /// </summary>
    public class AuditLoggingOptions
    {
        public const string SectionName = "AuditLogging";

        /// <summary>
        /// Audit logging sistemi aktif mi?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Audit log dosyalarının saklanacağı base path
        /// </summary>
        public string BasePath { get; set; } = "/AuditLogs";

        /// <summary>
        /// Varsayılan storage stratejisi
        /// </summary>
        public StorageStrategy DefaultStorageStrategy { get; set; } = StorageStrategy.SmartHybrid;

        /// <summary>
        /// SmartHybrid stratejisinde threshold (byte cinsinden)
        /// </summary>
        public int HybridThresholdBytes { get; set; } = 1024; // 1 KB

        /// <summary>
        /// Retention (saklama) ayarları
        /// </summary>
        public RetentionOptions Retention { get; set; } = new();

        /// <summary>
        /// Grouping (gruplama) ayarları
        /// </summary>
        public GroupingOptions Grouping { get; set; } = new();

        /// <summary>
        /// Maintenance (bakım) ayarları
        /// </summary>
        public MaintenanceOptions Maintenance { get; set; } = new();

        /// <summary>
        /// Performance (performans) ayarları
        /// </summary>
        public PerformanceOptions Performance { get; set; } = new();
    }

    public class RetentionOptions
    {
        /// <summary>
        /// Database'de kaç gün saklanacak?
        /// </summary>
        public int DatabaseDays { get; set; } = 30;

        /// <summary>
        /// Dosyalarda kaç gün saklanacak?
        /// </summary>
        public int FileDays { get; set; } = 365;

        /// <summary>
        /// Kaç gün sonra dosyalar sıkıştırılacak?
        /// </summary>
        public int CompressAfterDays { get; set; } = 1;
    }

    public class GroupingOptions
    {
        /// <summary>
        /// Transaction-based grouping aktif mi?
        /// </summary>
        public bool EnableTransactionGrouping { get; set; } = true;

        /// <summary>
        /// Bulk işlemlerde kaç kayıttan fazlası summary log olsun?
        /// </summary>
        public int BulkThreshold { get; set; } = 50;

        /// <summary>
        /// İlişkili değişiklikleri grupla
        /// </summary>
        public bool GroupRelatedChanges { get; set; } = true;
    }

    public class MaintenanceOptions
    {
        /// <summary>
        /// Background maintenance service aktif mi?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Bakım işlemi çalışma saati (24 saat formatında, örnek: "02:00")
        /// </summary>
        public string RunTime { get; set; } = "02:00";

        /// <summary>
        /// Aylık arşiv oluşturulsun mu?
        /// </summary>
        public bool CreateMonthlyArchive { get; set; } = true;

        /// <summary>
        /// Otomatik sıkıştırma aktif mi?
        /// </summary>
        public bool AutoCompression { get; set; } = true;
    }

    public class PerformanceOptions
    {
        /// <summary>
        /// Reflection cache aktif mi?
        /// </summary>
        public bool EnableReflectionCache { get; set; } = true;

        /// <summary>
        /// Async file writing aktif mi?
        /// </summary>
        public bool EnableAsyncFileWriting { get; set; } = true;

        /// <summary>
        /// Batch writing için batch boyutu
        /// </summary>
        public int BatchSize { get; set; } = 100;

        /// <summary>
        /// File write queue kapasitesi
        /// </summary>
        public int WriteQueueCapacity { get; set; } = 1000;
    }
}

using System;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Entity audit konfigürasyonlarını cache'ler (reflection overhead'i önlemek için)
    /// </summary>
    public interface IAuditConfigurationCache
    {
        /// <summary>
        /// Entity tipi için audit konfigürasyonunu döndürür (cache'den)
        /// </summary>
        AuditConfig GetConfig(Type entityType);

        /// <summary>
        /// Cache'i temizler
        /// </summary>
        void ClearCache();
    }

    /// <summary>
    /// Entity için audit konfigürasyonu
    /// </summary>
    public class AuditConfig
    {
        public bool IsAuditable { get; set; } = true;
        public bool LogInsert { get; set; } = true;
        public bool LogUpdate { get; set; } = true;
        public bool LogDelete { get; set; } = true;
        public SGKPortalApp.BusinessObjectLayer.Enums.Common.StorageStrategy StorageStrategy { get; set; }
        public int HybridThresholdBytes { get; set; } = 1024;
        public int BulkThreshold { get; set; } = 50;
        public bool GroupRelatedChanges { get; set; } = true;

        /// <summary>
        /// Hassas veri property'leri (maskelenecek)
        /// </summary>
        public HashSet<string> SensitiveProperties { get; set; } = new();

        /// <summary>
        /// Property bazında maskeleme bilgileri
        /// </summary>
        public Dictionary<string, SensitiveDataConfig> SensitiveDataConfigs { get; set; } = new();

        public static AuditConfig Default => new();
        public static AuditConfig Disabled => new() { IsAuditable = false };
    }

    /// <summary>
    /// Property için hassas veri konfigürasyonu
    /// </summary>
    public class SensitiveDataConfig
    {
        public string MaskFormat { get; set; } = "***";
        public bool ExcludeFromLog { get; set; } = false;
        public int? ShowFirstChars { get; set; }
        public int? ShowLastChars { get; set; }
    }
}

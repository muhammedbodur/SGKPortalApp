using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessObjectLayer.Attributes;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    public class AuditConfigurationCache : IAuditConfigurationCache
    {
        private readonly AuditLoggingOptions _options;
        private static readonly ConcurrentDictionary<Type, AuditConfig> _cache = new();

        public AuditConfigurationCache(IOptions<AuditLoggingOptions> options)
        {
            _options = options.Value;
        }

        public AuditConfig GetConfig(Type entityType)
        {
            return _cache.GetOrAdd(entityType, BuildConfig);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }

        private AuditConfig BuildConfig(Type entityType)
        {
            // NoAuditLog attribute kontrolü
            var noAuditAttr = entityType.GetCustomAttribute<NoAuditLogAttribute>();
            if (noAuditAttr != null)
                return AuditConfig.Disabled;

            // AuditableEntity'den türemiyorsa loglama (opsiyonel)
            var isAuditableEntity = typeof(AuditableEntity).IsAssignableFrom(entityType);

            // AuditLog attribute'unu oku
            var auditAttr = entityType.GetCustomAttribute<AuditLogAttribute>();

            if (auditAttr == null && !isAuditableEntity)
            {
                // Attribute yok ve AuditableEntity değil → Loglama
                return AuditConfig.Disabled;
            }

            var config = new AuditConfig();

            if (auditAttr != null)
            {
                // Attribute'dan konfigürasyon al
                config.LogInsert = auditAttr.Insert;
                config.LogUpdate = auditAttr.Update;
                config.LogDelete = auditAttr.Delete;
                config.StorageStrategy = auditAttr.StorageStrategy;
                config.HybridThresholdBytes = auditAttr.HybridThresholdBytes;
                config.BulkThreshold = auditAttr.BulkThreshold;
                config.GroupRelatedChanges = auditAttr.GroupRelatedChanges;
            }
            else
            {
                // Varsayılan konfigürasyon (global options'dan)
                config.StorageStrategy = _options.DefaultStorageStrategy;
                config.HybridThresholdBytes = _options.HybridThresholdBytes;
                config.BulkThreshold = _options.Grouping.BulkThreshold;
                config.GroupRelatedChanges = _options.Grouping.GroupRelatedChanges;
            }

            // Hassas veri property'lerini bul
            var sensitiveProps = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<SensitiveDataAttribute>() != null)
                .ToList();

            foreach (var prop in sensitiveProps)
            {
                var sensitiveAttr = prop.GetCustomAttribute<SensitiveDataAttribute>()!;
                config.SensitiveProperties.Add(prop.Name);
                config.SensitiveDataConfigs[prop.Name] = new SensitiveDataConfig
                {
                    MaskFormat = sensitiveAttr.MaskFormat,
                    ExcludeFromLog = sensitiveAttr.ExcludeFromLog,
                    ShowFirstChars = sensitiveAttr.ShowFirstChars,
                    ShowLastChars = sensitiveAttr.ShowLastChars
                };
            }

            return config;
        }
    }
}

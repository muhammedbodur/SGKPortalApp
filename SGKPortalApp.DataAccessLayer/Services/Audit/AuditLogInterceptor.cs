using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.BusinessLogicLayer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// EF Core SaveChanges işlemlerini intercept ederek audit log'ları oluşturur.
    /// Hybrid storage (DB + File) ve transaction grouping destekler.
    /// </summary>
    public class AuditLogInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditConfigurationCache _configCache;
        private readonly IAuditFileWriter _fileWriter;
        private readonly AuditLoggingOptions _options;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        public AuditLogInterceptor(
            ICurrentUserService currentUserService,
            IAuditConfigurationCache configCache,
            IAuditFileWriter fileWriter,
            IOptions<AuditLoggingOptions> options)
        {
            _currentUserService = currentUserService;
            _configCache = configCache;
            _fileWriter = fileWriter;
            _options = options.Value;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled || eventData.Context == null)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var context = eventData.Context;
            var changeTracker = context.ChangeTracker;

            // Transaction context varsa SaveChanges sayacını artır
            if (TransactionContext.IsActive)
            {
                TransactionContext.IncrementSaveChangesCount();
            }

            // Değişen entity'leri topla
            var entries = changeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted)
                .ToList();

            var logsToCreate = new List<DatabaseLog>();

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();

                // DatabaseLog'un kendisini loglama (sonsuz döngü engelleme)
                if (entityType == typeof(DatabaseLog))
                    continue;

                // Configuration'ı cache'den al
                var config = _configCache.GetConfig(entityType);

                // Loglama disabled ise skip et
                if (!config.IsEnabled)
                    continue;

                // Action'a göre loglama kontrolü
                var shouldLog = entry.State switch
                {
                    EntityState.Added => config.LogInsert,
                    EntityState.Modified => config.LogUpdate,
                    EntityState.Deleted => config.LogDelete,
                    _ => false
                };

                if (!shouldLog)
                    continue;

                // Soft delete kontrolü
                var isSoftDelete = IsSoftDelete(entry);

                // Audit log oluştur
                var log = await CreateAuditLogAsync(entry, config, isSoftDelete, cancellationToken);

                if (log != null)
                {
                    logsToCreate.Add(log);
                }
            }

            // Transaction grouping varsa buffer'a ekle, yoksa direkt DbSet'e ekle
            if (TransactionContext.IsActive)
            {
                foreach (var log in logsToCreate)
                {
                    TransactionContext.AddLog(log);
                }
            }
            else
            {
                // Transaction yok, direkt ekle
                foreach (var log in logsToCreate)
                {
                    context.Set<DatabaseLog>().Add(log);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Audit log entity'si oluşturur
        /// </summary>
        private async Task<DatabaseLog?> CreateAuditLogAsync(
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
            AuditConfig config,
            bool isSoftDelete,
            CancellationToken cancellationToken)
        {
            var entityType = entry.Entity.GetType();
            var tableName = entry.Metadata.GetTableName() ?? entityType.Name;

            // Before ve After state'leri al
            var beforeData = GetBeforeData(entry);
            var afterData = GetAfterData(entry);

            // Sensitive data masking uygula
            ApplySensitiveDataMasking(beforeData, config);
            ApplySensitiveDataMasking(afterData, config);

            // JSON'a serialize et
            var beforeJson = beforeData != null ? JsonSerializer.Serialize(beforeData, _jsonOptions) : null;
            var afterJson = afterData != null ? JsonSerializer.Serialize(afterData, _jsonOptions) : null;

            // Database action belirle
            var action = entry.State switch
            {
                EntityState.Added => DatabaseAction.INSERT,
                EntityState.Modified => isSoftDelete ? DatabaseAction.DELETE : DatabaseAction.UPDATE,
                EntityState.Deleted => DatabaseAction.DELETE,
                _ => DatabaseAction.SELECT
            };

            // Changed fields'ı hesapla
            var changedFields = GetChangedFields(entry);

            // Data size hesapla
            var dataSize = (beforeJson?.Length ?? 0) + (afterJson?.Length ?? 0);

            // Storage strategy'ye göre karar ver
            var storageType = DetermineStorageType(dataSize, config.StorageStrategy);

            // User bilgilerini al
            var tcKimlikNo = _currentUserService.GetTcKimlikNo();
            var ipAddress = _currentUserService.GetIpAddress();
            var userAgent = _currentUserService.GetUserAgent();

            var log = new DatabaseLog
            {
                TcKimlikNo = tcKimlikNo,
                TabloAdi = tableName,
                IslemTuru = action,
                IslemZamani = DateTime.UtcNow,
                StorageType = storageType,
                TransactionId = TransactionContext.TransactionId,
                IsGroupedLog = false, // Grouped log'lar ayrıca oluşturulur
                ChangedFields = changedFields,
                ChangedFieldCount = changedFields?.Split(',').Length ?? 0,
                DataSizeBytes = dataSize,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            // Storage type'a göre data'yı yerleştir
            if (storageType == LogStorageType.Database)
            {
                // Database'e yaz
                log.BeforeData = beforeJson;
                log.AfterData = afterJson;
            }
            else if (storageType == LogStorageType.File)
            {
                // File'a yaz
                var fileEntry = new AuditFileEntry
                {
                    LogId = 0, // Henüz ID yok
                    TransactionId = TransactionContext.TransactionId,
                    TcKimlikNo = tcKimlikNo,
                    TableName = tableName,
                    Action = action.ToString(),
                    Timestamp = log.IslemZamani,
                    Before = beforeData,
                    After = afterData,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ChangedFields"] = changedFields ?? "",
                        ["IpAddress"] = ipAddress ?? "",
                        ["UserAgent"] = userAgent ?? ""
                    }
                };

                var fileReference = await _fileWriter.WriteAsync(fileEntry);
                log.FileReference = fileReference;
            }

            return log;
        }

        /// <summary>
        /// Soft delete kontrolü (SilindiMi = true)
        /// </summary>
        private bool IsSoftDelete(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            if (entry.State != EntityState.Modified)
                return false;

            var silindiMiProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "SilindiMi");
            if (silindiMiProperty == null)
                return false;

            var originalValue = silindiMiProperty.OriginalValue as bool?;
            var currentValue = silindiMiProperty.CurrentValue as bool?;

            return originalValue == false && currentValue == true;
        }

        /// <summary>
        /// Before data'yı dictionary olarak döner
        /// </summary>
        private Dictionary<string, object?>? GetBeforeData(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
                return null;

            var data = new Dictionary<string, object?>();

            foreach (var prop in entry.Properties)
            {
                // Navigation properties hariç
                if (prop.Metadata.IsShadowProperty())
                    continue;

                data[prop.Metadata.Name] = entry.State == EntityState.Deleted
                    ? prop.CurrentValue
                    : prop.OriginalValue;
            }

            return data;
        }

        /// <summary>
        /// After data'yı dictionary olarak döner
        /// </summary>
        private Dictionary<string, object?>? GetAfterData(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted)
                return null;

            var data = new Dictionary<string, object?>();

            foreach (var prop in entry.Properties)
            {
                // Navigation properties hariç
                if (prop.Metadata.IsShadowProperty())
                    continue;

                data[prop.Metadata.Name] = prop.CurrentValue;
            }

            return data;
        }

        /// <summary>
        /// Değişen field'ları virgülle ayrılmış string olarak döner
        /// </summary>
        private string? GetChangedFields(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                return null;

            var changedFields = entry.Properties
                .Where(p => p.IsModified && !p.Metadata.IsShadowProperty())
                .Select(p => p.Metadata.Name)
                .ToList();

            return changedFields.Any() ? string.Join(",", changedFields) : null;
        }

        /// <summary>
        /// Sensitive data masking uygular
        /// </summary>
        private void ApplySensitiveDataMasking(Dictionary<string, object?>? data, AuditConfig config)
        {
            if (data == null || !config.SensitiveProperties.Any())
                return;

            foreach (var propName in config.SensitiveProperties)
            {
                if (!data.ContainsKey(propName))
                    continue;

                var sensitiveConfig = config.SensitiveDataConfigs[propName];

                // Exclude ise log'dan çıkar
                if (sensitiveConfig.ExcludeFromLog)
                {
                    data.Remove(propName);
                    continue;
                }

                // Masking uygula
                var value = data[propName]?.ToString();
                if (string.IsNullOrEmpty(value))
                    continue;

                var maskedValue = ApplyMasking(value, sensitiveConfig);
                data[propName] = maskedValue;
            }
        }

        /// <summary>
        /// Masking uygular
        /// </summary>
        private string ApplyMasking(string value, SensitiveDataConfig config)
        {
            if (config.ShowFirstChars.HasValue && config.ShowLastChars.HasValue)
            {
                var firstChars = config.ShowFirstChars.Value;
                var lastChars = config.ShowLastChars.Value;

                if (value.Length <= firstChars + lastChars)
                    return config.MaskFormat;

                var first = value.Substring(0, firstChars);
                var last = value.Substring(value.Length - lastChars);
                return $"{first}{config.MaskFormat}{last}";
            }
            else if (config.ShowFirstChars.HasValue)
            {
                var firstChars = config.ShowFirstChars.Value;
                if (value.Length <= firstChars)
                    return config.MaskFormat;

                var first = value.Substring(0, firstChars);
                return $"{first}{config.MaskFormat}";
            }
            else if (config.ShowLastChars.HasValue)
            {
                var lastChars = config.ShowLastChars.Value;
                if (value.Length <= lastChars)
                    return config.MaskFormat;

                var last = value.Substring(value.Length - lastChars);
                return $"{config.MaskFormat}{last}";
            }

            return config.MaskFormat;
        }

        /// <summary>
        /// Storage strategy'ye göre LogStorageType belirler
        /// </summary>
        private LogStorageType DetermineStorageType(int dataSize, StorageStrategy strategy)
        {
            return strategy switch
            {
                StorageStrategy.AlwaysDatabase => LogStorageType.Database,
                StorageStrategy.AlwaysFile => LogStorageType.File,
                StorageStrategy.SmartHybrid => dataSize < _options.HybridThresholdBytes
                    ? LogStorageType.Database
                    : LogStorageType.File,
                _ => LogStorageType.Database
            };
        }
    }
}

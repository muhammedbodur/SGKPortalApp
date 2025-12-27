using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Services.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    /// <summary>
    /// Audit log görüntüleme servisi
    /// Database ve file-based log'ları birleştirir
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogQueryRepository _auditLogQueryRepository;
        private readonly IAuditFileWriter _fileWriter;
        private readonly AuditLoggingOptions _options;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogService(
            IAuditLogQueryRepository auditLogQueryRepository,
            IAuditFileWriter fileWriter,
            IOptions<AuditLoggingOptions> options,
            ICurrentUserService currentUserService)
        {
            _auditLogQueryRepository = auditLogQueryRepository;
            _fileWriter = fileWriter;
            _options = options.Value;
            _currentUserService = currentUserService;
        }

        public async Task<AuditLogPagedResultDto> GetLogsAsync(AuditLogFilterDto filter)
        {
            // Get current user's departman/servis for permission-based filtering
            var currentUserDepartmanId = _currentUserService.GetDepartmanId();
            var currentUserServisId = _currentUserService.GetServisId();

            // Call repository for complex query with permission filtering
            var (databaseLogs, totalCount) = await _auditLogQueryRepository.GetLogsWithPermissionFilterAsync(
                filter,
                currentUserDepartmanId,
                currentUserServisId);

            // Map to DTO
            var logs = databaseLogs.Select(l => new AuditLogDto
            {
                DatabaseLogId = l.DatabaseLogId,
                TcKimlikNo = l.TcKimlikNo,
                TabloAdi = l.TabloAdi,
                IslemTuru = l.IslemTuru,
                IslemZamani = l.IslemZamani,
                StorageType = l.StorageType,
                FileReference = l.FileReference,
                TransactionId = l.TransactionId,
                IsGroupedLog = l.IsGroupedLog,
                ChangedFields = l.ChangedFields,
                ChangedFieldCount = l.ChangedFieldCount,
                DataSizeBytes = l.DataSizeBytes,
                IpAddress = l.IpAddress,
                UserAgent = l.UserAgent,
                BulkOperationCount = l.BulkOperationCount
            }).ToList();

            return new AuditLogPagedResultDto
            {
                Logs = logs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<AuditLogDetailDto?> GetLogDetailAsync(int logId)
        {
            // Get current user's departman/servis for permission check
            var currentUserDepartmanId = _currentUserService.GetDepartmanId();
            var currentUserServisId = _currentUserService.GetServisId();

            // Get log with permission check from repository
            var log = await _auditLogQueryRepository.GetLogWithPermissionCheckAsync(
                logId,
                currentUserDepartmanId,
                currentUserServisId);

            if (log == null)
                return null; // Log bulunamadı veya yetkisiz erişim

            var detail = new AuditLogDetailDto
            {
                DatabaseLogId = log.DatabaseLogId,
                TcKimlikNo = log.TcKimlikNo,
                TabloAdi = log.TabloAdi,
                IslemTuru = log.IslemTuru,
                IslemZamani = log.IslemZamani,
                StorageType = log.StorageType,
                FileReference = log.FileReference,
                TransactionId = log.TransactionId,
                IsGroupedLog = log.IsGroupedLog,
                ChangedFields = log.ChangedFields,
                ChangedFieldCount = log.ChangedFieldCount,
                DataSizeBytes = log.DataSizeBytes,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent
            };

            // Before/After data'yı al
            if (log.StorageType == LogStorageType.Database)
            {
                detail.BeforeDataJson = log.BeforeData;
                detail.AfterDataJson = log.AfterData;
            }
            else if (log.StorageType == LogStorageType.File && !string.IsNullOrEmpty(log.FileReference))
            {
                var fileEntry = await _fileWriter.ReadAsync(log.FileReference);
                if (fileEntry != null)
                {
                    detail.BeforeDataJson = fileEntry.Before != null
                        ? JsonSerializer.Serialize(fileEntry.Before)
                        : null;
                    detail.AfterDataJson = fileEntry.After != null
                        ? JsonSerializer.Serialize(fileEntry.After)
                        : null;
                }
            }

            // Field changes'i parse et
            detail.FieldChanges = ParseFieldChanges(detail.BeforeDataJson, detail.AfterDataJson);

            // Transaction içindeki diğer log'ları al
            if (log.TransactionId.HasValue)
            {
                detail.RelatedLogs = await _dbContext.DatabaseLogs
                    .Where(l => l.TransactionId == log.TransactionId && l.DatabaseLogId != logId)
                    .OrderBy(l => l.IslemZamani)
                    .Select(l => new AuditLogDto
                    {
                        DatabaseLogId = l.DatabaseLogId,
                        TcKimlikNo = l.TcKimlikNo,
                        TabloAdi = l.TabloAdi,
                        IslemTuru = l.IslemTuru,
                        IslemZamani = l.IslemZamani,
                        StorageType = l.StorageType,
                        FileReference = l.FileReference,
                        TransactionId = l.TransactionId,
                        IsGroupedLog = l.IsGroupedLog,
                        ChangedFields = l.ChangedFields
                    })
                    .ToListAsync();
            }

            return detail;
        }

        public async Task<List<AuditLogDto>> GetTransactionLogsAsync(Guid transactionId)
        {
            return await _dbContext.DatabaseLogs
                .Where(l => l.TransactionId == transactionId)
                .OrderBy(l => l.IslemZamani)
                .Select(l => new AuditLogDto
                {
                    DatabaseLogId = l.DatabaseLogId,
                    TcKimlikNo = l.TcKimlikNo,
                    TabloAdi = l.TabloAdi,
                    IslemTuru = l.IslemTuru,
                    IslemZamani = l.IslemZamani,
                    StorageType = l.StorageType,
                    FileReference = l.FileReference,
                    TransactionId = l.TransactionId,
                    IsGroupedLog = l.IsGroupedLog,
                    ChangedFields = l.ChangedFields,
                    ChangedFieldCount = l.ChangedFieldCount
                })
                .ToListAsync();
        }

        public async Task<List<AuditLogDto>> GetUserRecentLogsAsync(string tcKimlikNo, int count = 50)
        {
            return await _dbContext.DatabaseLogs
                .Where(l => l.TcKimlikNo == tcKimlikNo)
                .OrderByDescending(l => l.IslemZamani)
                .Take(count)
                .Select(l => new AuditLogDto
                {
                    DatabaseLogId = l.DatabaseLogId,
                    TcKimlikNo = l.TcKimlikNo,
                    TabloAdi = l.TabloAdi,
                    IslemTuru = l.IslemTuru,
                    IslemZamani = l.IslemZamani,
                    StorageType = l.StorageType,
                    FileReference = l.FileReference,
                    TransactionId = l.TransactionId,
                    IsGroupedLog = l.IsGroupedLog,
                    ChangedFields = l.ChangedFields
                })
                .ToListAsync();
        }

        public async Task<List<AuditLogDto>> GetEntityHistoryAsync(string tableName, string entityId)
        {
            // Entity ID'yi içeren değişiklikleri bul
            // NOT: Bu basit bir implementasyon - production'da daha gelişmiş arama gerekebilir
            return await _dbContext.DatabaseLogs
                .Where(l => l.TabloAdi == tableName)
                .Where(l => (l.BeforeData != null && l.BeforeData.Contains(entityId)) ||
                           (l.AfterData != null && l.AfterData.Contains(entityId)))
                .OrderBy(l => l.IslemZamani)
                .Select(l => new AuditLogDto
                {
                    DatabaseLogId = l.DatabaseLogId,
                    TcKimlikNo = l.TcKimlikNo,
                    TabloAdi = l.TabloAdi,
                    IslemTuru = l.IslemTuru,
                    IslemZamani = l.IslemZamani,
                    StorageType = l.StorageType,
                    FileReference = l.FileReference,
                    TransactionId = l.TransactionId,
                    IsGroupedLog = l.IsGroupedLog,
                    ChangedFields = l.ChangedFields
                })
                .ToListAsync();
        }

        #region Helper Methods

        /// <summary>
        /// Before/After JSON'larını parse ederek field changes listesi oluşturur
        /// </summary>
        private List<FieldChangeDto> ParseFieldChanges(string? beforeJson, string? afterJson)
        {
            var changes = new List<FieldChangeDto>();

            try
            {
                if (string.IsNullOrWhiteSpace(beforeJson) && string.IsNullOrWhiteSpace(afterJson))
                    return changes;

                var beforeDict = !string.IsNullOrWhiteSpace(beforeJson)
                    ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(beforeJson)
                    : new Dictionary<string, JsonElement>();

                var afterDict = !string.IsNullOrWhiteSpace(afterJson)
                    ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(afterJson)
                    : new Dictionary<string, JsonElement>();

                if (beforeDict == null && afterDict == null)
                    return changes;

                var allKeys = new HashSet<string>();
                if (beforeDict != null) allKeys.UnionWith(beforeDict.Keys);
                if (afterDict != null) allKeys.UnionWith(afterDict.Keys);

                foreach (var key in allKeys.OrderBy(k => k))
                {
                    var oldValue = beforeDict?.ContainsKey(key) == true
                        ? beforeDict[key].ToString()
                        : null;

                    var newValue = afterDict?.ContainsKey(key) == true
                        ? afterDict[key].ToString()
                        : null;

                    changes.Add(new FieldChangeDto
                    {
                        FieldName = key,
                        OldValue = oldValue,
                        NewValue = newValue
                    });
                }
            }
            catch
            {
                // JSON parse hatası - boş dön
            }

            return changes;
        }

        #endregion
    }
}

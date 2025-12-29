using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.Common.Interfaces;
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

            // Map to DTO and enrich with TargetPerson info
            var logs = new List<AuditLogDto>();
            foreach (var l in databaseLogs)
            {
                var dto = new AuditLogDto
                {
                    DatabaseLogId = l.DatabaseLogId,
                    TcKimlikNo = l.TcKimlikNo,
                    TabloAdi = l.TableName,
                    IslemTuru = l.DatabaseAction,
                    IslemZamani = l.IslemZamani,
                    StorageType = l.StorageType,
                    FileReference = l.FileReference,
                    TransactionId = l.TransactionId,
                    IsGroupedLog = l.IsGroupedLog,
                    ChangedFieldCount = l.ChangedFieldCount,
                    DataSizeBytes = l.DataSizeBytes,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    BulkOperationCount = l.BulkOperationCount
                };

                // İşlem yapılan kişi bilgisini extract et
                string? beforeData = null;
                string? afterData = null;

                if (l.StorageType == LogStorageType.Database)
                {
                    beforeData = l.BeforeData;
                    afterData = l.AfterData;
                }
                else if (l.StorageType == LogStorageType.File && !string.IsNullOrEmpty(l.FileReference))
                {
                    var fileEntry = await _fileWriter.ReadAsync(l.FileReference);
                    if (fileEntry != null)
                    {
                        beforeData = fileEntry.Before != null ? JsonSerializer.Serialize(fileEntry.Before) : null;
                        afterData = fileEntry.After != null ? JsonSerializer.Serialize(fileEntry.After) : null;
                    }
                }

                dto.TargetPersonTcKimlikNo = ExtractTcKimlikNoFromJson(beforeData, afterData);
                if (!string.IsNullOrEmpty(dto.TargetPersonTcKimlikNo))
                {
                    dto.TargetPersonAdSoyad = await _auditLogQueryRepository.GetAdSoyadByTcKimlikNoAsync(dto.TargetPersonTcKimlikNo);
                }

                logs.Add(dto);
            }

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
                TabloAdi = log.TableName,
                IslemTuru = log.DatabaseAction,
                IslemZamani = log.IslemZamani,
                StorageType = log.StorageType,
                FileReference = log.FileReference,
                TransactionId = log.TransactionId,
                IsGroupedLog = log.IsGroupedLog,
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

            // Field changes'i parse et ve FK lookuplarını yap
            detail.FieldChanges = await ParseFieldChangesWithLookupsAsync(detail.BeforeDataJson, detail.AfterDataJson);

            // İşlem yapılan kişi bilgisini çıkar (TcKimlikNo field'ı varsa)
            detail.TargetPersonTcKimlikNo = ExtractTcKimlikNoFromJson(detail.BeforeDataJson, detail.AfterDataJson);
            if (!string.IsNullOrEmpty(detail.TargetPersonTcKimlikNo))
            {
                detail.TargetPersonAdSoyad = await _auditLogQueryRepository.GetAdSoyadByTcKimlikNoAsync(detail.TargetPersonTcKimlikNo);
            }

            // Transaction içindeki diğer log'ları al
            if (log.TransactionId.HasValue)
            {
                var relatedLogs = await _auditLogQueryRepository.GetRelatedLogsInTransactionAsync(
                    log.TransactionId.Value, logId);
                
                detail.RelatedLogs = relatedLogs.Select(l => new AuditLogDto
                {
                    DatabaseLogId = l.DatabaseLogId,
                    TcKimlikNo = l.TcKimlikNo,
                    TabloAdi = l.TableName,
                    IslemTuru = l.DatabaseAction,
                    IslemZamani = l.IslemZamani,
                    StorageType = l.StorageType,
                    FileReference = l.FileReference,
                    TransactionId = l.TransactionId,
                    IsGroupedLog = l.IsGroupedLog,
                }).ToList();
            }

            return detail;
        }

        public async Task<List<AuditLogDto>> GetTransactionLogsAsync(Guid transactionId)
        {
            var logs = await _auditLogQueryRepository.GetTransactionLogsAsync(transactionId);
            
            return logs.Select(l => new AuditLogDto
            {
                DatabaseLogId = l.DatabaseLogId,
                TcKimlikNo = l.TcKimlikNo,
                TabloAdi = l.TableName,
                IslemTuru = l.DatabaseAction,
                IslemZamani = l.IslemZamani,
                StorageType = l.StorageType,
                FileReference = l.FileReference,
                TransactionId = l.TransactionId,
                IsGroupedLog = l.IsGroupedLog,
                ChangedFieldCount = l.ChangedFieldCount
            }).ToList();
        }

        public async Task<List<AuditLogDto>> GetUserRecentLogsAsync(string tcKimlikNo, int count = 50)
        {
            var logs = await _auditLogQueryRepository.GetUserRecentLogsAsync(tcKimlikNo, count);
            
            return logs.Select(l => new AuditLogDto
            {
                DatabaseLogId = l.DatabaseLogId,
                TcKimlikNo = l.TcKimlikNo,
                TabloAdi = l.TableName,
                IslemTuru = l.DatabaseAction,
                IslemZamani = l.IslemZamani,
                StorageType = l.StorageType,
                FileReference = l.FileReference,
                TransactionId = l.TransactionId,
                IsGroupedLog = l.IsGroupedLog,
            }).ToList();
        }

        public async Task<List<AuditLogDto>> GetEntityHistoryAsync(string tableName, string entityId)
        {
            var logs = await _auditLogQueryRepository.GetEntityHistoryAsync(tableName, entityId);
            
            return logs.Select(l => new AuditLogDto
            {
                DatabaseLogId = l.DatabaseLogId,
                TcKimlikNo = l.TcKimlikNo,
                TabloAdi = l.TableName,
                IslemTuru = l.DatabaseAction,
                IslemZamani = l.IslemZamani,
                StorageType = l.StorageType,
                FileReference = l.FileReference,
                TransactionId = l.TransactionId,
                IsGroupedLog = l.IsGroupedLog,
            }).ToList();
        }

        #region Helper Methods

        /// <summary>
        /// Before/After JSON'larını parse ederek SADECE değişen alanları döndürür (FK lookuplarıyla birlikte)
        /// </summary>
        private async Task<List<FieldChangeDto>> ParseFieldChangesWithLookupsAsync(string? beforeJson, string? afterJson)
        {
            var changes = ParseFieldChanges(beforeJson, afterJson);

            // Her field için FK lookup yap
            foreach (var change in changes)
            {
                change.OldValueDisplay = await ResolveFieldValueAsync(change.FieldName, change.OldValue);
                change.NewValueDisplay = await ResolveFieldValueAsync(change.FieldName, change.NewValue);
            }

            return changes;
        }

        /// <summary>
        /// Before/After JSON'larını parse ederek SADECE değişen alanları döndürür
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

                    // Her iki değer de boşsa skip et (gereksiz kayıt)
                    var oldValueEmpty = string.IsNullOrWhiteSpace(oldValue);
                    var newValueEmpty = string.IsNullOrWhiteSpace(newValue);
                    if (oldValueEmpty && newValueEmpty)
                        continue;

                    // Sadece değişen alanları ekle
                    if (oldValue != newValue)
                    {
                        changes.Add(new FieldChangeDto
                        {
                            FieldName = key,
                            OldValue = oldValue,
                            NewValue = newValue
                        });
                    }
                }
            }
            catch
            {
                // JSON parse hatası - boş dön
            }

            return changes;
        }

        /// <summary>
        /// Field value için kullanıcı dostu gösterim oluşturur (FK lookup yapar)
        /// Repository'ye delege eder - katman ayrımı
        /// </summary>
        private async Task<string?> ResolveFieldValueAsync(string fieldName, string? value)
        {
            return await _auditLogQueryRepository.ResolveFieldValueAsync(fieldName, value);
        }

        /// <summary>
        /// Before/After JSON'larından TcKimlikNo field'ını extract eder
        /// (Entity'de TcKimlikNo field'ı varsa, işlem yapılan kişiyi belirtir)
        /// </summary>
        private string? ExtractTcKimlikNoFromJson(string? beforeJson, string? afterJson)
        {
            try
            {
                // Önce After'dan dene (INSERT/UPDATE için)
                if (!string.IsNullOrWhiteSpace(afterJson))
                {
                    var afterDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(afterJson);
                    if (afterDict != null && afterDict.ContainsKey("TcKimlikNo"))
                    {
                        var tcValue = afterDict["TcKimlikNo"].ToString();
                        if (!string.IsNullOrWhiteSpace(tcValue))
                            return tcValue;
                    }
                }

                // After'da yoksa Before'dan dene (DELETE için)
                if (!string.IsNullOrWhiteSpace(beforeJson))
                {
                    var beforeDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(beforeJson);
                    if (beforeDict != null && beforeDict.ContainsKey("TcKimlikNo"))
                    {
                        var tcValue = beforeDict["TcKimlikNo"].ToString();
                        if (!string.IsNullOrWhiteSpace(tcValue))
                            return tcValue;
                    }
                }
            }
            catch
            {
                // Parse hatası - null dön
            }

            return null;
        }

        #endregion
    }
}

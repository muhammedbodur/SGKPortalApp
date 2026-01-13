using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    /// <summary>
    /// Audit log görüntüleme ve sorgulama servisi
    /// Database ve file-based log'ları birleştirerek sunar
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Audit log'ları filtreler ve sayfalama ile döner
        /// </summary>
        Task<AuditLogPagedResultDto> GetLogsAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Belirli bir log'un detayını döner (before/after data ile)
        /// </summary>
        Task<AuditLogDetailDto?> GetLogDetailAsync(int logId);

        /// <summary>
        /// Transaction ID'ye göre gruplu log'ları döner
        /// </summary>
        Task<List<AuditLogDto>> GetTransactionLogsAsync(Guid transactionId);

        /// <summary>
        /// Kullanıcının son N işlemini döner
        /// </summary>
        Task<List<AuditLogDto>> GetUserRecentLogsAsync(string tcKimlikNo, int count = 50);

        /// <summary>
        /// Belirli bir entity'nin değişim geçmişini döner
        /// </summary>
        Task<List<AuditLogDto>> GetEntityHistoryAsync(string tableName, string entityId);
    }
}

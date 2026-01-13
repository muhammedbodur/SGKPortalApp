using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    /// <summary>
    /// Audit log API servisi interface
    /// </summary>
    public interface IAuditLogApiService
    {
        /// <summary>
        /// Audit log'ları filtreler ve sayfalanmış döner
        /// </summary>
        Task<AuditLogPagedResultDto?> GetLogsAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Belirli bir log'un detayını döner
        /// </summary>
        Task<AuditLogDetailDto?> GetLogDetailAsync(int logId);

        /// <summary>
        /// Transaction ID'ye göre gruplu log'ları döner
        /// </summary>
        Task<List<AuditLogDto>?> GetTransactionLogsAsync(Guid transactionId);

        /// <summary>
        /// Kullanıcının son N işlemini döner
        /// </summary>
        Task<List<AuditLogDto>?> GetUserRecentLogsAsync(string tcKimlikNo, int count = 50);

        /// <summary>
        /// Belirli bir entity'nin değişim geçmişini döner
        /// </summary>
        Task<List<AuditLogDto>?> GetEntityHistoryAsync(string tableName, string entityId);
    }
}

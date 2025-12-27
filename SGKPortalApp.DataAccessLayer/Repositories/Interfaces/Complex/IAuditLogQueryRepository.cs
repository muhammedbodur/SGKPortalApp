using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex
{
    /// <summary>
    /// Audit log modülü için kompleks sorguları barındıran repository
    /// Yetki bazlı filtreleme ve kullanıcı bilgisi join'leri içerir
    /// </summary>
    public interface IAuditLogQueryRepository
    {
        /// <summary>
        /// Yetki bazlı filtreleme ile audit log'ları getirir
        /// Kullanıcının departman/servis yetkilerine göre otomatik filtreleme yapar
        /// </summary>
        /// <param name="filter">Filtreleme parametreleri</param>
        /// <param name="userDepartmanId">Kullanıcının departman ID'si (null ise kısıtlama yok)</param>
        /// <param name="userServisId">Kullanıcının servis ID'si (null ise kısıtlama yok)</param>
        /// <returns>Filtrelenmiş log listesi ve toplam kayıt sayısı</returns>
        Task<(List<DatabaseLog> Logs, int TotalCount)> GetLogsWithPermissionFilterAsync(
            AuditLogFilterDto filter,
            int? userDepartmanId,
            int? userServisId);

        /// <summary>
        /// Log detayını getirir ve yetki kontrolü yapar
        /// Kullanıcının bu log'u görüntüleme yetkisi yoksa null döner
        /// </summary>
        /// <param name="logId">Log ID</param>
        /// <param name="userDepartmanId">Kullanıcının departman ID'si (null ise kısıtlama yok)</param>
        /// <param name="userServisId">Kullanıcının servis ID'si (null ise kısıtlama yok)</param>
        /// <returns>Log detayı veya null (yetki yoksa)</returns>
        Task<DatabaseLog?> GetLogWithPermissionCheckAsync(
            int logId,
            int? userDepartmanId,
            int? userServisId);
    }
}

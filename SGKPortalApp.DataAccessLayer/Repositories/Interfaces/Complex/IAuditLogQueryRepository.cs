using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;
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

        /// <summary>
        /// Transaction'a ait tüm log'ları getirir
        /// </summary>
        Task<List<DatabaseLog>> GetTransactionLogsAsync(System.Guid transactionId);

        /// <summary>
        /// Kullanıcının son N log kaydını getirir
        /// </summary>
        Task<List<DatabaseLog>> GetUserRecentLogsAsync(string tcKimlikNo, int count);

        /// <summary>
        /// Belirli bir entity'nin geçmişini getirir
        /// </summary>
        Task<List<DatabaseLog>> GetEntityHistoryAsync(string tableName, string entityId);

        /// <summary>
        /// Transaction'daki diğer log'ları getirir (belirli bir log hariç)
        /// </summary>
        Task<List<DatabaseLog>> GetRelatedLogsInTransactionAsync(System.Guid transactionId, int excludeLogId);

        /// <summary>
        /// Field value için FK lookup yaparak kullanıcı dostu gösterim döndürür
        /// </summary>
        /// <param name="fieldName">Alan adı (örn: DepartmanId, ServisId)</param>
        /// <param name="value">Alan değeri (ID)</param>
        /// <returns>Kullanıcı dostu gösterim (örn: "Bilgi İşlem", "Banko #5 (Merkez)")</returns>
        Task<string?> ResolveFieldValueAsync(string fieldName, string? value);

        /// <summary>
        /// TC Kimlik No'dan Ad Soyad'ı getirir
        /// </summary>
        /// <param name="tcKimlikNo">TC Kimlik No</param>
        /// <returns>Ad Soyad veya null</returns>
        Task<string?> GetAdSoyadByTcKimlikNoAsync(string? tcKimlikNo);
    }
}

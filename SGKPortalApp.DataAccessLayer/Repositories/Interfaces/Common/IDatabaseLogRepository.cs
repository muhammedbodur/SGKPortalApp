using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IDatabaseLogRepository : IGenericRepository<DatabaseLog>
    {
        // Tarih bazında loglar
        Task<IEnumerable<DatabaseLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DatabaseLog>> GetTodayLogsAsync();

        // Kullanıcı bazında loglar
        Task<IEnumerable<DatabaseLog>> GetByUserAsync(string tcKimlikNo);

        // Action türü bazında loglar
        Task<IEnumerable<DatabaseLog>> GetByActionAsync(string databaseAction);

        // Tablo bazında loglar
        Task<IEnumerable<DatabaseLog>> GetByTableNameAsync(string tableName);

        // Son loglar (sayfalama)
        Task<IEnumerable<DatabaseLog>> GetRecentLogsAsync(int count = 100);

        // Log temizleme (eski logları sil)
        Task<int> DeleteOldLogsAsync(int daysToKeep = 30);
    }
}
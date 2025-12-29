using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface ILoginLogoutLogRepository : IGenericRepository<LoginLogoutLog>
    {
        // Kullanıcı bazında loglar
        Task<IEnumerable<LoginLogoutLog>> GetByUserAsync(string tcKimlikNo);

        // Tarih bazında loglar
        Task<IEnumerable<LoginLogoutLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoginLogoutLog>> GetTodayLogsAsync();

        // Login/Logout türü bazında
        Task<IEnumerable<LoginLogoutLog>> GetLoginLogsAsync();
        Task<IEnumerable<LoginLogoutLog>> GetLogoutLogsAsync();

        // Aktif oturumlar (Login var ama Logout yok)
        Task<IEnumerable<LoginLogoutLog>> GetActiveSessionsAsync();

        // Kullanıcının son login'i
        Task<LoginLogoutLog?> GetLastLoginAsync(string tcKimlikNo);

        // Son girişler (sayfalama)
        Task<IEnumerable<LoginLogoutLog>> GetRecentLogsAsync(int count = 50);

        // İstatistik
        Task<int> GetDailyLoginCountAsync(DateTime date);
        Task<int> GetActiveUserCountAsync();

        // Filtreleme ve sayfalama
        Task<(IEnumerable<LoginLogoutLog> Logs, int TotalCount)> GetFilteredLogsAsync(
            string? searchText,
            DateTime? startDate,
            DateTime? endDate,
            bool? onlyActiveSession,
            bool? onlyFailedLogins,
            string? ipAddress,
            int pageNumber,
            int pageSize);
    }
}
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class LoginLogoutLogRepository : GenericRepository<LoginLogoutLog>, ILoginLogoutLogRepository
    {
        public LoginLogoutLogRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetByUserAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<LoginLogoutLog>();

            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.TcKimlikNo == tcKimlikNo)
                .OrderByDescending(lll => lll.LoginTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.LoginTime >= startDate && lll.LoginTime <= endDate)
                .OrderByDescending(lll => lll.LoginTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetTodayLogsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.LoginTime >= today && lll.LoginTime < tomorrow)
                .OrderByDescending(lll => lll.LoginTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetLoginLogsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.LoginTime != default)
                .OrderByDescending(lll => lll.LoginTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetLogoutLogsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.LogoutTime.HasValue)
                .OrderByDescending(lll => lll.LogoutTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetActiveSessionsAsync()
        {
            // Login yapmış ama logout yapmamış kullanıcılar
            return await _dbSet
                .AsNoTracking()
                .Where(lll => !lll.LogoutTime.HasValue)
                .OrderByDescending(lll => lll.LoginTime)
                .ToListAsync();
        }

        public async Task<LoginLogoutLog?> GetLastLoginAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _dbSet
                .AsNoTracking()
                .Where(lll => lll.TcKimlikNo == tcKimlikNo)
                .OrderByDescending(lll => lll.LoginTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(lll => lll.LoginTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetDailyLoginCountAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .CountAsync(lll => lll.LoginTime >= startOfDay && lll.LoginTime < endOfDay);
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            var activeSessions = await GetActiveSessionsAsync();
            return activeSessions.Select(lll => lll.TcKimlikNo).Distinct().Count();
        }
    }
}
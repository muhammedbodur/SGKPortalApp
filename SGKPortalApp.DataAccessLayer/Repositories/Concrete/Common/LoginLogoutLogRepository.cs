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

        public async Task<(IEnumerable<LoginLogoutLog> Logs, int TotalCount)> GetFilteredLogsAsync(
            string? searchText,
            DateTime? startDate,
            DateTime? endDate,
            bool? onlyActiveSession,
            bool? onlyFailedLogins,
            string? ipAddress,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            // Tarih filtreleme
            if (startDate.HasValue)
                query = query.Where(l => l.LoginTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.LoginTime <= endDate.Value);

            // Arama (Ad Soyad, TC Kimlik No)
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim().ToLower();
                query = query.Where(l =>
                    (l.AdSoyad != null && l.AdSoyad.ToLower().Contains(search)) ||
                    (l.TcKimlikNo != null && l.TcKimlikNo.Contains(search)));
            }

            // Aktif oturum filtresi
            if (onlyActiveSession.HasValue && onlyActiveSession.Value)
                query = query.Where(l => !l.LogoutTime.HasValue);

            // Başarısız login filtresi
            if (onlyFailedLogins.HasValue && onlyFailedLogins.Value)
                query = query.Where(l => !l.LoginSuccessful);

            // IP adresi filtresi
            if (!string.IsNullOrWhiteSpace(ipAddress))
                query = query.Where(l => l.IpAddress != null && l.IpAddress.Contains(ipAddress.Trim()));

            // Toplam sayı
            var totalCount = await query.CountAsync();

            // Sayfalama ve sıralama
            var logs = await query
                .OrderByDescending(l => l.LoginTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (logs, totalCount);
        }

        public async Task<LoginLogoutLog?> GetBySessionIdAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return null;

            // SessionID'ye göre son login kaydını getir (LogoutTime olsun olmasın)
            // User ilişkisini de dahil ederek her iki tablodaki SessionID'lerin eşleşmesini kontrol et
            return await _dbSet
                .Include(l => l.User)
                .Where(l => l.SessionID == sessionId
                    && l.User != null
                    && l.User.SessionID == sessionId)
                .OrderByDescending(l => l.LoginTime)
                .FirstOrDefaultAsync();
        }

        public async Task<LoginLogoutLog?> GetActiveSessionBySessionIdAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return null;

            return await _dbSet
                .Where(l => l.SessionID == sessionId && !l.LogoutTime.HasValue)
                .OrderByDescending(l => l.LoginTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateLogoutTimeBySessionIdAsync(string sessionId, DateTime logoutTime)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return false;

            var loginLog = await _dbSet
                .Where(l => l.SessionID == sessionId && !l.LogoutTime.HasValue)
                .OrderByDescending(l => l.LoginTime)
                .FirstOrDefaultAsync();

            if (loginLog != null)
            {
                loginLog.LogoutTime = logoutTime;
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetOrphanSessionsAsync(DateTime timeoutThreshold)
        {
            return await _dbSet
                .Where(l => !l.LogoutTime.HasValue && l.LoginTime < timeoutThreshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<LoginLogoutLog>> GetDisconnectedButNotLoggedOutSessionsAsync(
            DateTime timeoutThreshold,
            IEnumerable<string> activeSessionIds)
        {
            var activeSessionIdList = activeSessionIds.ToList();

            return await _dbSet
                .Where(l => !l.LogoutTime.HasValue
                    && l.SessionID != null
                    && !activeSessionIdList.Contains(l.SessionID)
                    && l.LoginTime < timeoutThreshold)
                .ToListAsync();
        }

        public async Task<int> UpdateOrphanSessionsLogoutTimeAsync(DateTime timeoutThreshold, DateTime logoutTime)
        {
            var orphanSessions = await _dbSet
                .Where(l => !l.LogoutTime.HasValue && l.LoginTime < timeoutThreshold)
                .ToListAsync();

            foreach (var session in orphanSessions)
            {
                session.LogoutTime = logoutTime;
            }

            return orphanSessions.Count;
        }
    }
}
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
    public class DatabaseLogRepository : GenericRepository<DatabaseLog>, IDatabaseLogRepository
    {
        public DatabaseLogRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DatabaseLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(dl => dl.EklenmeTarihi >= startDate && dl.EklenmeTarihi <= endDate)
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseLog>> GetTodayLogsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Where(dl => dl.EklenmeTarihi >= today && dl.EklenmeTarihi < tomorrow)
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseLog>> GetByUserAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<DatabaseLog>();

            return await _dbSet
                .AsNoTracking()
                .Where(dl => dl.TcKimlikNo == tcKimlikNo)
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseLog>> GetByActionAsync(string databaseAction)
        {
            if (string.IsNullOrWhiteSpace(databaseAction))
                return Enumerable.Empty<DatabaseLog>();

            return await _dbSet
                .AsNoTracking()
                .Where(dl => dl.DatabaseAction.ToString() == databaseAction)
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseLog>> GetByTableNameAsync(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return Enumerable.Empty<DatabaseLog>();

            return await _dbSet
                .AsNoTracking()
                .Where(dl => dl.TableName == tableName)
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatabaseLog>> GetRecentLogsAsync(int count = 100)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(dl => dl.EklenmeTarihi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> DeleteOldLogsAsync(int daysToKeep = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            var oldLogs = await _dbSet
                .Where(dl => dl.EklenmeTarihi < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _dbSet.RemoveRange(oldLogs);
                return oldLogs.Count;
            }

            return 0;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class ResmiTatilRepository : GenericRepository<ResmiTatil>, IResmiTatilRepository
    {
        public ResmiTatilRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ResmiTatil>> GetByYearAsync(int year)
        {
            return await _dbSet
                .Where(t => t.Yil == year)
                .OrderBy(t => t.Tarih)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            return await _dbSet
                .AnyAsync(t => t.Tarih.Date == date.Date);
        }

        public async Task<string?> GetHolidayNameAsync(DateTime date)
        {
            var tatil = await _dbSet
                .Where(t => t.Tarih.Date == date.Date)
                .Select(t => t.TatilAdi)
                .FirstOrDefaultAsync();

            return tatil;
        }

        public async Task<IEnumerable<ResmiTatil>> GetActiveAsync()
        {
            return await _dbSet
                .Where(t => !t.SilindiMi)
                .OrderBy(t => t.Tarih)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsByDateAsync(DateTime date, int? excludeId = null)
        {
            var query = _dbSet.Where(t => t.Tarih.Date == date.Date);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.TatilId != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}

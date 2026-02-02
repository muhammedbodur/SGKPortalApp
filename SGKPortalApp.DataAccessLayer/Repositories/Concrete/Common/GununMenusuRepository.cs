using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Günün Menüsü Repository Implementation
    /// </summary>
    public class GununMenusuRepository : GenericRepository<GununMenusu>, IGununMenusuRepository
    {
        public GununMenusuRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<GununMenusu?> GetTodaysMenuAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .AsNoTracking()
                .Where(m => m.Tarih.Date == today && m.Aktiflik == Aktiflik.Aktif && !m.SilindiMi)
                .FirstOrDefaultAsync();
        }

        public async Task<GununMenusu?> GetMenuByDateAsync(DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(m => m.Tarih.Date == date.Date && m.Aktiflik == Aktiflik.Aktif && !m.SilindiMi)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<GununMenusu>> GetMenusByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(m => m.Tarih.Date >= startDate.Date &&
                           m.Tarih.Date <= endDate.Date &&
                           m.Aktiflik == Aktiflik.Aktif &&
                           !m.SilindiMi)
                .OrderBy(m => m.Tarih)
                .ToListAsync();
        }
    }
}

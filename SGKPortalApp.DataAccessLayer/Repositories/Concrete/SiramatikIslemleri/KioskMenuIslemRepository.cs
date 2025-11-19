using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KioskMenuIslemRepository : GenericRepository<KioskMenuIslem>, IKioskMenuIslemRepository
    {
        public KioskMenuIslemRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<KioskMenuIslem>> GetByKioskMenuAsync(int kioskMenuId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kmi => kmi.KanalAlt)
                    .ThenInclude(ka => ka.Kanal)
                .Where(kmi => kmi.KioskMenuId == kioskMenuId)
                .OrderBy(kmi => kmi.MenuSira)
                .ToListAsync();
        }

        public async Task<KioskMenuIslem?> GetWithDetailsAsync(int kioskMenuIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kmi => kmi.KioskMenu)
                .Include(kmi => kmi.KanalAlt)
                    .ThenInclude(ka => ka.Kanal)
                .FirstOrDefaultAsync(kmi => kmi.KioskMenuIslemId == kioskMenuIslemId);
        }

        public async Task<bool> ExistsByMenuAndSiraAsync(int kioskMenuId, int menuSira, int? excludeId = null)
        {
            var query = _dbSet.AsNoTracking()
                .Where(kmi => kmi.KioskMenuId == kioskMenuId && kmi.MenuSira == menuSira);

            if (excludeId.HasValue)
            {
                query = query.Where(kmi => kmi.KioskMenuIslemId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetMaxSiraByMenuAsync(int kioskMenuId)
        {
            var maxSira = await _dbSet
                .AsNoTracking()
                .Where(kmi => kmi.KioskMenuId == kioskMenuId)
                .MaxAsync(kmi => (int?)kmi.MenuSira);

            return maxSira ?? 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KioskMenuRepository : GenericRepository<KioskMenu>, IKioskMenuRepository
    {
        public KioskMenuRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<KioskMenu>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(menu => menu.Aktiflik == Aktiflik.Aktif)
                .OrderBy(menu => menu.MenuAdi)
                .ToListAsync();
        }

        public async Task<KioskMenu?> GetWithKiosksAsync(int kioskMenuId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(menu => menu.KioskMenuId == kioskMenuId);
        }

        public async Task<bool> ExistsByNameAsync(string menuAdi)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(menu => menu.MenuAdi == menuAdi);
        }

        public async Task<int> GetMaxSiraAsync()
        {
            var maxSira = await _dbSet
                .AsNoTracking()
                .MaxAsync(menu => (int?)menu.MenuSira);
            
            return maxSira ?? 0;
        }
    }
}

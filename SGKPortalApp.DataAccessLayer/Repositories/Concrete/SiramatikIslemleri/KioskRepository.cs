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
    public class KioskRepository : GenericRepository<Kiosk>, IKioskRepository
    {
        public KioskRepository(SGKDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Tüm kiosk'ları navigation property'leri ile birlikte getirir
        /// </summary>
        public override async Task<IEnumerable<Kiosk>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kiosk => kiosk.HizmetBinasi)
                    .ThenInclude(hb => hb.Departman)
                .Include(kiosk => kiosk.MenuAtamalari.Where(ma => ma.Aktiflik == Aktiflik.Aktif))
                    .ThenInclude(ma => ma.KioskMenu)
                .OrderBy(kiosk => kiosk.KioskAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Kiosk>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kiosk => kiosk.HizmetBinasi)
                    .ThenInclude(hb => hb.Departman)
                .Include(kiosk => kiosk.MenuAtamalari.Where(ma => ma.Aktiflik == Aktiflik.Aktif))
                    .ThenInclude(ma => ma.KioskMenu)
                .Where(kiosk => kiosk.HizmetBinasiId == hizmetBinasiId)
                .OrderBy(kiosk => kiosk.KioskAdi)
                .ToListAsync();
        }

        public async Task<Kiosk?> GetWithMenuAsync(int kioskId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kiosk => kiosk.HizmetBinasi)
                    .ThenInclude(hb => hb.Departman)
                .Include(kiosk => kiosk.MenuAtamalari)
                    .ThenInclude(ma => ma.KioskMenu)
                .FirstOrDefaultAsync(kiosk => kiosk.KioskId == kioskId);
        }

        public async Task<IEnumerable<Kiosk>> GetWithMenuAsync(IEnumerable<int> kioskIds)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kiosk => kiosk.HizmetBinasi)
                    .ThenInclude(hb => hb.Departman)
                .Include(kiosk => kiosk.MenuAtamalari)
                    .ThenInclude(ma => ma.KioskMenu)
                .Where(kiosk => kioskIds.Contains(kiosk.KioskId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Kiosk>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(kiosk => kiosk.Aktiflik == Aktiflik.Aktif)
                .OrderBy(kiosk => kiosk.KioskAdi)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string kioskAdi, int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(kiosk => kiosk.HizmetBinasiId == hizmetBinasiId && kiosk.KioskAdi == kioskAdi);
        }
    }
}

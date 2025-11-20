using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KioskMenuAtamaRepository : GenericRepository<KioskMenuAtama>, IKioskMenuAtamaRepository
    {
        public KioskMenuAtamaRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<KioskMenuAtama>> GetByKioskAsync(int kioskId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kma => kma.Kiosk)
                    .ThenInclude(k => k.HizmetBinasi)
                .Include(kma => kma.KioskMenu)
                .Where(kma => kma.KioskId == kioskId)
                .OrderByDescending(kma => kma.AtamaTarihi)
                .ToListAsync();
        }

        public async Task<KioskMenuAtama?> GetActiveByKioskAsync(int kioskId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kma => kma.KioskMenu)
                .Where(kma => kma.KioskId == kioskId && kma.Aktiflik == Aktiflik.Aktif)
                .FirstOrDefaultAsync();
        }

        public async Task<KioskMenuAtama?> GetWithDetailsAsync(int kioskMenuAtamaId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(kma => kma.Kiosk)
                    .ThenInclude(k => k.HizmetBinasi)
                        .ThenInclude(hb => hb.Departman)
                .Include(kma => kma.KioskMenu)
                .FirstOrDefaultAsync(kma => kma.KioskMenuAtamaId == kioskMenuAtamaId);
        }

        public async Task<KioskMenuAtama?> GetByKioskAndMenuAsync(int kioskId, int kioskMenuId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(kma => kma.KioskId == kioskId 
                    && kma.KioskMenuId == kioskMenuId 
                    && kma.SilindiMi == false);
        }

        public async Task<bool> HasActiveAtamaAsync(int kioskId)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(kma => kma.KioskId == kioskId && kma.Aktiflik == Aktiflik.Aktif);
        }

        public async Task DeactivateOtherAtamasAsync(int kioskId, int currentAtamaId)
        {
            var otherAtamas = await _dbSet
                .Where(kma => kma.KioskId == kioskId 
                    && kma.KioskMenuAtamaId != currentAtamaId 
                    && kma.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();

            foreach (var atama in otherAtamas)
            {
                atama.Aktiflik = Aktiflik.Pasif;
                atama.DuzenlenmeTarihi = DateTime.Now;
            }

            if (otherAtamas.Any())
            {
                _dbSet.UpdateRange(otherAtamas);
            }
        }
    }
}

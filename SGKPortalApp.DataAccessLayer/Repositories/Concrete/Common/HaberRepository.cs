using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Haber Repository Implementation
    /// </summary>
    public class HaberRepository : GenericRepository<Haber>, IHaberRepository
    {
        public HaberRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Haber>> GetActiveHaberlerAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(h => h.Aktiflik == Aktiflik.Aktif &&
                           !h.SilindiMi &&
                           h.YayinTarihi <= now &&
                           (h.BitisTarihi == null || h.BitisTarihi >= now))
                .OrderBy(h => h.Sira)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetSliderHaberlerAsync(int count = 5)
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(h => h.Aktiflik == Aktiflik.Aktif &&
                           !h.SilindiMi &&
                           !string.IsNullOrEmpty(h.GorselUrl) &&
                           h.YayinTarihi <= now &&
                           (h.BitisTarihi == null || h.BitisTarihi >= now))
                .OrderBy(h => h.Sira)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetListeHaberlerAsync(int count = 10)
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(h => h.Aktiflik == Aktiflik.Aktif &&
                           !h.SilindiMi &&
                           h.YayinTarihi <= now &&
                           (h.BitisTarihi == null || h.BitisTarihi >= now))
                .OrderByDescending(h => h.YayinTarihi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetYayindakiHaberlerAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(h => h.Aktiflik == Aktiflik.Aktif &&
                           !h.SilindiMi &&
                           h.YayinTarihi <= now &&
                           (h.BitisTarihi == null || h.BitisTarihi >= now))
                .OrderBy(h => h.Sira)
                .ThenByDescending(h => h.YayinTarihi)
                .ToListAsync();
        }

        public async Task<Haber?> GetByIdWithGorsellerAsync(int id)
        {
            return await _dbSet
                .Include(h => h.Gorseller.Where(g => !g.SilindiMi).OrderBy(g => g.Sira))
                .FirstOrDefaultAsync(h => h.HaberId == id && !h.SilindiMi);
        }
    }
}

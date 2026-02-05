using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Dashboard Haber Repository Implementation
    /// </summary>
    public class HaberDashboardRepository : GenericRepository<Haber>, IHaberDashboardRepository
    {
        public HaberDashboardRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Haber>> GetActiveHaberleriAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           !d.SilindiMi &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now))
                .OrderBy(d => d.Sira)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetSliderHaberleriAsync(int count = 5)
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           !d.SilindiMi &&
                           !string.IsNullOrEmpty(d.GorselUrl) &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now))
                .OrderBy(d => d.Sira)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetListeHaberleriAsync(int count = 10)
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           !d.SilindiMi &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now))
                .OrderByDescending(d => d.YayinTarihi)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Haber>> GetYayindakiHaberleriAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.Aktiflik == Aktiflik.Aktif &&
                           !d.SilindiMi &&
                           d.YayinTarihi <= now &&
                           (d.BitisTarihi == null || d.BitisTarihi >= now))
                .OrderBy(d => d.Sira)
                .ThenByDescending(d => d.YayinTarihi)
                .ToListAsync();
        }
    }
}

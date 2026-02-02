using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    /// <summary>
    /// Duyuru Repository Implementation
    /// </summary>
    public class DuyuruRepository : GenericRepository<Duyuru>, IDuyuruRepository
    {
        public DuyuruRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Duyuru>> GetActiveDuyurularAsync()
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

        public async Task<IEnumerable<Duyuru>> GetSliderDuyurularAsync(int count = 5)
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

        public async Task<IEnumerable<Duyuru>> GetListeDuyurularAsync(int count = 10)
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

        public async Task<IEnumerable<Duyuru>> GetYayindakiDuyurularAsync()
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

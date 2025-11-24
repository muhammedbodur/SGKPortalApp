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
    public class TvBankoRepository : GenericRepository<TvBanko>, ITvBankoRepository
    {
        public TvBankoRepository(SGKDbContext context) : base(context) { }

        // TV bazında banko eşleştirmelerini listeler
        public async Task<IEnumerable<TvBanko>> GetByTvAsync(int tvId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tb => tb.Tv)
                .Include(tb => tb.Banko)
                .Where(tb => tb.TvId == tvId)
                .ToListAsync();
        }

        // Banko bazında TV eşleştirmelerini listeler
        public async Task<IEnumerable<TvBanko>> GetByBankoAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tb => tb.Tv)
                .Include(tb => tb.Banko)
                .Where(tb => tb.BankoId == bankoId)
                .ToListAsync();
        }

        // Eşleştirmeyi detaylı getirir
        public async Task<TvBanko?> GetWithDetailsAsync(int tvBankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tb => tb.Tv)
                .Include(tb => tb.Banko)
                .FirstOrDefaultAsync(tb => tb.TvBankoId == tvBankoId);
        }

        // Tüm eşleştirmeleri detaylı listeler
        public async Task<IEnumerable<TvBanko>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tb => tb.Tv)
                .Include(tb => tb.Banko)
                .ToListAsync();
        }

        // Aktif eşleştirmeleri listeler
        public async Task<IEnumerable<TvBanko>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tb => tb.Tv)
                .Include(tb => tb.Banko)
                .Where(tb => tb.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Belirli TV ve Banko için eşleştirme getirir
        public async Task<TvBanko?> GetByTvAndBankoAsync(int tvId, int bankoId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(tb => tb.TvId == tvId && tb.BankoId == bankoId && tb.SilindiMi == false);
        }
    }
}
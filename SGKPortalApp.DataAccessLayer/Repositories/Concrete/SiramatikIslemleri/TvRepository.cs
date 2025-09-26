using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class TvRepository : GenericRepository<Tv>, ITvRepository
    {
        public TvRepository(SGKDbContext context) : base(context) { }

        // Hizmet binası bazında TV'leri listeler
        public async Task<IEnumerable<Tv>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tv => tv.HizmetBinasi)
                .Where(tv => tv.HizmetBinasiId == hizmetBinasiId)
                .ToListAsync();
        }

        // TV'yi detaylı getirir
        public async Task<Tv?> GetWithDetailsAsync(int tvId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tv => tv.HizmetBinasi)
                .Include(tv => tv.HubTvConnection)
                .Include(tv => tv.TvBankolar)
                .FirstOrDefaultAsync(tv => tv.TvId == tvId);
        }

        // Tüm TV'leri detaylı listeler
        public async Task<IEnumerable<Tv>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tv => tv.HizmetBinasi)
                .Include(tv => tv.HubTvConnection)
                .Include(tv => tv.TvBankolar)
                .ToListAsync();
        }

        // Banko eşleşmiş TV'leri listeler
        public async Task<IEnumerable<Tv>> GetWithBankoAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tv => tv.TvBankolar)
                .Where(tv => tv.TvBankolar != null && tv.TvBankolar.Any())
                .ToListAsync();
        }

        // Bankoya atanmış TV'yi getirir
        public async Task<Tv?> GetByBankoAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(tv => tv.TvBankolar)
                .FirstOrDefaultAsync(tv => tv.TvBankolar != null && tv.TvBankolar.Any(tb => tb.BankoId == bankoId));
        }

        // Kat tipine göre TV'leri listeler
        public async Task<IEnumerable<Tv>> GetByKatTipiAsync(KatTipi katTipi)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(tv => tv.KatTipi == katTipi)
                .ToListAsync();
        }

        // Aktif TV'leri listeler
        public async Task<IEnumerable<Tv>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(tv => tv.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm TV'leri listeler
        public async Task<IEnumerable<Tv>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Dropdown için TV'leri listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(tv => new ValueTuple<int, string>(tv.TvId, tv.Aciklama ?? tv.TvId.ToString()))
                .ToListAsync();
        }

        // Hizmet binası için dropdown TV'leri listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(tv => tv.HizmetBinasiId == hizmetBinasiId)
                .Select(tv => new ValueTuple<int, string>(tv.TvId, tv.Aciklama ?? tv.TvId.ToString()))
                .ToListAsync();
        }
    }
}
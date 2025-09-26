using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class HizmetBinasiRepository : GenericRepository<HizmetBinasi>, IHizmetBinasiRepository
    {
        public HizmetBinasiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<HizmetBinasi?> GetByHizmetBinasiAdiAsync(string hizmetBinasiAdi)
        {
            if (string.IsNullOrWhiteSpace(hizmetBinasiAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiAdi == hizmetBinasiAdi);
        }

        public async Task<IEnumerable<HizmetBinasi>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.DepartmanId == departmanId)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<HizmetBinasi?> GetWithDepartmanAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<IEnumerable<HizmetBinasi>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.HizmetBinasiAktiflik == SGKPortalApp.BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<HizmetBinasi>> GetAllAsync()
        {
            return await _dbSet
                .Include(hb => hb.Departman)
                .AsNoTracking()
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.HizmetBinasiAktiflik == SGKPortalApp.BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .Select(hb => new ValueTuple<int, string>(hb.HizmetBinasiId, hb.HizmetBinasiAdi))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetByDepartmanDropdownAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.DepartmanId == departmanId &&
                            hb.HizmetBinasiAktiflik == SGKPortalApp.BusinessObjectLayer.Enums.Common.Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .Select(hb => new ValueTuple<int, string>(hb.HizmetBinasiId, hb.HizmetBinasiAdi))
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Personel>()
                .AsNoTracking()
                .CountAsync(p => p.HizmetBinasiId == hizmetBinasiId && !p.SilindiMi);
        }
    }
}
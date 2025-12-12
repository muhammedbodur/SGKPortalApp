using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

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

        public async Task<HizmetBinasi?> GetWithPersonellerAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Personeller)
                    .ThenInclude(p => p.Departman)
                .Include(hb => hb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithBankolarAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Bankolar)
                .Include(hb => hb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithTvlerAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Tvler)
                .Include(hb => hb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithAllDetailsAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Departman)
                .Include(hb => hb.Personeller)
                    .ThenInclude(p => p.Departman)
                .Include(hb => hb.Bankolar)
                .Include(hb => hb.Tvler)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<IEnumerable<HizmetBinasi>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.Aktiflik == Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Personel>()
                .CountAsync(p => p.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<int> GetBankoCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Banko>()
                .CountAsync(b => b.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<int> GetTvCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Tv>()
                .CountAsync(t => t.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(hb => hb.Aktiflik == Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .Select(hb => new ValueTuple<int, string>(hb.HizmetBinasiId, hb.HizmetBinasiAdi))
                .ToListAsync();
        }
    }
}
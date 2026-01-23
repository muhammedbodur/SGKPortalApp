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

        /// <summary>
        /// Belirli bir departmana ait hizmet binalarını getirir (many-to-many)
        /// </summary>
        public async Task<IEnumerable<HizmetBinasi>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Departman)
                .Where(hb => hb.DepartmanHizmetBinalari.Any(dhb => dhb.DepartmanId == departmanId && !dhb.SilindiMi))
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        /// <summary>
        /// Hizmet binasını departmanlarıyla birlikte getirir (many-to-many)
        /// </summary>
        public async Task<HizmetBinasi?> GetWithDepartmanAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithPersonellerAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.Personeller)
                    .ThenInclude(p => p.Departman)
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithBankolarAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Bankolar.Where(b => !b.SilindiMi))
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithTvlerAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Tvler.Where(t => !t.SilindiMi))
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<HizmetBinasi?> GetWithAllDetailsAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Bankolar.Where(b => !b.SilindiMi))
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Tvler.Where(t => !t.SilindiMi))
                .Include(hb => hb.Personeller)
                    .ThenInclude(p => p.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.HizmetBinasiId == hizmetBinasiId);
        }

        public async Task<IEnumerable<HizmetBinasi>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(hb => hb.DepartmanHizmetBinalari.Where(dhb => !dhb.SilindiMi))
                    .ThenInclude(dhb => dhb.Departman)
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Bankolar.Where(b => !b.SilindiMi && b.Aktiflik == Aktiflik.Aktif))
                .Include(hb => hb.DepartmanHizmetBinalari)
                    .ThenInclude(dhb => dhb.Tvler.Where(t => !t.SilindiMi && t.Aktiflik == Aktiflik.Aktif))
                .Include(hb => hb.Personeller.Where(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif))
                .Where(hb => hb.Aktiflik == Aktiflik.Aktif)
                .OrderBy(hb => hb.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Personel>()
                .CountAsync(p => p.HizmetBinasiId == hizmetBinasiId && !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif);
        }

        public async Task<int> GetBankoCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Banko>()
                .CountAsync(b => b.DepartmanHizmetBinasi.HizmetBinasiId == hizmetBinasiId && !b.SilindiMi && b.Aktiflik == Aktiflik.Aktif);
        }

        public async Task<int> GetTvCountAsync(int hizmetBinasiId)
        {
            return await _context.Set<Tv>()
                .CountAsync(t => t.DepartmanHizmetBinasi.HizmetBinasiId == hizmetBinasiId && !t.SilindiMi && t.Aktiflik == Aktiflik.Aktif);
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

        /// <summary>
        /// LegacyKod'a göre hizmet binası getirir (MySQL sync için)
        /// </summary>
        public async Task<HizmetBinasi?> GetByLegacyKodAsync(int legacyKod)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(hb => hb.LegacyKod == legacyKod);
        }
    }
}
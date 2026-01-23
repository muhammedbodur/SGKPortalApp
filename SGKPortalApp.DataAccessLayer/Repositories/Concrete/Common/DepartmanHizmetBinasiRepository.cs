using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class DepartmanHizmetBinasiRepository : GenericRepository<DepartmanHizmetBinasi>, IDepartmanHizmetBinasiRepository
    {
        public DepartmanHizmetBinasiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DepartmanHizmetBinasi>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Include(dhb => dhb.HizmetBinasi)
                .Where(dhb => dhb.DepartmanId == departmanId && !dhb.SilindiMi)
                .OrderBy(dhb => dhb.HizmetBinasi.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<DepartmanHizmetBinasi>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Include(dhb => dhb.HizmetBinasi)
                .Where(dhb => dhb.HizmetBinasiId == hizmetBinasiId && !dhb.SilindiMi)
                .OrderBy(dhb => dhb.Departman.DepartmanAdi)
                .ToListAsync();
        }

        public async Task<DepartmanHizmetBinasi?> GetByDepartmanAndHizmetBinasiAsync(int departmanId, int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Include(dhb => dhb.HizmetBinasi)
                .FirstOrDefaultAsync(dhb => dhb.DepartmanId == departmanId 
                    && dhb.HizmetBinasiId == hizmetBinasiId 
                    && !dhb.SilindiMi);
        }

        public async Task<IEnumerable<DepartmanHizmetBinasi>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Include(dhb => dhb.HizmetBinasi)
                .Where(dhb => !dhb.SilindiMi)
                .OrderBy(dhb => dhb.Departman.DepartmanAdi)
                .ThenBy(dhb => dhb.HizmetBinasi.HizmetBinasiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Include(dhb => dhb.HizmetBinasi)
                .Where(dhb => !dhb.SilindiMi)
                .OrderBy(dhb => dhb.Departman.DepartmanAdi)
                .ThenBy(dhb => dhb.HizmetBinasi.HizmetBinasiAdi)
                .Select(dhb => new ValueTuple<int, string>(
                    dhb.DepartmanHizmetBinasiId,
                    dhb.Departman.DepartmanAdi + " - " + dhb.HizmetBinasi.HizmetBinasiAdi
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.HizmetBinasi)
                .Where(dhb => dhb.DepartmanId == departmanId && !dhb.SilindiMi)
                .OrderBy(dhb => dhb.HizmetBinasi.HizmetBinasiAdi)
                .Select(dhb => new ValueTuple<int, string>(
                    dhb.DepartmanHizmetBinasiId,
                    dhb.HizmetBinasi.HizmetBinasiAdi
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string DisplayText)>> GetDropdownByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dhb => dhb.Departman)
                .Where(dhb => dhb.HizmetBinasiId == hizmetBinasiId && !dhb.SilindiMi)
                .OrderBy(dhb => dhb.Departman.DepartmanAdi)
                .Select(dhb => new ValueTuple<int, string>(
                    dhb.DepartmanHizmetBinasiId,
                    dhb.Departman.DepartmanAdi
                ))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int departmanId, int hizmetBinasiId)
        {
            return await _dbSet
                .AnyAsync(dhb => dhb.DepartmanId == departmanId 
                    && dhb.HizmetBinasiId == hizmetBinasiId 
                    && !dhb.SilindiMi);
        }
    }
}

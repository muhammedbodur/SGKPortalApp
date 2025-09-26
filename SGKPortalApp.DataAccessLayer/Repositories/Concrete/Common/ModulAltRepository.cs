using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class ModulAltRepository : GenericRepository<ModulAlt>, IModulAltRepository
    {
        public ModulAltRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<ModulAlt?> GetByModulAltAdiAsync(string modulAltAdi)
        {
            if (string.IsNullOrWhiteSpace(modulAltAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(ma => ma.ModulAltAdi == modulAltAdi);
        }

        public async Task<IEnumerable<ModulAlt>> GetByModulAsync(int modulId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ma => ma.ModulId == modulId)
                .OrderBy(ma => ma.ModulAltAdi)
                .ToListAsync();
        }

        public async Task<ModulAlt?> GetWithModulAsync(int modulAltId)
        {
            return await _dbSet
                .Include(ma => ma.Modul)
                .AsNoTracking()
                .FirstOrDefaultAsync(ma => ma.ModulAltId == modulAltId);
        }

        public async Task<IEnumerable<ModulAlt>> GetAllWithModulAsync()
        {
            return await _dbSet
                .Include(ma => ma.Modul)
                .AsNoTracking()
                .OrderBy(ma => ma.Modul.ModulAdi)
                .ThenBy(ma => ma.ModulAltAdi)
                .ToListAsync();
        }

        // ModulAlt entity'sinde UstModulAltId field'ı olmadığı için basitleştirdim
        public async Task<IEnumerable<ModulAlt>> GetByParentModulAltAsync(int parentModulAltId)
        {
            // Bu metod şimdilik boş döner, entity'de parent field'ı yok
            return await Task.FromResult(Enumerable.Empty<ModulAlt>());
        }

        public async Task<IEnumerable<ModulAlt>> GetRootModulAltlarAsync()
        {
            // Parent field'ı olmadığı için tüm alt modülleri döner
            return await GetAllAsync();
        }

        public async Task<IEnumerable<ModulAlt>> GetActiveAsync()
        {
            // AktifMi field'ı olmadığı için tüm kayıtları döner
            return await _dbSet
                .AsNoTracking()
                .OrderBy(ma => ma.ModulAltAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulAlt>> GetAllAsync()
        {
            return await _dbSet
                .Include(ma => ma.Modul)
                .AsNoTracking()
                .OrderBy(ma => ma.Modul.ModulAdi)
                .ThenBy(ma => ma.ModulAltAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(ma => ma.ModulAltAdi)
                .Select(ma => new ValueTuple<int, string>(ma.ModulAltId, ma.ModulAltAdi))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetByModulDropdownAsync(int modulId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ma => ma.ModulId == modulId)
                .OrderBy(ma => ma.ModulAltAdi)
                .Select(ma => new ValueTuple<int, string>(ma.ModulAltId, ma.ModulAltAdi))
                .ToListAsync();
        }
    }
}
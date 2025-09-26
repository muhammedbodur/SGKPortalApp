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
    public class ModulRepository : GenericRepository<Modul>, IModulRepository
    {
        public ModulRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Modul?> GetByModulAdiAsync(string modulAdi)
        {
            if (string.IsNullOrWhiteSpace(modulAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ModulAdi == modulAdi);
        }

        public async Task<Modul?> GetWithControllersAsync(int modulId)
        {
            return await _dbSet
                .Include(m => m.ModulControllers)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ModulId == modulId);
        }

        public async Task<IEnumerable<Modul>> GetAllWithControllersAsync()
        {
            return await _dbSet
                .Include(m => m.ModulControllers)
                .AsNoTracking()
                .OrderBy(m => m.ModulAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Modul>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(m => m.ModulAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Modul>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(m => m.ModulAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(m => m.ModulAdi)
                .Select(m => new ValueTuple<int, string>(m.ModulId, m.ModulAdi))
                .ToListAsync();
        }

        public async Task<int> GetControllerCountAsync(int modulId)
        {
            return await _context.Set<ModulController>()
                .AsNoTracking()
                .CountAsync(mc => mc.ModulId == modulId);
        }
    }
}
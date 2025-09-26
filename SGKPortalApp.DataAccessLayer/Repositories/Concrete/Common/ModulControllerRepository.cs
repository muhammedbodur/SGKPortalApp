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
    public class ModulControllerRepository : GenericRepository<ModulController>, IModulControllerRepository
    {
        public ModulControllerRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<ModulController?> GetByControllerAdiAsync(string controllerAdi)
        {
            if (string.IsNullOrWhiteSpace(controllerAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(mc => mc.ModulControllerAdi == controllerAdi);
        }

        public async Task<IEnumerable<ModulController>> GetByModulAsync(int modulId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(mc => mc.ModulId == modulId)
                .OrderBy(mc => mc.ModulControllerAdi)
                .ToListAsync();
        }

        public async Task<ModulController?> GetWithIslemlerAsync(int modulControllerId)
        {
            return await _dbSet
                .Include(mc => mc.ModulControllerIslemler)
                .AsNoTracking()
                .FirstOrDefaultAsync(mc => mc.ModulControllerId == modulControllerId);
        }

        public async Task<IEnumerable<ModulController>> GetAllWithIslemlerAsync()
        {
            return await _dbSet
                .Include(mc => mc.ModulControllerIslemler)
                .Include(mc => mc.Modul)
                .AsNoTracking()
                .OrderBy(mc => mc.Modul.ModulAdi)
                .ThenBy(mc => mc.ModulControllerAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulController>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(mc => mc.ModulControllerAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModulController>> GetAllAsync()
        {
            return await _dbSet
                .Include(mc => mc.Modul)
                .AsNoTracking()
                .OrderBy(mc => mc.Modul.ModulAdi)
                .ThenBy(mc => mc.ModulControllerAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(mc => mc.ModulControllerAdi)
                .Select(mc => new ValueTuple<int, string>(mc.ModulControllerId, mc.ModulControllerAdi))
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetByModulDropdownAsync(int modulId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(mc => mc.ModulId == modulId)
                .OrderBy(mc => mc.ModulControllerAdi)
                .Select(mc => new ValueTuple<int, string>(mc.ModulControllerId, mc.ModulControllerAdi))
                .ToListAsync();
        }

        public async Task<int> GetIslemCountAsync(int modulControllerId)
        {
            return await _context.Set<ModulControllerIslem>()
                .AsNoTracking()
                .CountAsync(mci => mci.ModulControllerId == modulControllerId);
        }
    }
}
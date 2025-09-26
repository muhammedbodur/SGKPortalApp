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
    public class IlRepository : GenericRepository<Il>, IIlRepository
    {
        public IlRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Il?> GetByIlAdiAsync(string ilAdi)
        {
            if (string.IsNullOrWhiteSpace(ilAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IlAdi == ilAdi);
        }

        public async Task<Il?> GetWithIlcelerAsync(int ilId)
        {
            return await _dbSet
                .Include(i => i.Ilceler)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.IlId == ilId);
        }

        public async Task<IEnumerable<Il>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(i => i.IlAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(i => i.IlAdi)
                .Select(i => new ValueTuple<int, string>(i.IlId, i.IlAdi))
                .ToListAsync();
        }

        public async Task<int> GetIlceCountAsync(int ilId)
        {
            return await _context.Set<Ilce>()
                .AsNoTracking()
                .CountAsync(ic => ic.IlId == ilId);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class UnvanRepository : GenericRepository<Unvan>, IUnvanRepository
    {
        public UnvanRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Unvan?> GetByUnvanAdiAsync(string unvanAdi)
        {
            if (string.IsNullOrWhiteSpace(unvanAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UnvanAdi == unvanAdi && !u.SilindiMi);
        }

        public async Task<IEnumerable<Unvan>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.SilindiMi)
                .Select(u => new ValueTuple<int, string>(u.UnvanId, u.UnvanAdi))
                .ToListAsync();
        }
    }
}
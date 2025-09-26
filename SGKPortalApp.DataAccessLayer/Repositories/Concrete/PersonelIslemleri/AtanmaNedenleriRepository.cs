using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class AtanmaNedenleriRepository : GenericRepository<AtanmaNedenleri>, IAtanmaNedenleriRepository
    {
        public AtanmaNedenleriRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<AtanmaNedenleri?> GetByAtanmaNedeniAsync(string atanmaNedeni)
        {
            if (string.IsNullOrWhiteSpace(atanmaNedeni))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AtanmaNedeni == atanmaNedeni);
        }

        public async Task<IEnumerable<AtanmaNedenleri>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(a => a.AtanmaNedeni)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(a => a.AtanmaNedeni)
                .Select(a => new ValueTuple<int, string>(a.AtanmaNedeniId, a.AtanmaNedeni))
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int atanmaNedeniId)
        {
            return await _context.Set<Personel>()
                .AsNoTracking()
                .CountAsync(p => p.AtanmaNedeniId == atanmaNedeniId && !p.SilindiMi);
        }
    }
}
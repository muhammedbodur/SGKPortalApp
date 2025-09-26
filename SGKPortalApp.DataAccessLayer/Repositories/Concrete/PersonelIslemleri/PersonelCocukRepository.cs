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
    public class PersonelCocukRepository : GenericRepository<PersonelCocuk>, IPersonelCocukRepository
    {
        public PersonelCocukRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelCocuk>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelCocuk>();

            return await _dbSet
                .AsNoTracking()
                .Where(pc => pc.PersonelTcKimlikNo == tcKimlikNo && !pc.SilindiMi)
                .OrderBy(pc => pc.CocukAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelCocuk>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(pc => !pc.SilindiMi)
                .OrderBy(pc => pc.CocukAdi)
                .ToListAsync();
        }

        public async Task<int> GetCocukCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(pc => pc.PersonelTcKimlikNo == tcKimlikNo && !pc.SilindiMi);
        }
    }
}
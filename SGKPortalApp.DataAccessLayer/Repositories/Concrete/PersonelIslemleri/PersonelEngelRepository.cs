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
    public class PersonelEngelRepository : GenericRepository<PersonelEngel>, IPersonelEngelRepository
    {
        public PersonelEngelRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelEngel>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelEngel>();

            return await _dbSet
                .AsNoTracking()
                .Where(pe => pe.TcKimlikNo == tcKimlikNo && !pe.SilindiMi)
                .OrderBy(pe => pe.EngelDerecesi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelEngel>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(pe => !pe.SilindiMi)
                .OrderBy(pe => pe.EngelDerecesi)
                .ToListAsync();
        }

        public async Task<int> GetEngelCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(pe => pe.TcKimlikNo == tcKimlikNo && !pe.SilindiMi);
        }
    }
}

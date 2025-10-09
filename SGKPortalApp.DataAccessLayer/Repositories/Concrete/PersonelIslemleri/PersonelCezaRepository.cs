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
    public class PersonelCezaRepository : GenericRepository<PersonelCeza>, IPersonelCezaRepository
    {
        public PersonelCezaRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelCeza>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelCeza>();

            return await _dbSet
                .AsNoTracking()
                .Where(pc => pc.TcKimlikNo == tcKimlikNo && !pc.SilindiMi)
                .OrderByDescending(pc => pc.CezaTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelCeza>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(pc => !pc.SilindiMi)
                .OrderByDescending(pc => pc.CezaTarihi)
                .ToListAsync();
        }

        public async Task<int> GetCezaCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(pc => pc.TcKimlikNo == tcKimlikNo && !pc.SilindiMi);
        }
    }
}

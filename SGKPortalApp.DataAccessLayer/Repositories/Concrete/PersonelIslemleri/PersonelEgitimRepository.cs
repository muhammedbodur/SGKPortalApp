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
    public class PersonelEgitimRepository : GenericRepository<PersonelEgitim>, IPersonelEgitimRepository
    {
        public PersonelEgitimRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelEgitim>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelEgitim>();

            return await _dbSet
                .AsNoTracking()
                .Where(pe => pe.TcKimlikNo == tcKimlikNo && !pe.SilindiMi)
                .OrderByDescending(pe => pe.EgitimBaslangicTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelEgitim>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(pe => !pe.SilindiMi)
                .OrderByDescending(pe => pe.EgitimBaslangicTarihi)
                .ToListAsync();
        }

        public async Task<int> GetEgitimCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(pe => pe.TcKimlikNo == tcKimlikNo && !pe.SilindiMi);
        }
    }
}

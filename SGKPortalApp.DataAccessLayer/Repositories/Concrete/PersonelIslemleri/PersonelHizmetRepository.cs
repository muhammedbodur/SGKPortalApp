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
    public class PersonelHizmetRepository : GenericRepository<PersonelHizmet>, IPersonelHizmetRepository
    {
        public PersonelHizmetRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelHizmet>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelHizmet>();

            return await _dbSet
                .Include(ph => ph.Departman)
                .Include(ph => ph.Servis)
                .AsNoTracking()
                .Where(ph => ph.TcKimlikNo == tcKimlikNo && !ph.SilindiMi)
                .OrderByDescending(ph => ph.GorevBaslamaTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelHizmet>> GetAllActiveAsync()
        {
            return await _dbSet
                .Include(ph => ph.Departman)
                .Include(ph => ph.Servis)
                .AsNoTracking()
                .Where(ph => !ph.SilindiMi)
                .OrderByDescending(ph => ph.GorevBaslamaTarihi)
                .ToListAsync();
        }

        public async Task<int> GetHizmetCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(ph => ph.TcKimlikNo == tcKimlikNo && !ph.SilindiMi);
        }
    }
}

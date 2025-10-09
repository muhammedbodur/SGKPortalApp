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
    public class PersonelImzaYetkisiRepository : GenericRepository<PersonelImzaYetkisi>, IPersonelImzaYetkisiRepository
    {
        public PersonelImzaYetkisiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelImzaYetkisi>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelImzaYetkisi>();

            return await _dbSet
                .Include(piy => piy.Departman)
                .Include(piy => piy.Servis)
                .AsNoTracking()
                .Where(piy => piy.TcKimlikNo == tcKimlikNo && !piy.SilindiMi)
                .OrderByDescending(piy => piy.ImzaYetkisiBaslamaTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelImzaYetkisi>> GetActiveByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelImzaYetkisi>();

            var now = DateTime.Now;
            return await _dbSet
                .Include(piy => piy.Departman)
                .Include(piy => piy.Servis)
                .AsNoTracking()
                .Where(piy => piy.TcKimlikNo == tcKimlikNo && !piy.SilindiMi
                    && piy.ImzaYetkisiBaslamaTarihi <= now
                    && (piy.ImzaYetkisiBitisTarihi == null || piy.ImzaYetkisiBitisTarihi >= now))
                .OrderByDescending(piy => piy.ImzaYetkisiBaslamaTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelImzaYetkisi>> GetAllActiveAsync()
        {
            return await _dbSet
                .Include(piy => piy.Departman)
                .Include(piy => piy.Servis)
                .AsNoTracking()
                .Where(piy => !piy.SilindiMi)
                .OrderByDescending(piy => piy.ImzaYetkisiBaslamaTarihi)
                .ToListAsync();
        }

        public async Task<int> GetImzaYetkisiCountByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(piy => piy.TcKimlikNo == tcKimlikNo && !piy.SilindiMi);
        }
    }
}

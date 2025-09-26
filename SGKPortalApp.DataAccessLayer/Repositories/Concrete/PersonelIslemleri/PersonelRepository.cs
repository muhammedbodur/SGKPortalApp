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
    public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
    {
        public PersonelRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Personel?> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TcKimlikNo == tcKimlikNo && !p.SilindiMi);
        }

        public async Task<Personel?> GetBySicilNoAsync(int sicilNo)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.SicilNo == sicilNo && !p.SilindiMi);
        }

        public async Task<IEnumerable<Personel>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.DepartmanId == departmanId && !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetByUnvanAsync(int unvanId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.UnvanId == unvanId && !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<Personel>> GetActivePersonelAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string AdSoyad)>> GetPersonelDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => !p.SilindiMi)
                .Select(p => new ValueTuple<int, string>(p.SicilNo, p.AdSoyad))
                .ToListAsync();
        }
    }
}
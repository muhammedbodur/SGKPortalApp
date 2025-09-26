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
    public class PersonelYetkiRepository : GenericRepository<PersonelYetki>, IPersonelYetkiRepository
    {
        public PersonelYetkiRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelYetki>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelYetki>();

            return await _dbSet
                .Include(py => py.Yetki)
                .AsNoTracking()
                .Where(py => py.TcKimlikNo == tcKimlikNo)
                .OrderBy(py => py.Yetki.YetkiAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelYetki>> GetByYetkiAsync(int yetkiId)
        {
            return await _dbSet
                .Include(py => py.Personel)
                .AsNoTracking()
                .Where(py => py.YetkiId == yetkiId)
                .OrderBy(py => py.Personel.AdSoyad)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelYetki>> GetActivePermissionsAsync()
        {
            return await _dbSet
                .Include(py => py.Personel)
                .Include(py => py.Yetki)
                .AsNoTracking()
                .Where(py => !py.Personel.SilindiMi)
                .OrderBy(py => py.Personel.AdSoyad)
                .ThenBy(py => py.Yetki.YetkiAdi)
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(string tcKimlikNo, int yetkiId)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo) || yetkiId <= 0)
                return false;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(py => py.TcKimlikNo == tcKimlikNo && py.YetkiId == yetkiId);
        }
    }
}
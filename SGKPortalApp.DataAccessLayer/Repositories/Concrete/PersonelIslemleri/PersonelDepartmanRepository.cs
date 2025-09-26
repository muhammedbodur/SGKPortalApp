using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class PersonelDepartmanRepository : GenericRepository<PersonelDepartman>, IPersonelDepartmanRepository
    {
        public PersonelDepartmanRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PersonelDepartman>> GetByPersonelTcAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return Enumerable.Empty<PersonelDepartman>();

            return await _dbSet
                .Include(pd => pd.Departman)
                .AsNoTracking()
                .Where(pd => pd.TcKimlikNo == tcKimlikNo && !pd.SilindiMi)
                .OrderByDescending(pd => pd.BaslangicTarihi)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelDepartman>> GetByDepartmanAsync(int departmanId)
        {
            return await _dbSet
                .Include(pd => pd.Personel)
                .AsNoTracking()
                .Where(pd => pd.DepartmanId == departmanId && !pd.SilindiMi)
                .OrderBy(pd => pd.Personel.AdSoyad)
                .ToListAsync();
        }

        public async Task<IEnumerable<PersonelDepartman>> GetActiveAssignmentsAsync()
        {
            return await _dbSet
                .Include(pd => pd.Personel)
                .Include(pd => pd.Departman)
                .AsNoTracking()
                .Where(pd => pd.Aktiflik == Aktiflik.Aktif && !pd.SilindiMi)
                .OrderBy(pd => pd.Departman.DepartmanAdi)
                .ThenBy(pd => pd.Personel.AdSoyad)
                .ToListAsync();
        }

        public async Task<PersonelDepartman?> GetActiveAssignmentByPersonelAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            return await _dbSet
                .Include(pd => pd.Departman)
                .AsNoTracking()
                .FirstOrDefaultAsync(pd => pd.TcKimlikNo == tcKimlikNo &&
                                          pd.Aktiflik == Aktiflik.Aktif &&
                                          !pd.SilindiMi);
        }
    }
}
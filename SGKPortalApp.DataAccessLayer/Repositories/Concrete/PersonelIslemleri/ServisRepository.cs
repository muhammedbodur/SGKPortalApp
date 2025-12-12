using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class ServisRepository : GenericRepository<Servis>, IServisRepository
    {
        public ServisRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Servis?> GetByServisAdiAsync(string servisAdi)
        {
            if (string.IsNullOrWhiteSpace(servisAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServisAdi == servisAdi && !s.SilindiMi);
        }

        public async Task<IEnumerable<Servis>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.Aktiflik == Aktiflik.Aktif && !s.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.Aktiflik == Aktiflik.Aktif && !s.SilindiMi)
                .Select(s => new ValueTuple<int, string>(s.ServisId, s.ServisAdi))
                .ToListAsync();
        }
    }
}
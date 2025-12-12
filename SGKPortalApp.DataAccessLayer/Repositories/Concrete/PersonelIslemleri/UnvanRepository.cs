using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.PersonelIslemleri
{
    public class UnvanRepository : GenericRepository<Unvan>, IUnvanRepository
    {
        public UnvanRepository(SGKDbContext context) : base(context)
        {
        }

        public override async Task<Unvan?> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(u => u.Personeller.Where(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif))
                .FirstOrDefaultAsync(u => u.UnvanId == (int)id && !u.SilindiMi);
        }

        public async Task<Unvan?> GetByUnvanAdiAsync(string unvanAdi)
        {
            if (string.IsNullOrWhiteSpace(unvanAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UnvanAdi == unvanAdi && !u.SilindiMi);
        }

        public async Task<IEnumerable<Unvan>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Personeller.Where(p => !p.SilindiMi && p.PersonelAktiflikDurum == PersonelAktiflikDurum.Aktif))
                .Where(u => !u.SilindiMi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => !u.SilindiMi)
                .Select(u => new ValueTuple<int, string>(u.UnvanId, u.UnvanAdi))
                .ToListAsync();
        }
    }
}
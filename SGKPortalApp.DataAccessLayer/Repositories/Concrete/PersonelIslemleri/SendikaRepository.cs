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
    public class SendikaRepository : GenericRepository<Sendika>, ISendikaRepository
    {
        public SendikaRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<Sendika?> GetBySendikaAdiAsync(string sendikaAdi)
        {
            if (string.IsNullOrWhiteSpace(sendikaAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SendikaAdi == sendikaAdi);
        }

        public async Task<IEnumerable<Sendika>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(s => s.SendikaAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(s => s.SendikaAdi)
                .Select(s => new ValueTuple<int, string>(s.SendikaId, s.SendikaAdi))
                .ToListAsync();
        }

        public async Task<int> GetPersonelCountAsync(int sendikaId)
        {
            return await _context.Set<Personel>()
                .AsNoTracking()
                .CountAsync(p => p.SendikaId == sendikaId && !p.SilindiMi);
        }
    }
}
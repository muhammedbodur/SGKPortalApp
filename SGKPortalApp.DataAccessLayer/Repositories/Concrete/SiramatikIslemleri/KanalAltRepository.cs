using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KanalAltRepository : GenericRepository<KanalAlt>, IKanalAltRepository
    {
        public KanalAltRepository(SGKDbContext context) : base(context) { }

        // Kanal bazında alt kanalları listeler
        public async Task<IEnumerable<KanalAlt>> GetByKanalAsync(int kanalId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ka => ka.KanalId == kanalId)
                .ToListAsync();
        }

        // Alt kanal adıyla alt kanalı getirir
        public async Task<KanalAlt?> GetByAdAsync(string kanalAltAdi)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(ka => ka.KanalAltAdi == kanalAltAdi);
        }

        // Alt kanalı detaylı getirir
        public async Task<KanalAlt?> GetWithDetailsAsync(int kanalAltId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ka => ka.KanalAltIslemleri)
                .FirstOrDefaultAsync(ka => ka.KanalAltId == kanalAltId);
        }

        // Tüm alt kanalları detaylı listeler
        public async Task<IEnumerable<KanalAlt>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ka => ka.KanalAltIslemleri)
                .ToListAsync();
        }

        // Aktif alt kanalları listeler
        public async Task<IEnumerable<KanalAlt>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ka => ka.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Dropdown için alt kanalları listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(ka => new ValueTuple<int, string>(ka.KanalAltId, ka.KanalAltAdi))
                .ToListAsync();
        }
    }
}
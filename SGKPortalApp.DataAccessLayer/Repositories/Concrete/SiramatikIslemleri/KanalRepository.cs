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
    public class KanalRepository : GenericRepository<Kanal>, IKanalRepository
    {
        public KanalRepository(SGKDbContext context) : base(context) { }

        // Kanal adıyla kanalı getirir
        public async Task<Kanal?> GetByKanalAdiAsync(string kanalAdi)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.KanalAdi == kanalAdi);
        }

        // Kanalı alt kanallarıyla getirir
        public async Task<Kanal?> GetWithKanalAltAsync(int kanalId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.KanalAltlari)
                .FirstOrDefaultAsync(k => k.KanalId == kanalId);
        }

        // Tüm kanalları alt kanallarıyla listeler
        public async Task<IEnumerable<Kanal>> GetAllWithKanalAltAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.KanalAltlari)
                .ToListAsync();
        }

        // Aktif kanalları listeler
        public async Task<IEnumerable<Kanal>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(k => k.Aktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm kanalları listeler
        public async Task<IEnumerable<Kanal>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Dropdown için kanalları listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(k => new ValueTuple<int, string>(k.KanalId, k.KanalAdi))
                .ToListAsync();
        }

        // Kanalın alt kanal sayısını getirir
        public async Task<int> GetKanalAltCountAsync(int kanalId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.KanalAltlari)
                .Where(k => k.KanalId == kanalId)
                .Select(k => k.KanalAltlari!.Count)
                .FirstOrDefaultAsync();
        }
    }
}
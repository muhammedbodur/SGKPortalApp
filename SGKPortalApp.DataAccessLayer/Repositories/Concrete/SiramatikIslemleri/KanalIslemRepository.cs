using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class KanalIslemRepository : GenericRepository<KanalIslem>, IKanalIslemRepository
    {
        public KanalIslemRepository(SGKDbContext context) : base(context) { }

        // Kanal bazında işlemleri listeler
        public async Task<IEnumerable<KanalIslem>> GetByKanalAsync(int kanalId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Where(ki => ki.KanalId == kanalId)
                .OrderBy(ki => ki.Sira)
                .ToListAsync();
        }

        // Hizmet binası bazında işlemleri listeler
        public async Task<IEnumerable<KanalIslem>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Include(ki => ki.KanalAltIslemleri)
                .Where(ki => ki.HizmetBinasiId == hizmetBinasiId)
                .OrderBy(ki => ki.Sira)
                .ToListAsync();
        }

        // Tarih aralığına göre işlemleri listeler
        public async Task<IEnumerable<KanalIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Where(ki => ki.EklenmeTarihi >= startDate && ki.EklenmeTarihi <= endDate)
                .OrderBy(ki => ki.EklenmeTarihi)
                .ToListAsync();
        }

        // İşlemi detaylı getirir
        public async Task<KanalIslem?> GetWithDetailsAsync(int kanalIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Include(ki => ki.KanalAltIslemleri)
                .FirstOrDefaultAsync(ki => ki.KanalIslemId == kanalIslemId);
        }

        // Tüm işlemleri detaylı listeler
        public async Task<IEnumerable<KanalIslem>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Include(ki => ki.KanalAltIslemleri)
                .OrderBy(ki => ki.Sira)
                .ToListAsync();
        }

        // Aktif işlemleri listeler
        public async Task<IEnumerable<KanalIslem>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Where(ki => ki.Aktiflik == Aktiflik.Aktif)
                .OrderBy(ki => ki.Sira)
                .ToListAsync();
        }

        // Numara aralığına göre işlemleri listeler
        public async Task<IEnumerable<KanalIslem>> GetByNumaraAraligiAsync(int baslangicNumara, int bitisNumara)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ki => ki.Kanal)
                .Include(ki => ki.HizmetBinasi)
                .Where(ki => ki.BaslangicNumara >= baslangicNumara && ki.BitisNumara <= bitisNumara)
                .OrderBy(ki => ki.Sira)
                .ToListAsync();
        }

        // Günlük işlem sayısını getirir
        public async Task<int> GetDailyIslemCountAsync(int kanalId, DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(ki => ki.KanalId == kanalId && ki.EklenmeTarihi.Date == date.Date);
        }
    }
}
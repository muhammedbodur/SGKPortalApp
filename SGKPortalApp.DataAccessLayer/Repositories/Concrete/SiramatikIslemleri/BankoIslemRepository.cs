using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class BankoIslemRepository : GenericRepository<BankoIslem>, IBankoIslemRepository
    {
        public BankoIslemRepository(SGKDbContext context) : base(context) { }

        // Banko grubuna göre işlemleri listeler
        public async Task<IEnumerable<BankoIslem>> GetByBankoGrupAsync(BankoGrup bankoGrup)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bi => bi.BankoGrup == bankoGrup)
                .ToListAsync();
        }

        // Üst işlem ID'sine göre işlemleri listeler
        public async Task<IEnumerable<BankoIslem>> GetByUstIslemAsync(int bankoUstIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bi => bi.BankoUstIslemId == bankoUstIslemId)
                .ToListAsync();
        }

        // Tarih aralığına göre işlemleri listeler
        public async Task<IEnumerable<BankoIslem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bi => bi.EklenmeTarihi >= startDate && bi.EklenmeTarihi <= endDate)
                .ToListAsync();
        }

        // İşlemi detaylı getirir
        public async Task<BankoIslem?> GetWithDetailsAsync(int bankoIslemId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(bi => bi.BankoIslemId == bankoIslemId);
        }

        // Tüm işlemleri detaylı listeler
        public async Task<IEnumerable<BankoIslem>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Aktif işlemleri listeler
        public async Task<IEnumerable<BankoIslem>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bi => bi.BankoIslemAktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Dropdown için işlemleri listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bi => bi.BankoIslemAdi != null)
                .Select(bi => new ValueTuple<int, string>(bi.BankoIslemId, bi.BankoIslemAdi!))
                .ToListAsync();
        }

        // Günlük işlem sayısını getirir
        public async Task<int> GetDailyIslemCountAsync(DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(bi => bi.EklenmeTarihi.Date == date.Date);
        }
    }
}
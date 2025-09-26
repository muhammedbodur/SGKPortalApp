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
    public class BankoKullaniciRepository : GenericRepository<BankoKullanici>, IBankoKullaniciRepository
    {
        public BankoKullaniciRepository(SGKDbContext context) : base(context) { }

        // Bankoya atanmış kullanıcıyı getirir
        public async Task<BankoKullanici?> GetByBankoAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .FirstOrDefaultAsync(bk => bk.BankoId == bankoId);
        }

        // Personele atanmış banko kullanıcısını getirir
        public async Task<BankoKullanici?> GetByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .FirstOrDefaultAsync(bk => bk.TcKimlikNo == tcKimlikNo);
        }

        // Kullanıcıyı detaylı getirir
        public async Task<BankoKullanici?> GetWithDetailsAsync(int bankoKullaniciId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .FirstOrDefaultAsync(bk => bk.BankoKullaniciId == bankoKullaniciId);
        }

        // Tüm kullanıcıları detaylı listeler
        public async Task<IEnumerable<BankoKullanici>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .ToListAsync();
        }

        // Aktif atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetActiveAssignmentsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .Where(bk => bk.Banko.BankoAktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Tarih aralığına göre atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bk => bk.EklenmeTarihi >= startDate && bk.EklenmeTarihi <= endDate)
                .ToListAsync();
        }
    }
}
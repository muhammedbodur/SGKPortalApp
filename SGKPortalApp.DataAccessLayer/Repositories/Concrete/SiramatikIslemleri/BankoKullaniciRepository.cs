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
                .Where(bk => !bk.SilindiMi) // Explicit SilindiMi kontrolü
                .FirstOrDefaultAsync(bk => bk.BankoId == bankoId);
        }

        // Personele atanmış banko kullanıcısını getirir
        // ⭐ ÖNEMLİ: HizmetBinasiId'ye bakmadan TÜM kayıtları getir
        // Çünkü personel hizmet binası değiştirdiğinde ESKİ kaydı bulmamız gerekiyor
        public async Task<BankoKullanici?> GetByPersonelAsync(string tcKimlikNo)
        {
            return await _dbSet
                .IgnoreQueryFilters() // ⭐ Query filter'ı devre dışı bırak (SilindiMi kontrolü manuel yapılacak)
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .Where(bk => !bk.SilindiMi) // Explicit SilindiMi kontrolü
                .OrderByDescending(bk => bk.EklenmeTarihi) // En son eklenen kayıt
                .FirstOrDefaultAsync(bk => bk.TcKimlikNo == tcKimlikNo);
        }

        // Kullanıcıyı detaylı getirir
        public async Task<BankoKullanici?> GetWithDetailsAsync(int bankoKullaniciId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .Where(bk => !bk.SilindiMi)
                .FirstOrDefaultAsync(bk => bk.BankoKullaniciId == bankoKullaniciId);
        }

        // Tüm kullanıcıları detaylı listeler
        public async Task<IEnumerable<BankoKullanici>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .Where(bk => !bk.SilindiMi)
                .ToListAsync();
        }

        // Aktif atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetActiveAssignmentsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(bk => bk.Banko)
                .Include(bk => bk.Personel)
                .Where(bk => !bk.SilindiMi && bk.Banko.BankoAktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bk => !bk.SilindiMi)
                .ToListAsync();
        }

        // Tarih aralığına göre atamaları listeler
        public async Task<IEnumerable<BankoKullanici>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bk => !bk.SilindiMi && bk.EklenmeTarihi >= startDate && bk.EklenmeTarihi <= endDate)
                .ToListAsync();
        }

        // Banko atanmış mı kontrol eder
        public async Task<bool> IsBankoAssignedAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bk => !bk.SilindiMi)
                .AnyAsync(bk => bk.BankoId == bankoId);
        }

        // Personel atanmış mı kontrol eder
        public async Task<bool> IsPersonelAssignedAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(bk => !bk.SilindiMi)
                .AnyAsync(bk => bk.TcKimlikNo == tcKimlikNo);
        }

        // Personeli bankosundan çıkarır
        public async Task UnassignPersonelAsync(string tcKimlikNo)
        {
            var assignment = await _dbSet
                .Where(bk => !bk.SilindiMi)
                .FirstOrDefaultAsync(bk => bk.TcKimlikNo == tcKimlikNo);

            if (assignment != null)
            {
                _dbSet.Remove(assignment);
            }
        }

        // Bankoyu boşaltır
        public async Task UnassignBankoAsync(int bankoId)
        {
            var assignment = await _dbSet
                .Where(bk => !bk.SilindiMi)
                .FirstOrDefaultAsync(bk => bk.BankoId == bankoId);

            if (assignment != null)
            {
                _dbSet.Remove(assignment);
            }
        }
    }
}
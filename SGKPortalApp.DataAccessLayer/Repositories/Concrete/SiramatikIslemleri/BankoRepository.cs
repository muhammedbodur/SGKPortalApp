using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SiramatikIslemleri
{
    public class BankoRepository : GenericRepository<Banko>, IBankoRepository
    {
        public BankoRepository(SGKDbContext context) : base(context) { }

        // Banko numarası ve hizmet binasıyla bankoyu getirir
        public async Task<Banko?> GetByBankoNoAsync(int bankoNo, int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BankoNo == bankoNo && b.HizmetBinasiId == hizmetBinasiId);
        }

        // Hizmet binasındaki bankoları listeler
        public async Task<IEnumerable<Banko>> GetByHizmetBinasiAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.HizmetBinasiId == hizmetBinasiId)
                .ToListAsync();
        }

        // Bankoyu hizmet binası ile getirir
        public async Task<Banko?> GetWithHizmetBinasiAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.HizmetBinasi)
                .FirstOrDefaultAsync(b => b.BankoId == bankoId);
        }

        // Tüm bankoları hizmet binası ile listeler
        public async Task<IEnumerable<Banko>> GetAllWithHizmetBinasiAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.HizmetBinasi)
                .ToListAsync();
        }

        // Kullanıcı atanmış bankoları listeler
        public async Task<IEnumerable<Banko>> GetWithKullaniciAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.BankoKullanicilari)
                .Where(b => b.BankoKullanicilari != null && b.BankoKullanicilari.Any())
                .ToListAsync();
        }

        // Personelin bankosunu getirir
        public async Task<Banko?> GetByKullaniciAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.BankoKullanicilari)
                .FirstOrDefaultAsync(b => b.BankoKullanicilari != null && b.BankoKullanicilari.Any(bk => bk.TcKimlikNo == tcKimlikNo));
        }

        // Aktif bankoları listeler
        public async Task<IEnumerable<Banko>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.BankoAktiflik == Aktiflik.Aktif)
                .ToListAsync();
        }

        // Tüm bankoları listeler
        public async Task<IEnumerable<Banko>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // Dropdown için bankoları listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(b => new ValueTuple<int, string>(b.BankoId, b.BankoNo.ToString()))
                .ToListAsync();
        }

        // Hizmet binası için dropdown bankoları listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.HizmetBinasiId == hizmetBinasiId)
                .Select(b => new ValueTuple<int, string>(b.BankoId, b.BankoNo.ToString()))
                .ToListAsync();
        }

        // Hizmet binasındaki aktif banko sayısını getirir
        public async Task<int> GetActiveBankoCountAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(b => b.HizmetBinasiId == hizmetBinasiId && b.BankoAktiflik == Aktiflik.Aktif);
        }

        // Banko tipine göre bankoları listeler
        public async Task<IEnumerable<Banko>> GetByBankoTipiAsync(BankoTipi bankoTipi)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.BankoTipi == bankoTipi)
                .ToListAsync();
        }

        // Kat tipine göre bankoları listeler
        public async Task<IEnumerable<Banko>> GetByKatTipiAsync(KatTipi katTipi)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.KatTipi == katTipi)
                .ToListAsync();
        }
    }
}
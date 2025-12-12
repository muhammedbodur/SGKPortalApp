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
                .Include(b => b.HizmetBinasi)
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
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
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                .Where(b => b.BankoKullanicilari != null && b.BankoKullanicilari.Any(bk => !bk.SilindiMi))
                .ToListAsync();
        }

        // Personelin bankosunu getirir
        // ⭐ ARTıK ÇOK BASİT: HizmetBinasiId BankoKullanici'da olduğu için database garantisi var
        public async Task<Banko?> GetByKullaniciAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                .Include(b => b.HizmetBinasi)
                .FirstOrDefaultAsync(b => b.BankoKullanicilari != null &&
                                         b.BankoKullanicilari.Any(bk => bk.TcKimlikNo == tcKimlikNo && !bk.SilindiMi));
            // ⭐ HizmetBinasiId kontrolü artık gerekli değil - database seviyesinde garanti ediliyor!
        }

        // Aktif bankoları listeler
        public async Task<IEnumerable<Banko>> GetActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.Aktiflik == Aktiflik.Aktif)
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
                .CountAsync(b => b.HizmetBinasiId == hizmetBinasiId && b.Aktiflik == Aktiflik.Aktif);
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

        // Boş bankoları listeler (personel atanmamış)
        public async Task<IEnumerable<Banko>> GetAvailableBankosAsync(int hizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.HizmetBinasi)
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                .Where(b => b.HizmetBinasiId == hizmetBinasiId
                         && b.Aktiflik == Aktiflik.Aktif
                         && (b.BankoKullanicilari == null || !b.BankoKullanicilari.Any()))
                .OrderBy(b => b.KatTipi)
                .ThenBy(b => b.BankoNo)
                .ToListAsync();
        }

        // Bankoyu atanmış personel ile getirir
        public async Task<Banko?> GetWithPersonelAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.HizmetBinasi)
                .Include(b => b.BankoKullanicilari!.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
                        .ThenInclude(p => p.Servis)
                .FirstOrDefaultAsync(b => b.BankoId == bankoId);
        }

        // Bina bazlı kat gruplu bankoları getirir
        public async Task<Dictionary<KatTipi, List<Banko>>> GetGroupedByKatAsync(int hizmetBinasiId)
        {
            var bankolar = await _dbSet
                .AsNoTracking()
                .Include(b => b.HizmetBinasi)
                .Include(b => b.BankoKullanicilari!.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
                        .ThenInclude(p => p.Servis)
                .Where(b => b.HizmetBinasiId == hizmetBinasiId)
                .OrderBy(b => b.BankoNo)
                .ToListAsync();

            return bankolar
                .GroupBy(b => b.KatTipi)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
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

        // Banko numarası ve departman-bina kombinasyonuyla bankoyu getirir
        public async Task<Banko?> GetByBankoNoAsync(int bankoNo, int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BankoNo == bankoNo && b.DepartmanHizmetBinasiId == departmanHizmetBinasiId);
        }

        // Departman-hizmet binasındaki bankoları listeler
        public async Task<IEnumerable<Banko>> GetByDepartmanHizmetBinasiAsync(int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
                .Where(b => b.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                .ToListAsync();
        }

        // Bankoyu departman-hizmet binası ile getirir
        public async Task<Banko?> GetWithDepartmanHizmetBinasiAsync(int bankoId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .FirstOrDefaultAsync(b => b.BankoId == bankoId);
        }

        // Tüm bankoları departman-hizmet binası ile listeler
        public async Task<IEnumerable<Banko>> GetAllWithDepartmanHizmetBinasiAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
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
        public async Task<Banko?> GetByKullaniciAsync(string tcKimlikNo)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .FirstOrDefaultAsync(b => b.BankoKullanicilari != null &&
                                         b.BankoKullanicilari.Any(bk => bk.TcKimlikNo == tcKimlikNo && !bk.SilindiMi));
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

        // Departman-hizmet binası için dropdown bankoları listeler
        public async Task<IEnumerable<(int Id, string Ad)>> GetByDepartmanHizmetBinasiDropdownAsync(int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(b => b.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                .Select(b => new ValueTuple<int, string>(b.BankoId, b.BankoNo.ToString()))
                .ToListAsync();
        }

        // Departman-hizmet binasındaki aktif banko sayısını getirir
        public async Task<int> GetActiveBankoCountAsync(int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(b => b.DepartmanHizmetBinasiId == departmanHizmetBinasiId && b.Aktiflik == Aktiflik.Aktif);
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
        public async Task<IEnumerable<Banko>> GetAvailableBankosAsync(int departmanHizmetBinasiId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Include(b => b.BankoKullanicilari.Where(bk => !bk.SilindiMi))
                .Where(b => b.DepartmanHizmetBinasiId == departmanHizmetBinasiId
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
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Include(b => b.BankoKullanicilari!.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
                        .ThenInclude(p => p.Servis)
                .FirstOrDefaultAsync(b => b.BankoId == bankoId);
        }

        // Bina bazlı kat gruplu bankoları getirir
        public async Task<Dictionary<KatTipi, List<Banko>>> GetGroupedByKatAsync(int departmanHizmetBinasiId)
        {
            var bankolar = await _dbSet
                .AsNoTracking()
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.Departman)
                .Include(b => b.DepartmanHizmetBinasi)
                    .ThenInclude(dhb => dhb.HizmetBinasi)
                .Include(b => b.BankoKullanicilari!.Where(bk => !bk.SilindiMi))
                    .ThenInclude(bk => bk.Personel)
                        .ThenInclude(p => p.Servis)
                .Where(b => b.DepartmanHizmetBinasiId == departmanHizmetBinasiId)
                .OrderBy(b => b.BankoNo)
                .ToListAsync();

            return bankolar
                .GroupBy(b => b.KatTipi)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
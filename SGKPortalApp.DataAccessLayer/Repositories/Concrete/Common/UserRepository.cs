using Microsoft.EntityFrameworkCore;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.Common
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SGKDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByTcKimlikNoAsync(string tcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return null;

            // ⚠️ AsNoTracking kaldırıldı - Login sırasında SessionID güncellemesi için tracking gerekli
            return await _dbSet
                .Include(u => u.Personel)
                    .ThenInclude(p => p.Departman)
                .Include(u => u.Personel)
                    .ThenInclude(p => p.Servis)
                .Include(u => u.Personel)
                    .ThenInclude(p => p.HizmetBinasi)
                .FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo && !u.SilindiMi);
        }

        public async Task<User?> GetBySessionIdAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return null;

            return await _dbSet
                .AsNoTracking()
                .Include(u => u.Personel)
                .FirstOrDefaultAsync(u => u.SessionID == sessionId && !u.SilindiMi);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.AktifMi && !u.SilindiMi)
                .OrderBy(u => u.TcKimlikNo)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetLockedUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.HesapKilitTarihi.HasValue && !u.SilindiMi)
                .OrderBy(u => u.TcKimlikNo)
                .ToListAsync();
        }

        public async Task UpdateLastLoginAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user != null)
            {
                user.SonGirisTarihi = DateTime.Now;
                user.BasarisizGirisSayisi = 0;
            }
        }

        public async Task IncrementFailedLoginAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user != null)
            {
                user.BasarisizGirisSayisi++;
            }
        }

        public async Task ResetFailedLoginAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user != null)
            {
                user.BasarisizGirisSayisi = 0;
            }
        }

        public async Task LockUserAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user != null)
            {
                user.HesapKilitTarihi = DateTime.Now;
                user.AktifMi = false;
            }
        }

        public async Task UnlockUserAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user != null)
            {
                user.HesapKilitTarihi = null;
                user.AktifMi = true;
                user.BasarisizGirisSayisi = 0;
            }
        }

        public async Task<bool> ActivateBankoModeAsync(string tcKimlikNo, int bankoId)
        {
            // Debug logging
            Console.WriteLine($"[UserRepository] ActivateBankoModeAsync çağrıldı - TcKimlikNo: '{tcKimlikNo}', BankoId: {bankoId}");

            // SilindiMi filtresi eklensin (diğer metodlarla tutarlılık)
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo && !u.SilindiMi);

            if (user == null)
            {
                Console.WriteLine($"[UserRepository] HATA: Kullanıcı bulunamadı! TcKimlikNo: '{tcKimlikNo}'");
                // Tüm kullanıcıları kontrol et (debug)
                var allUsers = await _dbSet.Select(u => new { u.TcKimlikNo, u.SilindiMi }).ToListAsync();
                Console.WriteLine($"[UserRepository] Toplam {allUsers.Count} kullanıcı var. İlk 5: {string.Join(", ", allUsers.Take(5).Select(u => $"{u.TcKimlikNo}(Silinmis:{u.SilindiMi})"))}");
                return false;
            }

            Console.WriteLine($"[UserRepository] Kullanıcı bulundu: {user.TcKimlikNo}, Aktif: {user.AktifMi}");
            user.BankoModuAktif = true;
            user.AktifBankoId = bankoId;
            user.BankoModuBaslangic = DateTime.Now;
            Console.WriteLine($"[UserRepository] Banko modu aktif edildi");

            return true;
        }

        public async Task<bool> DeactivateBankoModeAsync(string tcKimlikNo)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo);
            if (user == null)
                return false;

            user.BankoModuAktif = false;
            user.AktifBankoId = null;
            // BankoModuBaslangic'i null yapma - log için sakla

            return true;
        }

        public async Task<bool> IsBankoModeActiveAsync(string tcKimlikNo)
        {
            var user = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo && !u.SilindiMi);

            return user?.BankoModuAktif ?? false;
        }

        public async Task<int?> GetActiveBankoIdAsync(string tcKimlikNo)
        {
            var user = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo && !u.SilindiMi);

            return user?.AktifBankoId;
        }
    }
}
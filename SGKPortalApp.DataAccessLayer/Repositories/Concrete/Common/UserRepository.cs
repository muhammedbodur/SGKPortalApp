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

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TcKimlikNo == tcKimlikNo && !u.SilindiMi);
        }

        public async Task<User?> GetByKullaniciAdiAsync(string kullaniciAdi)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.KullaniciAdi == kullaniciAdi && !u.SilindiMi);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && !u.SilindiMi);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.AktifMi && !u.SilindiMi)
                .OrderBy(u => u.KullaniciAdi)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetLockedUsersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.HesapKilitTarihi.HasValue && !u.SilindiMi)
                .OrderBy(u => u.KullaniciAdi)
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

        public async Task<IEnumerable<(string TcKimlikNo, string KullaniciAdi)>> GetDropdownAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.AktifMi && !u.SilindiMi)
                .OrderBy(u => u.KullaniciAdi)
                .Select(u => new ValueTuple<string, string>(u.TcKimlikNo, u.KullaniciAdi))
                .ToListAsync();
        }
    }
}
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Authentication
        Task<User?> GetByTcKimlikNoAsync(string tcKimlikNo);
        Task<User?> GetByKullaniciAdiAsync(string kullaniciAdi);
        Task<User?> GetByEmailAsync(string email);

        // Kullanıcı durumu
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetLockedUsersAsync();

        // Login işlemleri
        Task UpdateLastLoginAsync(string tcKimlikNo);
        Task IncrementFailedLoginAsync(string tcKimlikNo);
        Task ResetFailedLoginAsync(string tcKimlikNo);
        Task LockUserAsync(string tcKimlikNo);
        Task UnlockUserAsync(string tcKimlikNo);

        // Dropdown için
        Task<IEnumerable<(string TcKimlikNo, string KullaniciAdi)>> GetDropdownAsync();
    }
}
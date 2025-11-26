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

        // Kullanıcı durumu
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetLockedUsersAsync();

        // Login işlemleri
        Task UpdateLastLoginAsync(string tcKimlikNo);
        Task IncrementFailedLoginAsync(string tcKimlikNo);
        Task ResetFailedLoginAsync(string tcKimlikNo);
        Task LockUserAsync(string tcKimlikNo);
        Task UnlockUserAsync(string tcKimlikNo);

        // Banko Modu işlemleri
        Task<bool> ActivateBankoModeAsync(string tcKimlikNo, int bankoId);
        Task<bool> DeactivateBankoModeAsync(string tcKimlikNo);
        Task<bool> IsBankoModeActiveAsync(string tcKimlikNo);
        Task<int?> GetActiveBankoIdAsync(string tcKimlikNo);
    }
}
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    /// <summary>
    /// ZKTeco User yönetim servisi
    /// Database (ZKTecoUsers) + Device (ZKTecoApi) kombinasyonu
    /// </summary>
    public interface IZKTecoUserService
    {
        // ========== Database Operations ==========

        Task<List<ZKTecoUserDto>> GetAllUsersAsync();
        Task<ZKTecoUserDto?> GetUserByIdAsync(int id);
        Task<List<ZKTecoUserDto>> GetUsersByDeviceIdAsync(int deviceId);
        Task<ZKTecoUser> CreateUserAsync(CreateZKTecoUserDto dto);
        Task<ZKTecoUser> UpdateUserAsync(int id, UpdateZKTecoUserDto dto);
        Task<bool> DeleteUserAsync(int id);

        // ========== Device Sync Operations ==========

        Task<List<ZKTecoApiUserDto>> GetUsersFromDeviceAsync(int deviceId);
        Task<bool> SyncUserToDeviceAsync(int userId, int deviceId);
        Task<bool> SyncAllUsersToDeviceAsync(int deviceId);
        Task<bool> SyncUsersFromDeviceToDbAsync(int deviceId);
        Task<bool> AddUserToDeviceAsync(int deviceId, ZKTecoApiUserDto user);
        Task<bool> UpdateUserOnDeviceAsync(int deviceId, string enrollNumber, ZKTecoApiUserDto user);
        Task<bool> DeleteUserFromDeviceAsync(int deviceId, string enrollNumber);
        Task<bool> ClearAllUsersFromDeviceAsync(int deviceId);
    }
}

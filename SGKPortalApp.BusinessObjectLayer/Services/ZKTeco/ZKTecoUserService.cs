using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    public class ZKTecoUserService : IZKTecoUserService
    {
        private readonly SGKDbContext _dbContext;
        private readonly IZKTecoApiClient _apiClient;
        private readonly IZKTecoDeviceService _deviceService;
        private readonly ILogger<ZKTecoUserService> _logger;

        public ZKTecoUserService(
            SGKDbContext dbContext,
            IZKTecoApiClient apiClient,
            IZKTecoDeviceService deviceService,
            ILogger<ZKTecoUserService> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _deviceService = deviceService;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<ZKTecoUserDto>> GetAllUsersAsync()
        {
            return await _dbContext.ZKTecoUsers
                .Include(u => u.Device)
                .Select(u => new ZKTecoUserDto
                {
                    Id = u.Id,
                    EnrollNumber = u.EnrollNumber,
                    Name = u.Name,
                    Password = u.Password,
                    CardNumber = u.CardNumber,
                    Privilege = u.Privilege,
                    Enabled = u.Enabled,
                    DeviceId = u.DeviceId,
                    DeviceIp = u.DeviceIp,
                    DeviceName = u.Device != null ? u.Device.DeviceName : null,
                    LastSyncTime = u.LastSyncTime,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<ZKTecoUserDto?> GetUserByIdAsync(int id)
        {
            return await _dbContext.ZKTecoUsers
                .Include(u => u.Device)
                .Where(u => u.Id == id)
                .Select(u => new ZKTecoUserDto
                {
                    Id = u.Id,
                    EnrollNumber = u.EnrollNumber,
                    Name = u.Name,
                    Password = u.Password,
                    CardNumber = u.CardNumber,
                    Privilege = u.Privilege,
                    Enabled = u.Enabled,
                    DeviceId = u.DeviceId,
                    DeviceIp = u.DeviceIp,
                    DeviceName = u.Device != null ? u.Device.DeviceName : null,
                    LastSyncTime = u.LastSyncTime,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ZKTecoUserDto>> GetUsersByDeviceIdAsync(int deviceId)
        {
            return await _dbContext.ZKTecoUsers
                .Include(u => u.Device)
                .Where(u => u.DeviceId == deviceId)
                .Select(u => new ZKTecoUserDto
                {
                    Id = u.Id,
                    EnrollNumber = u.EnrollNumber,
                    Name = u.Name,
                    Password = u.Password,
                    CardNumber = u.CardNumber,
                    Privilege = u.Privilege,
                    Enabled = u.Enabled,
                    DeviceId = u.DeviceId,
                    DeviceIp = u.DeviceIp,
                    DeviceName = u.Device != null ? u.Device.DeviceName : null,
                    LastSyncTime = u.LastSyncTime,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<ZKTecoUser> CreateUserAsync(CreateZKTecoUserDto dto)
        {
            var user = new ZKTecoUser
            {
                EnrollNumber = dto.EnrollNumber,
                Name = dto.Name,
                Password = dto.Password,
                CardNumber = dto.CardNumber,
                Privilege = dto.Privilege,
                Enabled = dto.Enabled,
                DeviceId = dto.DeviceId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbContext.ZKTecoUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User created: {user.Name} ({user.EnrollNumber})");
            return user;
        }

        public async Task<ZKTecoUser> UpdateUserAsync(int id, UpdateZKTecoUserDto dto)
        {
            var user = await _dbContext.ZKTecoUsers.FindAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Name = dto.Name;
            user.Password = dto.Password;
            user.CardNumber = dto.CardNumber;
            user.Privilege = dto.Privilege;
            user.Enabled = dto.Enabled;
            user.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User updated: {user.Name}");
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _dbContext.ZKTecoUsers.FindAsync(id);
            if (user == null) return false;

            _dbContext.ZKTecoUsers.Remove(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User deleted: {user.Name}");
            return true;
        }

        // ========== Device Sync Operations ==========

        public async Task<List<ZKTecoApiUserDto>> GetUsersFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return new List<ZKTecoApiUserDto>();

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetAllUsersFromDeviceAsync(device.IpAddress, port);
        }

        public async Task<bool> SyncUserToDeviceAsync(int userId, int deviceId)
        {
            var user = await _dbContext.ZKTecoUsers.FindAsync(userId);
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);

            if (user == null || device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;

            var apiUser = new ZKTecoApiUserDto
            {
                EnrollNumber = user.EnrollNumber,
                Name = user.Name,
                Password = user.Password,
                CardNumber = user.CardNumber,
                Privilege = user.Privilege,
                Enabled = user.Enabled
            };

            var success = await _apiClient.AddUserToDeviceAsync(device.IpAddress, apiUser, port);

            if (success)
            {
                user.LastSyncTime = DateTime.Now;
                user.DeviceId = deviceId;
                user.DeviceIp = device.IpAddress;
                await _dbContext.SaveChangesAsync();
            }

            return success;
        }

        public async Task<bool> SyncUserToAllDevicesAsync(int userId)
        {
            var user = await _dbContext.ZKTecoUsers.FindAsync(userId);
            if (user == null) return false;

            var activeDevices = await _deviceService.GetActiveDevicesAsync();
            var successCount = 0;

            foreach (var device in activeDevices)
            {
                if (await SyncUserToDeviceAsync(userId, device.Id))
                    successCount++;
            }

            _logger.LogInformation($"Synced user {user.Name} to {successCount}/{activeDevices.Count} devices");
            return successCount > 0;
        }

        public async Task<bool> SyncAllUsersToDeviceAsync(int deviceId)
        {
            var users = await GetUsersByDeviceIdAsync(deviceId);
            var successCount = 0;

            foreach (var user in users)
            {
                if (await SyncUserToDeviceAsync(user.Id, deviceId))
                    successCount++;
            }

            _logger.LogInformation($"Synced {successCount}/{users.Count} users to device {deviceId}");
            return successCount > 0;
        }

        public async Task<bool> SyncUsersFromDeviceToDbAsync(int deviceId)
        {
            var deviceUsers = await GetUsersFromDeviceAsync(deviceId);
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            foreach (var apiUser in deviceUsers)
            {
                var existingUser = await _dbContext.ZKTecoUsers
                    .FirstOrDefaultAsync(u => u.EnrollNumber == apiUser.EnrollNumber && u.DeviceId == deviceId);

                if (existingUser == null)
                {
                    // Create new
                    var newUser = new ZKTecoUser
                    {
                        EnrollNumber = apiUser.EnrollNumber,
                        Name = apiUser.Name,
                        Password = apiUser.Password,
                        CardNumber = apiUser.CardNumber,
                        Privilege = apiUser.Privilege,
                        Enabled = apiUser.Enabled,
                        DeviceId = deviceId,
                        DeviceIp = device.IpAddress,
                        LastSyncTime = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _dbContext.ZKTecoUsers.Add(newUser);
                }
                else
                {
                    // Update existing
                    existingUser.Name = apiUser.Name;
                    existingUser.Password = apiUser.Password;
                    existingUser.CardNumber = apiUser.CardNumber;
                    existingUser.Privilege = apiUser.Privilege;
                    existingUser.Enabled = apiUser.Enabled;
                    existingUser.LastSyncTime = DateTime.Now;
                    existingUser.UpdatedAt = DateTime.Now;
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Synced {deviceUsers.Count} users from device {deviceId} to database");
            return true;
        }

        public async Task<bool> AddUserToDeviceAsync(int deviceId, ZKTecoApiUserDto user)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.AddUserToDeviceAsync(device.IpAddress, user, port);
        }

        public async Task<bool> UpdateUserOnDeviceAsync(int deviceId, string enrollNumber, ZKTecoApiUserDto user)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.UpdateUserOnDeviceAsync(device.IpAddress, enrollNumber, user, port);
        }

        public async Task<bool> DeleteUserFromDeviceAsync(int deviceId, string enrollNumber)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.DeleteUserFromDeviceAsync(device.IpAddress, enrollNumber, port);
        }

        public async Task<bool> DeleteUserFromAllDevicesAsync(string enrollNumber)
        {
            var activeDevices = await _deviceService.GetActiveDevicesAsync();
            var successCount = 0;

            foreach (var device in activeDevices)
            {
                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                if (await _apiClient.DeleteUserFromDeviceAsync(device.IpAddress, enrollNumber, port))
                    successCount++;
            }

            _logger.LogInformation($"Deleted user {enrollNumber} from {successCount}/{activeDevices.Count} devices");
            return successCount > 0;
        }

        public async Task<bool> ClearAllUsersFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.ClearAllUsersFromDeviceAsync(device.IpAddress, port);
        }
    }
}

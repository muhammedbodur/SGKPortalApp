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
    public class ZKTecoDeviceService : IZKTecoDeviceService
    {
        private readonly SGKDbContext _dbContext;
        private readonly IZKTecoApiClient _apiClient;
        private readonly ILogger<ZKTecoDeviceService> _logger;

        public ZKTecoDeviceService(
            SGKDbContext dbContext,
            IZKTecoApiClient apiClient,
            ILogger<ZKTecoDeviceService> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<ZKTecoDevice>> GetAllDevicesAsync()
        {
            return await _dbContext.ZKTecoDevices
                .OrderBy(d => d.DeviceName)
                .ToListAsync();
        }

        public async Task<ZKTecoDevice?> GetDeviceByIdAsync(int id)
        {
            return await _dbContext.ZKTecoDevices.FindAsync(id);
        }

        public async Task<ZKTecoDevice?> GetDeviceByIpAsync(string ipAddress)
        {
            return await _dbContext.ZKTecoDevices
                .FirstOrDefaultAsync(d => d.IpAddress == ipAddress);
        }

        public async Task<ZKTecoDevice> CreateDeviceAsync(ZKTecoDevice device)
        {
            device.CreatedAt = DateTime.Now;
            device.UpdatedAt = DateTime.Now;

            _dbContext.ZKTecoDevices.Add(device);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Device created: {device.DeviceName} ({device.IpAddress})");
            return device;
        }

        public async Task<ZKTecoDevice> UpdateDeviceAsync(ZKTecoDevice device)
        {
            device.UpdatedAt = DateTime.Now;

            _dbContext.ZKTecoDevices.Update(device);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Device updated: {device.DeviceName} ({device.IpAddress})");
            return device;
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var device = await GetDeviceByIdAsync(id);
            if (device == null) return false;

            _dbContext.ZKTecoDevices.Remove(device);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Device deleted: {device.DeviceName}");
            return true;
        }

        public async Task<List<ZKTecoDevice>> GetActiveDevicesAsync()
        {
            return await _dbContext.ZKTecoDevices
                .Where(d => d.IsActive)
                .OrderBy(d => d.DeviceName)
                .ToListAsync();
        }

        // ========== Device Operations (API Calls) ==========

        public async Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return null;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            var status = await _apiClient.GetDeviceStatusAsync(device.IpAddress, port);

            if (status != null)
            {
                // Update device info
                device.LastHealthCheckTime = DateTime.Now;
                device.LastHealthCheckSuccess = true;
                device.HealthCheckCount++;
                device.LastHealthCheckStatus = "Başarılı";
                await UpdateDeviceAsync(device);
            }

            return status;
        }

        public async Task<DeviceStatusDto?> GetDeviceStatusByIpAsync(string deviceIp, int port = 4370)
        {
            return await _apiClient.GetDeviceStatusAsync(deviceIp, port);
        }

        public async Task<bool> TestConnectionAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            var success = await _apiClient.TestConnectionAsync(device.IpAddress, port);

            device.LastHealthCheckTime = DateTime.Now;
            device.LastHealthCheckSuccess = success;
            device.HealthCheckCount++;
            device.LastHealthCheckStatus = success ? "Bağlantı başarılı" : "Bağlantı başarısız";
            await UpdateDeviceAsync(device);

            return success;
        }

        public async Task<bool> RestartDeviceAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.RestartDeviceAsync(device.IpAddress, port);
        }

        public async Task<bool> EnableDeviceAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.EnableDeviceAsync(device.IpAddress, port);
        }

        public async Task<bool> DisableDeviceAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.DisableDeviceAsync(device.IpAddress, port);
        }

        public async Task<DeviceTimeDto?> GetDeviceTimeAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return null;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetDeviceTimeAsync(device.IpAddress, port);
        }

        public async Task<bool> SynchronizeDeviceTimeAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.SetDeviceTimeAsync(device.IpAddress, DateTime.Now, port);
        }

        // ========== Realtime Monitoring ==========

        public async Task<bool> StartMonitoringAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.StartRealtimeMonitoringAsync(device.IpAddress, port);
        }

        public async Task<bool> StopMonitoringAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.StopRealtimeMonitoringAsync(device.IpAddress, port);
        }

        public async Task<bool> GetMonitoringStatusAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetRealtimeMonitoringStatusAsync(device.IpAddress, port);
        }
    }
}

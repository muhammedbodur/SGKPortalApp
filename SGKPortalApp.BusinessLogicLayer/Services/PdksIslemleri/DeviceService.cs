using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IZKTecoApiClient _apiClient;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(
            IUnitOfWork unitOfWork,
            IZKTecoApiClient apiClient,
            ILogger<DeviceService> logger)
        {
            _unitOfWork = unitOfWork;
            _apiClient = apiClient;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi).OrderBy(d => d.DeviceName).ToList();
        }

        public async Task<Device?> GetDeviceByIdAsync(int id)
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();
            var device = await deviceRepo.GetByIdAsync(id);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device?> GetDeviceByIpAsync(string ipAddress)
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();
            var device = await deviceRepo.GetDeviceByIpAsync(ipAddress);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();

            var existing = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
            if (existing != null && !existing.SilindiMi)
            {
                throw new InvalidOperationException($"Bu IP adresine ({device.IpAddress}) sahip bir cihaz zaten mevcut.");
            }

            device.EklenmeTarihi = DateTime.Now;
            device.SilindiMi = false;

            await deviceRepo.AddAsync(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device created: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return device;
        }

        public async Task<Device> UpdateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();

            var existing = await deviceRepo.GetByIdAsync(device.DeviceId);
            if (existing == null || existing.SilindiMi)
            {
                throw new InvalidOperationException($"Device with ID {device.DeviceId} not found.");
            }

            if (existing.IpAddress != device.IpAddress)
            {
                var deviceWithSameIp = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
                if (deviceWithSameIp != null && deviceWithSameIp.DeviceId != device.DeviceId && !deviceWithSameIp.SilindiMi)
                {
                    throw new InvalidOperationException($"Bu IP adresine ({device.IpAddress}) sahip başka bir cihaz mevcut.");
                }
            }

            deviceRepo.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device updated: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return device;
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();

            var device = await deviceRepo.GetByIdAsync(id);
            if (device == null || device.SilindiMi)
            {
                return false;
            }

            device.SilindiMi = true;
            device.SilinmeTarihi = DateTime.Now;

            deviceRepo.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device soft-deleted: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return true;
        }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            var deviceRepo = _unitOfWork.GetRepository<DataAccessLayer.Repositories.Interfaces.PdksIslemleri.IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi && d.IsActive).OrderBy(d => d.DeviceName).ToList();
        }

        // ========== Device Operations (API Calls) ==========

        public async Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId)
        {
            var device = await GetDeviceByIdAsync(deviceId);
            if (device == null) return null;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetDeviceStatusAsync(device.IpAddress, port);
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

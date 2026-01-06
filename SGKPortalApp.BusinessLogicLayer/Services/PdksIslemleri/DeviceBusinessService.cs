using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Services.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class DeviceBusinessService : IDeviceBusinessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IZKTecoApiClient _apiClient;
        private readonly ILogger<DeviceBusinessService> _logger;

        public DeviceBusinessService(
            IUnitOfWork unitOfWork,
            IZKTecoApiClient apiClient,
            ILogger<DeviceBusinessService> logger)
        {
            _unitOfWork = unitOfWork;
            _apiClient = apiClient;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi).OrderBy(d => d.DeviceName).ToList();
        }

        public async Task<Device?> GetDeviceByIdAsync(int id)
        {
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();
            var device = await deviceRepo.GetByIdAsync(id);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device?> GetDeviceByIpAsync(string ipAddress)
        {
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();
            var device = await deviceRepo.GetDeviceByIpAsync(ipAddress);
            return device?.SilindiMi == false ? device : null;
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();

            // Check if device with same IP already exists
            var existing = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
            if (existing != null && !existing.SilindiMi)
            {
                throw new InvalidOperationException($"Bu IP adresine ({device.IpAddress}) sahip bir cihaz zaten mevcut.");
            }

            device.EklenmeTarihi = DateTime.Now;
            device.SilindiMi = false;

            await deviceRepo.CreateAsync(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Device created: {DeviceName} ({IpAddress})", device.DeviceName, device.IpAddress);
            return device;
        }

        public async Task<Device> UpdateDeviceAsync(Device device)
        {
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();

            var existing = await deviceRepo.GetByIdAsync(device.Id);
            if (existing == null || existing.SilindiMi)
            {
                throw new InvalidOperationException($"Device with ID {device.Id} not found.");
            }

            // Check if IP address is changing to an already used IP
            if (existing.IpAddress != device.IpAddress)
            {
                var deviceWithSameIp = await deviceRepo.GetDeviceByIpAsync(device.IpAddress);
                if (deviceWithSameIp != null && deviceWithSameIp.Id != device.Id && !deviceWithSameIp.SilindiMi)
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
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();

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
            var deviceRepo = _unitOfWork.Repository<Device, IDeviceRepository>();
            var devices = await deviceRepo.GetAllAsync();
            return devices.Where(d => !d.SilindiMi && d.IsActive).OrderBy(d => d.DeviceName).ToList();
        }

        // ========== Device Operations (API Calls) ==========

        public async Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetDeviceStatusAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device status for device {DeviceId}", deviceId);
                return null;
            }
        }

        public async Task<bool> TestConnectionAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                var success = await _apiClient.TestConnectionAsync(device.IpAddress, port);

                // Update health check information
                device.LastHealthCheckTime = DateTime.Now;
                device.LastHealthCheckSuccess = success;
                device.HealthCheckCount++;
                device.LastHealthCheckStatus = success ? "Bağlantı başarılı" : "Bağlantı başarısız";

                await UpdateDeviceAsync(device);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> RestartDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.RestartDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> EnableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.EnableDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> DisableDeviceAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.DisableDeviceAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<DeviceTimeDto?> GetDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return null;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetDeviceTimeAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device time for device {DeviceId}", deviceId);
                return null;
            }
        }

        public async Task<bool> SynchronizeDeviceTimeAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.SetDeviceTimeAsync(device.IpAddress, DateTime.Now, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing device time for device {DeviceId}", deviceId);
                return false;
            }
        }

        // ========== Realtime Monitoring ==========

        public async Task<bool> StartMonitoringAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.StartRealtimeMonitoringAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting monitoring for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> StopMonitoringAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.StopRealtimeMonitoringAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping monitoring for device {DeviceId}", deviceId);
                return false;
            }
        }

        public async Task<bool> GetMonitoringStatusAsync(int deviceId)
        {
            try
            {
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null) return false;

                var port = int.TryParse(device.Port, out var p) ? p : 4370;
                return await _apiClient.GetRealtimeMonitoringStatusAsync(device.IpAddress, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monitoring status for device {DeviceId}", deviceId);
                return false;
            }
        }
    }
}

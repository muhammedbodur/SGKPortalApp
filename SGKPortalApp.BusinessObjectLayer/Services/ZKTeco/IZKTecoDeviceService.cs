using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    /// <summary>
    /// ZKTeco Device yönetim servisi
    /// Database (ZKTecoDevices) + ZKTecoApi kombinasyonu
    /// </summary>
    public interface IZKTecoDeviceService
    {
        // ========== Database Operations (CRUD) ==========

        Task<List<ZKTecoDevice>> GetAllDevicesAsync();
        Task<ZKTecoDevice?> GetDeviceByIdAsync(int id);
        Task<ZKTecoDevice?> GetDeviceByIpAsync(string ipAddress);
        Task<ZKTecoDevice> CreateDeviceAsync(ZKTecoDevice device);
        Task<ZKTecoDevice> UpdateDeviceAsync(ZKTecoDevice device);
        Task<bool> DeleteDeviceAsync(int id);
        Task<List<ZKTecoDevice>> GetActiveDevicesAsync();

        // ========== Device Operations (API Calls) ==========

        Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId);
        Task<DeviceStatusDto?> GetDeviceStatusByIpAsync(string deviceIp, int port = 4370);
        Task<bool> TestConnectionAsync(int deviceId);
        Task<bool> RestartDeviceAsync(int deviceId);
        Task<bool> EnableDeviceAsync(int deviceId);
        Task<bool> DisableDeviceAsync(int deviceId);
        Task<DeviceTimeDto?> GetDeviceTimeAsync(int deviceId);
        Task<bool> SynchronizeDeviceTimeAsync(int deviceId);

        // ========== Realtime Monitoring ==========

        Task<bool> StartMonitoringAsync(int deviceId);
        Task<bool> StopMonitoringAsync(int deviceId);
        Task<bool> GetMonitoringStatusAsync(int deviceId);
    }
}

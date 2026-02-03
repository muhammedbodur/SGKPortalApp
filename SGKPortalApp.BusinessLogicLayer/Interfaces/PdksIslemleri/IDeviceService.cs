using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
{
    /// <summary>
    /// ZKTeco Device yönetim servisi
    /// Database (Devices) + ZKTecoApi kombinasyonu
    /// </summary>
    public interface IDeviceService
    {
        // ========== Database Operations (CRUD) ==========

        Task<List<Device>> GetAllDevicesAsync();
        Task<Device?> GetDeviceByIdAsync(int id);
        Task<Device?> GetDeviceByIpAsync(string ipAddress);
        Task<Device> CreateDeviceAsync(Device device);
        Task<Device> UpdateDeviceAsync(Device device);
        Task<bool> DeleteDeviceAsync(int id);
        Task<List<Device>> GetActiveDevicesAsync();

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

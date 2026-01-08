using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
{
    /// <summary>
    /// ZKTeco Device business logic service
    /// Handles both database operations and ZKTeco API calls
    /// </summary>
    public interface IDeviceBusinessService
    {
        // ========== Database Operations (CRUD) ==========
        Task<List<Device>> GetAllDevicesAsync();
        Task<Device?> GetDeviceByIdAsync(int id);
        Task<Device?> GetDeviceByIpAsync(string ipAddress);
        Task<Device> CreateDeviceAsync(Device device);
        Task<Device> UpdateDeviceAsync(Device device);
        Task<bool> DeleteDeviceAsync(int id);
        Task<List<Device>> GetActiveDevicesAsync();

        // ========== Device Control Operations ==========
        Task<DeviceStatusDto?> GetDeviceStatusAsync(int deviceId);
        Task<bool> TestConnectionAsync(int deviceId);
        Task<bool> RestartDeviceAsync(int deviceId);
        Task<bool> PowerOffDeviceAsync(int deviceId);
        Task<bool> EnableDeviceAsync(int deviceId);
        Task<bool> DisableDeviceAsync(int deviceId);
        Task<DeviceTimeDto?> GetDeviceTimeAsync(int deviceId);
        Task<bool> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null);
        Task<bool> SynchronizeDeviceTimeAsync(int deviceId);

        // ========== User Management (Device Users) ==========
        Task<List<ApiUserDto>> GetDeviceUsersAsync(int deviceId);
        Task<ApiUserDto?> GetDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ApiUserDto?> GetDeviceUserByCardAsync(int deviceId, long cardNumber);
        Task<CardSearchResponse> SearchUserByCardAsync(CardSearchRequest request);
        Task<CardSearchResponse> SearchUserByCardOnAllDevicesAsync(long cardNumber);
        Task<bool> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false);
        Task<bool> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false);
        Task<bool> DeleteDeviceUserAsync(int deviceId, string enrollNumber);
        Task<bool> ClearAllDeviceUsersAsync(int deviceId);
        Task<int> GetDeviceUserCountAsync(int deviceId);
        Task<bool> RemoveCardFromUserAsync(int deviceId, string enrollNumber);

        // ========== Attendance Management ==========
        Task<List<AttendanceLogDto>> GetAttendanceLogsAsync(int deviceId);
        Task<bool> ClearAttendanceLogsAsync(int deviceId);
        Task<int> GetAttendanceLogCountAsync(int deviceId);

        // ========== Realtime Monitoring ==========
        Task<bool> StartRealtimeMonitoringAsync(int deviceId);
        Task<bool> StopRealtimeMonitoringAsync(int deviceId);
        Task<bool> GetMonitoringStatusAsync(int deviceId);
    }
}

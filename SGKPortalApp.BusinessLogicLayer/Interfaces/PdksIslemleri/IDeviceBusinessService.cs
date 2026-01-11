using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
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
        Task<ApiResponseDto<List<DeviceResponseDto>>> GetAllDevicesAsync();
        Task<ApiResponseDto<DeviceResponseDto>> GetDeviceByIdAsync(int id);
        Task<ApiResponseDto<Device>> GetDeviceByIpAsync(string ipAddress);
        Task<ApiResponseDto<DeviceResponseDto>> CreateDeviceAsync(Device device);
        Task<ApiResponseDto<DeviceResponseDto>> UpdateDeviceAsync(Device device);
        Task<ApiResponseDto<bool>> DeleteDeviceAsync(int id);
        Task<ApiResponseDto<List<DeviceResponseDto>>> GetActiveDevicesAsync();

        // ========== Device Control Operations ==========
        Task<ApiResponseDto<DeviceStatusDto>> GetDeviceStatusAsync(int deviceId);
        Task<ApiResponseDto<bool>> TestConnectionAsync(int deviceId);
        Task<ApiResponseDto<bool>> RestartDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> PowerOffDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> EnableDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> DisableDeviceAsync(int deviceId);
        Task<ApiResponseDto<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId);
        Task<ApiResponseDto<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null);
        Task<ApiResponseDto<bool>> SynchronizeDeviceTimeAsync(int deviceId);

        // ========== User Management (Device Users) ==========
        Task<List<ApiUserDto>> GetDeviceUsersAsync(int deviceId);
        Task<ApiUserDto?> GetDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ApiUserDto?> GetDeviceUserByCardAsync(int deviceId, long cardNumber);
        Task<DeviceUserMatch> GetDeviceUserWithMismatchInfoAsync(int deviceId, string enrollNumber);
        Task<DeviceUserMatch> GetDeviceUserByCardWithMismatchInfoAsync(int deviceId, long cardNumber);
        Task<List<DeviceUserMatch>> GetAllDeviceUsersWithMismatchInfoAsync(int deviceId);
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

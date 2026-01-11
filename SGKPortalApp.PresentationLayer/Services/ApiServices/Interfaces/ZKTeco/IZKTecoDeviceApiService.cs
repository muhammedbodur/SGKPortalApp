using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface IZKTecoDeviceApiService
    {
        // ========== Device CRUD ==========
        Task<ServiceResult<List<DeviceResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<DeviceResponseDto>>> GetActiveAsync();
        Task<ServiceResult<DeviceResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<DeviceResponseDto>> CreateAsync(DeviceResponseDto device);
        Task<ServiceResult<DeviceResponseDto>> UpdateAsync(int id, DeviceResponseDto device);
        Task<ServiceResult<bool>> DeleteAsync(int id);

        // ========== Device Control ==========
        Task<ServiceResult<DeviceStatusDto>> GetStatusAsync(int deviceId);
        Task<ServiceResult<bool>> TestConnectionAsync(int deviceId);
        Task<ServiceResult<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId);
        Task<ServiceResult<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null);
        Task<ServiceResult<bool>> SynchronizeDeviceTimeAsync(int deviceId);
        Task<ServiceResult<bool>> EnableDeviceAsync(int deviceId);
        Task<ServiceResult<bool>> DisableDeviceAsync(int deviceId);
        Task<ServiceResult<bool>> RestartDeviceAsync(int deviceId);
        Task<ServiceResult<bool>> PowerOffDeviceAsync(int deviceId);

        // ========== User Management ==========
        Task<ServiceResult<List<ApiUserDto>>> GetDeviceUsersAsync(int deviceId);
        Task<ServiceResult<List<DeviceUserMatch>>> GetDeviceUsersWithMismatchesAsync(int deviceId);
        Task<ServiceResult<ApiUserDto>> GetDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ServiceResult<ApiUserDto>> GetDeviceUserByCardAsync(int deviceId, long cardNumber);
        Task<ServiceResult<bool>> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false);
        Task<ServiceResult<bool>> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false);
        Task<ServiceResult<bool>> DeleteDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ServiceResult<bool>> ClearAllDeviceUsersAsync(int deviceId);
        Task<ServiceResult<int>> GetDeviceUserCountAsync(int deviceId);
        Task<ServiceResult<bool>> RemoveCardFromUserAsync(int deviceId, string enrollNumber);

        // ========== Attendance Management ==========
        Task<ServiceResult<List<AttendanceLogDto>>> GetAttendanceLogsAsync(int deviceId);
        Task<ServiceResult<bool>> ClearAttendanceLogsAsync(int deviceId);
        Task<ServiceResult<int>> GetAttendanceLogCountAsync(int deviceId);

        // ========== Realtime Monitoring ==========
        Task<ServiceResult<bool>> StartRealtimeMonitoringAsync(int deviceId);
        Task<ServiceResult<bool>> StopRealtimeMonitoringAsync(int deviceId);
    }
}

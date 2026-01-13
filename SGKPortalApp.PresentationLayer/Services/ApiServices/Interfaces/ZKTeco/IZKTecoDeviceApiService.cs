using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface IZKTecoDeviceApiService
    {
        // ========== Device CRUD ==========
        Task<ApiResponseDto<List<DeviceResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<DeviceResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<DeviceResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<DeviceResponseDto>> CreateAsync(DeviceResponseDto device);
        Task<ApiResponseDto<DeviceResponseDto>> UpdateAsync(int id, DeviceResponseDto device);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        // ========== Device Control ==========
        Task<ApiResponseDto<DeviceStatusDto>> GetStatusAsync(int deviceId);
        Task<ApiResponseDto<bool>> TestConnectionAsync(int deviceId);
        Task<ApiResponseDto<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId);
        Task<ApiResponseDto<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null);
        Task<ApiResponseDto<bool>> SynchronizeDeviceTimeAsync(int deviceId);
        Task<ApiResponseDto<bool>> EnableDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> DisableDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> RestartDeviceAsync(int deviceId);
        Task<ApiResponseDto<bool>> PowerOffDeviceAsync(int deviceId);

        // ========== User Management ==========
        Task<ApiResponseDto<List<ApiUserDto>>> GetDeviceUsersAsync(int deviceId);
        Task<ApiResponseDto<List<DeviceUserMatch>>> GetDeviceUsersWithMismatchesAsync(int deviceId);
        Task<ApiResponseDto<ApiUserDto>> GetDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ApiResponseDto<ApiUserDto>> GetDeviceUserByCardAsync(int deviceId, long cardNumber);
        Task<ApiResponseDto<bool>> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false);
        Task<ApiResponseDto<bool>> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false);
        Task<ApiResponseDto<bool>> DeleteDeviceUserAsync(int deviceId, string enrollNumber);
        Task<ApiResponseDto<bool>> ClearAllDeviceUsersAsync(int deviceId);
        Task<ApiResponseDto<int>> GetDeviceUserCountAsync(int deviceId);
        Task<ApiResponseDto<bool>> RemoveCardFromUserAsync(int deviceId, string enrollNumber);

        // ========== Attendance Management ==========
        Task<ApiResponseDto<List<AttendanceLogDto>>> GetAttendanceLogsAsync(int deviceId);
        Task<ApiResponseDto<bool>> ClearAttendanceLogsAsync(int deviceId);
        Task<ApiResponseDto<int>> GetAttendanceLogCountAsync(int deviceId);

        // ========== Realtime Monitoring ==========
        Task<ApiResponseDto<bool>> StartRealtimeMonitoringAsync(int deviceId);
        Task<ApiResponseDto<bool>> StopRealtimeMonitoringAsync(int deviceId);
    }
}

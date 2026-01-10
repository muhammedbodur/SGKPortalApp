using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using System.Net.Http.Json;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Concrete.ZKTeco
{
    public class ZKTecoDeviceApiService : IZKTecoDeviceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ZKTecoDeviceApiService> _logger;

        public ZKTecoDeviceApiService(HttpClient httpClient, ILogger<ZKTecoDeviceApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ========== Device CRUD ==========

        public async Task<ServiceResult<List<DeviceResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DeviceResponseDto>>.Fail("Cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DeviceResponseDto>>>();
                return result ?? ServiceResult<List<DeviceResponseDto>>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync failed");
                return ServiceResult<List<DeviceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DeviceResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device/active");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DeviceResponseDto>>.Fail("Aktif cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<DeviceResponseDto>>>();
                return result ?? ServiceResult<List<DeviceResponseDto>>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync failed");
                return ServiceResult<List<DeviceResponseDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceResponseDto>>();
                return result ?? ServiceResult<DeviceResponseDto>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed");
                return ServiceResult<DeviceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceStatusDto>> GetStatusAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/status");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceStatusDto>.Fail("Cihaz durumu alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<DeviceStatusDto>();
                return ServiceResult<DeviceStatusDto>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatusAsync failed");
                return ServiceResult<DeviceStatusDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> CreateAsync(Device device)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Device", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz oluşturulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<Device>();
                if (result == null)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz oluşturuldu ancak veri okunamadı");
                }
                
                var dto = new DeviceResponseDto
                {
                    DeviceId = result.DeviceId,
                    DeviceName = result.DeviceName,
                    IpAddress = result.IpAddress,
                    Port = result.Port ?? "4370",
                    DeviceCode = result.DeviceCode,
                    DeviceInfo = result.DeviceInfo,
                    IsActive = result.IsActive,
                    HizmetBinasiId = result.HizmetBinasiId,
                    LastSyncTime = result.LastSyncTime,
                    SyncCount = result.SyncCount,
                    LastSyncSuccess = result.LastSyncSuccess,
                    LastSyncStatus = result.LastSyncStatus,
                    LastHealthCheckTime = result.LastHealthCheckTime,
                    HealthCheckCount = result.HealthCheckCount,
                    LastHealthCheckSuccess = result.LastHealthCheckSuccess,
                    LastHealthCheckStatus = result.LastHealthCheckStatus
                };
                
                return ServiceResult<DeviceResponseDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed");
                return ServiceResult<DeviceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> UpdateAsync(Device device)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Device/{device.DeviceId}", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz güncellenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<Device>();
                if (result == null)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz güncellendi ancak veri okunamadı");
                }
                
                var dto = new DeviceResponseDto
                {
                    DeviceId = result.DeviceId,
                    DeviceName = result.DeviceName,
                    IpAddress = result.IpAddress,
                    Port = result.Port ?? "4370",
                    DeviceCode = result.DeviceCode,
                    DeviceInfo = result.DeviceInfo,
                    IsActive = result.IsActive,
                    HizmetBinasiId = result.HizmetBinasiId,
                    LastSyncTime = result.LastSyncTime,
                    SyncCount = result.SyncCount,
                    LastSyncSuccess = result.LastSyncSuccess,
                    LastSyncStatus = result.LastSyncStatus,
                    LastHealthCheckTime = result.LastHealthCheckTime,
                    HealthCheckCount = result.HealthCheckCount,
                    LastHealthCheckSuccess = result.LastHealthCheckSuccess,
                    LastHealthCheckStatus = result.LastHealthCheckStatus
                };
                
                return ServiceResult<DeviceResponseDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed");
                return ServiceResult<DeviceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Device/{id}");
                return ServiceResult<bool>.Ok(response.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ========== Device Control ==========

        public async Task<ServiceResult<bool>> TestConnectionAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/test", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TestConnectionAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/time");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceTimeDto>.Fail("Cihaz zamanı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<DeviceTimeDto>();
                return ServiceResult<DeviceTimeDto>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceTimeAsync failed");
                return ServiceResult<DeviceTimeDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Device/{deviceId}/time", dateTime);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetDeviceTimeAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> SynchronizeDeviceTimeAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/time/sync", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SynchronizeDeviceTimeAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> EnableDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/enable", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnableDeviceAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DisableDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/disable", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisableDeviceAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RestartDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/restart", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RestartDeviceAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> PowerOffDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/poweroff", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PowerOffDeviceAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ========== User Management ==========

        public async Task<ServiceResult<List<ApiUserDto>>> GetDeviceUsersAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/users");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<ApiUserDto>>.Fail("Kullanıcılar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<List<ApiUserDto>>();
                return ServiceResult<List<ApiUserDto>>.Ok(result ?? new List<ApiUserDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUsersAsync failed");
                return ServiceResult<List<ApiUserDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ApiUserDto>> GetDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/users/{enrollNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<ApiUserDto>.Fail("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiUserDto>();
                return ServiceResult<ApiUserDto>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserAsync failed");
                return ServiceResult<ApiUserDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ApiUserDto>> GetDeviceUserByCardAsync(int deviceId, long cardNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/users/card/{cardNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<ApiUserDto>.Fail("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiUserDto>();
                return ServiceResult<ApiUserDto>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserByCardAsync failed");
                return ServiceResult<ApiUserDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Device/{deviceId}/users?force={force}", request);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateDeviceUserAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Device/{deviceId}/users/{enrollNumber}?force={force}", request);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateDeviceUserAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Device/{deviceId}/users/{enrollNumber}");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteDeviceUserAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ClearAllDeviceUsersAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Device/{deviceId}/users");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearAllDeviceUsersAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetDeviceUserCountAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/users/count");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<int>.Ok((int)(result?.Count ?? 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserCountAsync failed");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RemoveCardFromUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Device/{deviceId}/users/{enrollNumber}/card");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveCardFromUserAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        // ========== Attendance Management ==========

        public async Task<ServiceResult<List<AttendanceLogDto>>> GetAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/attendance");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<AttendanceLogDto>>.Fail("Kayıtlar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<List<AttendanceLogDto>>();
                return ServiceResult<List<AttendanceLogDto>>.Ok(result ?? new List<AttendanceLogDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceLogsAsync failed");
                return ServiceResult<List<AttendanceLogDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ClearAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Device/{deviceId}/attendance");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearAttendanceLogsAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetAttendanceLogCountAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Device/{deviceId}/attendance/count");
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<int>.Ok((int)(result?.Count ?? 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceLogCountAsync failed");
                return ServiceResult<int>.Fail($"Hata: {ex.Message}");
            }
        }

        // ========== Realtime Monitoring ==========

        public async Task<ServiceResult<bool>> StartRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/monitoring/start", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StartRealtimeMonitoringAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> StopRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/Device/{deviceId}/monitoring/stop", null);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return ServiceResult<bool>.Ok(result?.Success == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StopRealtimeMonitoringAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

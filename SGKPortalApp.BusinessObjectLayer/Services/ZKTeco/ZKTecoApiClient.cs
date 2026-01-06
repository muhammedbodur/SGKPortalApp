using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    /// <summary>
    /// ZKTecoApi HTTP client implementation
    /// ZKTecoApi REST API'sini çağırır
    /// </summary>
    public class ZKTecoApiClient : IZKTecoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ZKTecoApiClient> _logger;
        private readonly string _baseUrl;

        public ZKTecoApiClient(HttpClient httpClient, ILogger<ZKTecoApiClient> logger, string baseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));

            // Set default timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        // ========== Device Operations ==========

        public async Task<bool> TestConnectionAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/connect?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Connection test failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Connection test error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<DeviceStatusDto?> GetDeviceStatusAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/status?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get device status failed: {deviceIp}:{port} - {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DeviceStatusDto>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get device status error: {deviceIp}:{port}");
                return null;
            }
        }

        public async Task<DeviceTimeDto?> GetDeviceTimeAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/time?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get device time failed: {deviceIp}:{port} - {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DeviceTimeDto>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get device time error: {deviceIp}:{port}");
                return null;
            }
        }

        public async Task<bool> SetDeviceTimeAsync(string deviceIp, DateTime newTime, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/time?port={port}",
                    newTime.ToString("o")); // ISO 8601 format

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Set device time failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Set device time error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> EnableDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/enable?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Enable device failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Enable device error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> DisableDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/disable?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Disable device failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Disable device error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> RestartDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/restart?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Restart device failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Restart device error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> PowerOffDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/device/{deviceIp}/poweroff?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Power off device failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Power off device error: {deviceIp}:{port}");
                return false;
            }
        }

        // ========== User Operations ==========

        public async Task<List<ZKTecoApiUserDto>> GetAllUsersFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/users/{deviceIp}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get all users failed: {deviceIp}:{port} - {response.StatusCode}");
                    return new List<ZKTecoApiUserDto>();
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ZKTecoApiUserDto>>>();
                return result?.Data ?? new List<ZKTecoApiUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get all users error: {deviceIp}:{port}");
                return new List<ZKTecoApiUserDto>();
            }
        }

        public async Task<ZKTecoApiUserDto?> GetUserFromDeviceAsync(string deviceIp, string enrollNumber, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/users/{deviceIp}/{enrollNumber}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get user failed: {deviceIp}:{port} - {enrollNumber} - {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ZKTecoApiUserDto>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get user error: {deviceIp}:{port} - {enrollNumber}");
                return null;
            }
        }

        public async Task<ZKTecoApiUserDto?> GetUserByCardNumberAsync(string deviceIp, long cardNumber, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/users/{deviceIp}/card/{cardNumber}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get user by card failed: {deviceIp}:{port} - Card: {cardNumber} - {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ZKTecoApiUserDto>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get user by card error: {deviceIp}:{port} - Card: {cardNumber}");
                return null;
            }
        }

        public async Task<bool> AddUserToDeviceAsync(string deviceIp, ZKTecoApiUserDto user, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"{_baseUrl}/api/users/{deviceIp}?port={port}",
                    user);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Add user failed: {deviceIp}:{port} - {user.EnrollNumber} - {response.StatusCode} - {errorContent}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Add user error: {deviceIp}:{port} - {user.EnrollNumber}");
                return false;
            }
        }

        public async Task<bool> UpdateUserOnDeviceAsync(string deviceIp, string enrollNumber, ZKTecoApiUserDto user, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_baseUrl}/api/users/{deviceIp}/{enrollNumber}?port={port}",
                    user);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Update user failed: {deviceIp}:{port} - {enrollNumber} - {response.StatusCode} - {errorContent}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Update user error: {deviceIp}:{port} - {enrollNumber}");
                return false;
            }
        }

        public async Task<bool> DeleteUserFromDeviceAsync(string deviceIp, string enrollNumber, int port = 4370)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{_baseUrl}/api/users/{deviceIp}/{enrollNumber}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Delete user failed: {deviceIp}:{port} - {enrollNumber} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete user error: {deviceIp}:{port} - {enrollNumber}");
                return false;
            }
        }

        public async Task<bool> ClearAllUsersFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{_baseUrl}/api/users/{deviceIp}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Clear all users failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Clear all users error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<int> GetUserCountFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/users/{deviceIp}/count?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get user count failed: {deviceIp}:{port} - {response.StatusCode}");
                    return 0;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserCountData>>();
                return result?.Data?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get user count error: {deviceIp}:{port}");
                return 0;
            }
        }

        // ========== Attendance Operations ==========

        public async Task<List<AttendanceRecordDto>> GetAttendanceLogsFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/attendance/{deviceIp}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get attendance logs failed: {deviceIp}:{port} - {response.StatusCode}");
                    return new List<AttendanceRecordDto>();
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<AttendanceRecordDto>>>();
                return result?.Data ?? new List<AttendanceRecordDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get attendance logs error: {deviceIp}:{port}");
                return new List<AttendanceRecordDto>();
            }
        }

        public async Task<int> GetAttendanceLogCountFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/attendance/{deviceIp}/count?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get attendance count failed: {deviceIp}:{port} - {response.StatusCode}");
                    return 0;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AttendanceCountData>>();
                return result?.Data?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get attendance count error: {deviceIp}:{port}");
                return 0;
            }
        }

        public async Task<bool> ClearAttendanceLogsFromDeviceAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{_baseUrl}/api/attendance/{deviceIp}?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Clear attendance logs failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Clear attendance logs error: {deviceIp}:{port}");
                return false;
            }
        }

        // ========== Realtime Operations ==========

        public async Task<bool> StartRealtimeMonitoringAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/realtime/{deviceIp}/start?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Start realtime monitoring failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Start realtime monitoring error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> StopRealtimeMonitoringAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/realtime/{deviceIp}/stop?port={port}",
                    null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Stop realtime monitoring failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Stop realtime monitoring error: {deviceIp}:{port}");
                return false;
            }
        }

        public async Task<bool> GetRealtimeMonitoringStatusAsync(string deviceIp, int port = 4370)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/realtime/{deviceIp}/status?port={port}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Get realtime monitoring status failed: {deviceIp}:{port} - {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<RealtimeStatusData>>();
                return result?.Data?.IsMonitoring ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get realtime monitoring status error: {deviceIp}:{port}");
                return false;
            }
        }

        // ========== Helper Classes ==========

        private class ApiResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }

        private class UserCountData
        {
            public int Count { get; set; }
            public int Capacity { get; set; }
        }

        private class AttendanceCountData
        {
            public int Count { get; set; }
        }

        private class RealtimeStatusData
        {
            public bool IsMonitoring { get; set; }
        }
    }
}

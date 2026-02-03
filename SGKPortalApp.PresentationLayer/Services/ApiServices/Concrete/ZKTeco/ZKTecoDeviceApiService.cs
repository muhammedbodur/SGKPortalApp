using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Shared.ZKTeco;
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

        public async Task<ApiResponseDto<List<DeviceResponseDto>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<List<DeviceResponseDto>>.ErrorResult("Cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DeviceResponseDto>>>();
                return result ?? ApiResponseDto<List<DeviceResponseDto>>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync failed");
                return ApiResponseDto<List<DeviceResponseDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<DeviceResponseDto>>> GetActiveAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("pdks/device/active");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<List<DeviceResponseDto>>.ErrorResult("Aktif cihazlar yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DeviceResponseDto>>>();
                return result ?? ApiResponseDto<List<DeviceResponseDto>>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveAsync failed");
                return ApiResponseDto<List<DeviceResponseDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<DeviceResponseDto>.ErrorResult("Cihaz yüklenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DeviceResponseDto>>();
                return result ?? ApiResponseDto<DeviceResponseDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed");
                return ApiResponseDto<DeviceResponseDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DeviceStatusDto>> GetStatusAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/status");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<DeviceStatusDto>.ErrorResult("Cihaz durumu alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DeviceStatusDto>>();
                return result ?? ApiResponseDto<DeviceStatusDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatusAsync failed");
                return ApiResponseDto<DeviceStatusDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> CreateAsync(DeviceResponseDto device)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pdks/device", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<DeviceResponseDto>.ErrorResult("Cihaz oluşturulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DeviceResponseDto>>();
                return result ?? ApiResponseDto<DeviceResponseDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed");
                return ApiResponseDto<DeviceResponseDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DeviceResponseDto>> UpdateAsync(int id, DeviceResponseDto device)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"pdks/device/{device.DeviceId}", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<DeviceResponseDto>.ErrorResult("Cihaz güncellenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DeviceResponseDto>>();
                return result ?? ApiResponseDto<DeviceResponseDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed");
                return ApiResponseDto<DeviceResponseDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"pdks/device/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz silinemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        // ========== Device Control ==========

        public async Task<ApiResponseDto<bool>> TestConnectionAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/test", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Bağlantı testi başarısız");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TestConnectionAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DeviceTimeDto>> GetDeviceTimeAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/time");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<DeviceTimeDto>.ErrorResult("Cihaz zamanı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DeviceTimeDto>>();
                return result ?? ApiResponseDto<DeviceTimeDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceTimeAsync failed");
                return ApiResponseDto<DeviceTimeDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> SetDeviceTimeAsync(int deviceId, DateTime? dateTime = null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"pdks/device/{deviceId}/time", dateTime);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz saati ayarlanamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetDeviceTimeAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> SynchronizeDeviceTimeAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/time/sync", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz saati senkronize edilemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SynchronizeDeviceTimeAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> EnableDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/enable", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz etkinleştirilemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EnableDeviceAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DisableDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/disable", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz devre dışı bırakılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DisableDeviceAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> RestartDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/restart", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz yeniden başlatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RestartDeviceAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> PowerOffDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/poweroff", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Cihaz kapatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PowerOffDeviceAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        // ========== User Management ==========

        public async Task<ApiResponseDto<List<ApiUserDto>>> GetDeviceUsersAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<List<ApiUserDto>>.ErrorResult("Kullanıcılar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ApiUserDto>>>();
                return result ?? ApiResponseDto<List<ApiUserDto>>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUsersAsync failed");
                return ApiResponseDto<List<ApiUserDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<DeviceUserMatch>>> GetDeviceUsersWithMismatchesAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/with-mismatches");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<List<DeviceUserMatch>>.ErrorResult("Uyumsuzluk bilgisi alınamadı");
                }
                var apiResult = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DeviceUserMatch>>>();
                if (apiResult == null || !apiResult.Success)
                    return ApiResponseDto<List<DeviceUserMatch>>.ErrorResult(apiResult?.Message ?? "Veri okunamadı");
                return ApiResponseDto<List<DeviceUserMatch>>.SuccessResult(apiResult.Data!, apiResult.Message ?? "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUsersWithMismatchesAsync failed");
                return ApiResponseDto<List<DeviceUserMatch>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<ApiUserDto>> GetDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/{enrollNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<ApiUserDto>.ErrorResult("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ApiUserDto>>();
                return result ?? ApiResponseDto<ApiUserDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserAsync failed");
                return ApiResponseDto<ApiUserDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<ApiUserDto>> GetDeviceUserByCardAsync(int deviceId, long cardNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/card/{cardNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<ApiUserDto>.ErrorResult("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ApiUserDto>>();
                return result ?? ApiResponseDto<ApiUserDto>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserByCardAsync failed");
                return ApiResponseDto<ApiUserDto>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> CreateDeviceUserAsync(int deviceId, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"pdks/device/{deviceId}/users?force={force}", request);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı oluşturulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateDeviceUserAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> UpdateDeviceUserAsync(int deviceId, string enrollNumber, UserCreateUpdateDto request, bool force = false)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"pdks/device/{deviceId}/users/{enrollNumber}?force={force}", request);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı güncellenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateDeviceUserAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users/{enrollNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcı silinemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteDeviceUserAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> ClearAllDeviceUsersAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kullanıcılar temizlenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearAllDeviceUsersAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<int>> GetDeviceUserCountAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/count");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<int>.ErrorResult("Kullanıcı sayısı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                return result ?? ApiResponseDto<int>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUserCountAsync failed");
                return ApiResponseDto<int>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> RemoveCardFromUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users/{enrollNumber}/card");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kart kaldırılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveCardFromUserAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        // ========== Attendance Management ==========

        public async Task<ApiResponseDto<List<AttendanceLogDto>>> GetAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/attendance");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<List<AttendanceLogDto>>.ErrorResult("Kayıtlar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<AttendanceLogDto>>>();
                return result ?? ApiResponseDto<List<AttendanceLogDto>>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceLogsAsync failed");
                return ApiResponseDto<List<AttendanceLogDto>>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> ClearAttendanceLogsAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/attendance");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Kayıtlar temizlenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClearAttendanceLogsAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<int>> GetAttendanceLogCountAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/attendance/count");
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<int>.ErrorResult("Kayıt sayısı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<int>>();
                return result ?? ApiResponseDto<int>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttendanceLogCountAsync failed");
                return ApiResponseDto<int>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        // ========== Realtime Monitoring ==========

        public async Task<ApiResponseDto<bool>> StartRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/monitoring/start", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Canlı izleme başlatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StartRealtimeMonitoringAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<bool>> StopRealtimeMonitoringAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/monitoring/stop", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponseDto<bool>.ErrorResult("Canlı izleme durdurulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
                return result ?? ApiResponseDto<bool>.ErrorResult("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StopRealtimeMonitoringAsync failed");
                return ApiResponseDto<bool>.ErrorResult($"Hata: {ex.Message}");
            }
        }
    }
}

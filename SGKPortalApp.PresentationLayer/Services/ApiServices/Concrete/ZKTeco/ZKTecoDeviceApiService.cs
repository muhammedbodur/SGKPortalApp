using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/status");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceStatusDto>.Fail("Cihaz durumu alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceStatusDto>>();
                return result ?? ServiceResult<DeviceStatusDto>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatusAsync failed");
                return ServiceResult<DeviceStatusDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> CreateAsync(DeviceResponseDto device)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pdks/device", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz oluşturulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceResponseDto>>();
                return result ?? ServiceResult<DeviceResponseDto>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed");
                return ServiceResult<DeviceResponseDto>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DeviceResponseDto>> UpdateAsync(int id, DeviceResponseDto device)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"pdks/device/{device.DeviceId}", device);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceResponseDto>.Fail("Cihaz güncellenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceResponseDto>>();
                return result ?? ServiceResult<DeviceResponseDto>.Fail("Veri okunamadı");
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
                var response = await _httpClient.DeleteAsync($"pdks/device/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz silinemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/test", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Bağlantı testi başarısız");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/time");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<DeviceTimeDto>.Fail("Cihaz zamanı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<DeviceTimeDto>>();
                return result ?? ServiceResult<DeviceTimeDto>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsJsonAsync($"pdks/device/{deviceId}/time", dateTime);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz saati ayarlanamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/time/sync", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz saati senkronize edilemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/enable", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz etkinleştirilemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/disable", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz devre dışı bırakılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/restart", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz yeniden başlatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/poweroff", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Cihaz kapatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<ApiUserDto>>.Fail("Kullanıcılar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<ApiUserDto>>>();
                return result ?? ServiceResult<List<ApiUserDto>>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUsersAsync failed");
                return ServiceResult<List<ApiUserDto>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<DeviceUserMatch>>> GetDeviceUsersWithMismatchesAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/with-mismatches");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<DeviceUserMatch>>.Fail("Uyumsuzluk bilgisi alınamadı");
                }
                var apiResult = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<DeviceUserMatch>>>();
                if (apiResult == null || !apiResult.Success)
                    return ServiceResult<List<DeviceUserMatch>>.Fail(apiResult?.Message ?? "Veri okunamadı");
                return ServiceResult<List<DeviceUserMatch>>.Ok(apiResult.Data!, apiResult.Message ?? "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviceUsersWithMismatchesAsync failed");
                return ServiceResult<List<DeviceUserMatch>>.Fail($"Hata: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ApiUserDto>> GetDeviceUserAsync(int deviceId, string enrollNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/{enrollNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<ApiUserDto>.Fail("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<ApiUserDto>>();
                return result ?? ServiceResult<ApiUserDto>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/card/{cardNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<ApiUserDto>.Fail("Kullanıcı bulunamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<ApiUserDto>>();
                return result ?? ServiceResult<ApiUserDto>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsJsonAsync($"pdks/device/{deviceId}/users?force={force}", request);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kullanıcı oluşturulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PutAsJsonAsync($"pdks/device/{deviceId}/users/{enrollNumber}?force={force}", request);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kullanıcı güncellenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users/{enrollNumber}");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kullanıcı silinemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kullanıcılar temizlenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/users/count");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<int>.Fail("Kullanıcı sayısı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<int>>();
                return result ?? ServiceResult<int>.Fail("Veri okunamadı");
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
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/users/{enrollNumber}/card");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kart kaldırılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/attendance");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<List<AttendanceLogDto>>.Fail("Kayıtlar alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<List<AttendanceLogDto>>>();
                return result ?? ServiceResult<List<AttendanceLogDto>>.Fail("Veri okunamadı");
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
                var response = await _httpClient.DeleteAsync($"pdks/device/{deviceId}/attendance");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Kayıtlar temizlenemedi");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.GetAsync($"pdks/device/{deviceId}/attendance/count");
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<int>.Fail("Kayıt sayısı alınamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<int>>();
                return result ?? ServiceResult<int>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/monitoring/start", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Canlı izleme başlatılamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
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
                var response = await _httpClient.PostAsync($"pdks/device/{deviceId}/monitoring/stop", null);
                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult<bool>.Fail("Canlı izleme durdurulamadı");
                }
                var result = await response.Content.ReadFromJsonAsync<ServiceResult<bool>>();
                return result ?? ServiceResult<bool>.Fail("Veri okunamadı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StopRealtimeMonitoringAsync failed");
                return ServiceResult<bool>.Fail($"Hata: {ex.Message}");
            }
        }
    }
}

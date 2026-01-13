using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri
{
    /// <summary>
    /// ZKTecoApi client interface
    /// ZKTecoApi REST API'sini çağırır (HTTP client)
    /// </summary>
    public interface IZKTecoApiClient
    {
        // ========== Device Operations ==========

        /// <summary>
        /// Cihaza bağlantı testi yap
        /// </summary>
        Task<bool> TestConnectionAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihaz durumu ve bilgilerini al
        /// GET /api/device/{ip}/status
        /// </summary>
        Task<DeviceStatusDto?> GetDeviceStatusAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihaz zamanını getir
        /// GET /api/device/{ip}/time
        /// </summary>
        Task<DeviceTimeDto?> GetDeviceTimeAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihaz zamanını ayarla
        /// POST /api/device/{ip}/time
        /// </summary>
        Task<bool> SetDeviceTimeAsync(string deviceIp, DateTime newTime, int port = 4370);

        /// <summary>
        /// Cihazı aktif et
        /// POST /api/device/{ip}/enable
        /// </summary>
        Task<bool> EnableDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazı pasif et (kart okumaz)
        /// POST /api/device/{ip}/disable
        /// </summary>
        Task<bool> DisableDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazı yeniden başlat
        /// POST /api/device/{ip}/restart
        /// </summary>
        Task<bool> RestartDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazı kapat
        /// POST /api/device/{ip}/poweroff
        /// </summary>
        Task<bool> PowerOffDeviceAsync(string deviceIp, int port = 4370);

        // ========== User Operations (CRUD - Sadece cihaz üzerinde) ==========

        /// <summary>
        /// Cihazdaki tüm kullanıcıları getir
        /// GET /api/users/{ip}
        /// NOT: Database değil, direkt cihazdan
        /// </summary>
        Task<List<ApiUserDto>> GetAllUsersFromDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazdan belirli bir kullanıcıyı getir
        /// GET /api/users/{ip}/{enrollNumber}
        /// </summary>
        Task<ApiUserDto?> GetUserFromDeviceAsync(string deviceIp, string enrollNumber, int port = 4370);

        /// <summary>
        /// Kart numarasıyla kullanıcıyı bul (ZKTecoApi endpoint)
        /// GET /api/users/{ip}/card/{cardNumber}
        /// </summary>
        Task<ApiUserDto?> GetUserByCardNumberAsync(string deviceIp, long cardNumber, int port = 4370);

        /// <summary>
        /// EnrollNumber ile kullanıcıyı bul (ZKTecoApi endpoint)
        /// GET /api/users/{ip}/{enrollNumber}
        /// </summary>
        Task<ApiUserDto?> GetUserByEnrollNumberAsync(string deviceIp, string enrollNumber, int port = 4370);

        /// <summary>
        /// Cihaza kullanıcı ekle
        /// POST /api/users/{ip}
        /// NOT: Database değil, direkt cihaza
        /// </summary>
        /// <param name="force">Kart çakışması varsa otomatik temizle (varsayılan: false)</param>
        Task<bool> AddUserToDeviceAsync(string deviceIp, ApiUserDto user, int port = 4370, bool force = false);

        /// <summary>
        /// Cihazdaki kullanıcıyı güncelle
        /// PUT /api/users/{ip}/{enrollNumber}
        /// </summary>
        Task<bool> UpdateUserOnDeviceAsync(string deviceIp, string enrollNumber, ApiUserDto user, int port = 4370);

        /// <summary>
        /// Cihazdan kullanıcıyı sil
        /// DELETE /api/users/{ip}/{enrollNumber}
        /// </summary>
        Task<bool> DeleteUserFromDeviceAsync(string deviceIp, string enrollNumber, int port = 4370);

        /// <summary>
        /// Cihazdaki tüm kullanıcıları sil
        /// DELETE /api/users/{ip}
        /// </summary>
        Task<bool> ClearAllUsersFromDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazdaki kullanıcı sayısını getir
        /// GET /api/users/{ip}/count
        /// </summary>
        Task<int> GetUserCountFromDeviceAsync(string deviceIp, int port = 4370);

        // ========== Attendance Operations ==========

        /// <summary>
        /// Cihazdan attendance log'larını çek
        /// GET /api/attendance/{ip}
        /// </summary>
        Task<List<AttendanceRecordDto>> GetAttendanceLogsFromDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Attendance log sayısını getir
        /// GET /api/attendance/{ip}/count
        /// </summary>
        Task<int> GetAttendanceLogCountFromDeviceAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazdaki tüm attendance log'larını sil
        /// DELETE /api/attendance/{ip}
        /// </summary>
        Task<bool> ClearAttendanceLogsFromDeviceAsync(string deviceIp, int port = 4370);

        // ========== Realtime Operations ==========

        /// <summary>
        /// Cihazdan realtime event dinlemeyi başlat
        /// POST /api/realtime/{ip}/start
        /// ZKTecoApi üzerinde SDK event listener'ı başlatır
        /// </summary>
        Task<bool> StartRealtimeMonitoringAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Realtime event dinlemeyi durdur
        /// POST /api/realtime/{ip}/stop
        /// </summary>
        Task<bool> StopRealtimeMonitoringAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Realtime monitoring durumunu kontrol et
        /// GET /api/realtime/{ip}/status
        /// Cihaz için event listener aktif mi?
        /// </summary>
        Task<bool> GetRealtimeMonitoringStatusAsync(string deviceIp, int port = 4370);
    }
}

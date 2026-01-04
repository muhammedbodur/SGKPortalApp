using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
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
        /// </summary>
        Task<DeviceStatusDto?> GetDeviceStatusAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazı yeniden başlat
        /// </summary>
        Task<bool> RestartDeviceAsync(string deviceIp, int port = 4370);

        // ========== User Operations (CRUD - Sadece cihaz üzerinde) ==========

        /// <summary>
        /// Cihazdaki tüm kullanıcıları getir
        /// NOT: Database değil, direkt cihazdan
        /// </summary>
        Task<List<UserInfoDto>> GetAllUsersAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazdan belirli bir kullanıcıyı getir
        /// </summary>
        Task<UserInfoDto?> GetUserAsync(string deviceIp, string enrollNumber, int port = 4370);

        /// <summary>
        /// Kart numarasıyla kullanıcıyı bul
        /// </summary>
        Task<UserInfoDto?> GetUserByCardNumberAsync(string deviceIp, long cardNumber, int port = 4370);

        /// <summary>
        /// Cihaza kullanıcı ekle
        /// NOT: Database değil, direkt cihaza
        /// </summary>
        Task<bool> AddUserToDeviceAsync(string deviceIp, UserCreateUpdateDto user, int port = 4370);

        /// <summary>
        /// Cihazdaki kullanıcıyı güncelle
        /// </summary>
        Task<bool> UpdateUserOnDeviceAsync(string deviceIp, string enrollNumber, UserCreateUpdateDto user, int port = 4370);

        /// <summary>
        /// Cihazdan kullanıcıyı sil
        /// </summary>
        Task<bool> DeleteUserFromDeviceAsync(string deviceIp, string enrollNumber, int port = 4370);

        /// <summary>
        /// Cihazdaki kullanıcı sayısını getir
        /// </summary>
        Task<(int count, int capacity)> GetUserCountAsync(string deviceIp, int port = 4370);

        // ========== Attendance Operations ==========

        /// <summary>
        /// Cihazdan attendance log'larını çek
        /// </summary>
        Task<List<AttendanceLogDto>> GetAttendanceLogsAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Attendance log sayısını getir
        /// </summary>
        Task<int> GetAttendanceLogCountAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Cihazdaki tüm attendance log'larını sil
        /// </summary>
        Task<bool> ClearAttendanceLogsAsync(string deviceIp, int port = 4370);

        // ========== Realtime Operations ==========

        /// <summary>
        /// Cihazdan realtime event dinlemeyi başlat
        /// ZKTecoApi üzerinde SDK event listener'ı başlatır
        /// </summary>
        Task<bool> StartRealtimeMonitoringAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Realtime event dinlemeyi durdur
        /// </summary>
        Task<bool> StopRealtimeMonitoringAsync(string deviceIp, int port = 4370);

        /// <summary>
        /// Realtime monitoring durumunu kontrol et
        /// Cihaz için event listener aktif mi?
        /// </summary>
        Task<bool> GetRealtimeMonitoringStatusAsync(string deviceIp, int port = 4370);
    }
}

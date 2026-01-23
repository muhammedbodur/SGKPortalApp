using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// Günde 2 kez (00:00 ve 12:00) tüm aktif cihazlardan attendance kayıtlarını çeken background service
    /// Her cihaz 5 dakika arayla sırayla senkronize edilir
    /// appsettings.json'dan aktif/pasif ve başlama saatleri yapılandırılabilir
    /// </summary>
    public class AttendanceSyncBackgroundService : BackgroundService, IManagedBackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceSyncBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly bool _isEnabled;
        private readonly List<TimeSpan> _syncTimes;
        private readonly TimeSpan _deviceSyncInterval;
        private bool _isRunning;
        private TimeSpan _checkInterval = TimeSpan.FromHours(1);

        // IManagedBackgroundService properties
        public string ServiceName => "AttendanceSyncService";
        public string DisplayName => "ZKTeco Attendance Senkronizasyonu";
        public bool IsRunning => _isRunning;
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; private set; }
        public DateTime? NextRunTime { get; private set; }
        public TimeSpan Interval
        {
            get => _checkInterval;
            set => _checkInterval = value;
        }
        public string? LastError { get; private set; }
        public int SuccessCount { get; private set; }
        public int ErrorCount { get; private set; }

        public AttendanceSyncBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AttendanceSyncBackgroundService> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;

            // appsettings'den ayarları oku
            _isEnabled = configuration.GetValue<bool>("ZKTecoApi:AttendanceSync:Enabled", false);
            
            // Başlama saatlerini config'den al (varsayılan: 00:00 ve 12:00)
            var syncTimesConfig = configuration.GetSection("ZKTecoApi:AttendanceSync:SyncTimes").Get<string[]>() 
                ?? new[] { "00:00", "12:00" };
            
            _syncTimes = new List<TimeSpan>();
            foreach (var timeStr in syncTimesConfig)
            {
                if (TimeSpan.TryParse(timeStr, out var time))
                {
                    _syncTimes.Add(time);
                }
            }

            // Cihazlar arası bekleme süresi (varsayılan: 5 dakika)
            var deviceIntervalMinutes = configuration.GetValue<int>("ZKTecoApi:AttendanceSync:DeviceIntervalMinutes", 5);
            _deviceSyncInterval = TimeSpan.FromMinutes(deviceIntervalMinutes);
        }

        public async Task TriggerAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 {ServiceName} manuel tetiklendi", ServiceName);
            _isRunning = true;
            try
            {
                await SyncAllDevicesSequentiallyAsync(cancellationToken);
                LastRunTime = DateTime.Now;
                LastError = null;
                SuccessCount++;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                ErrorCount++;
                throw;
            }
            finally
            {
                _isRunning = false;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("⏸️ {ServiceName} is DISABLED (appsettings.json)", ServiceName);
                return;
            }

            if (_syncTimes.Count == 0)
            {
                _logger.LogWarning("⚠️ No sync times configured. Service will not run.");
                return;
            }

            _logger.LogInformation($"🔄 {ServiceName} started");
            _logger.LogInformation($"📅 Scheduled sync times: {string.Join(", ", _syncTimes.Select(t => t.ToString(@"hh\:mm")))}");
            _logger.LogInformation($"⏱️ Device sync interval: {_deviceSyncInterval.TotalMinutes} minutes");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextHour = now.Date.AddHours(now.Hour + 1);
                    var delayUntilNextHour = nextHour - now;
                    NextRunTime = nextHour;

                    _logger.LogInformation($"⏰ Next check at: {nextHour:HH:mm} (in {delayUntilNextHour.TotalMinutes:F0} minutes)");
                    await Task.Delay(delayUntilNextHour, stoppingToken);

                    if (stoppingToken.IsCancellationRequested)
                        break;

                    if (IsPaused)
                    {
                        _logger.LogDebug("⏸️ {ServiceName} duraklatıldı, atlanıyor...", ServiceName);
                        continue;
                    }

                    if (await IsSyncNeededTodayAsync())
                    {
                        _logger.LogInformation($"🚀 Sync needed! Starting attendance sync at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        _isRunning = true;
                        await SyncAllDevicesSequentiallyAsync(stoppingToken);
                        LastRunTime = DateTime.Now;
                        LastError = null;
                        SuccessCount++;
                        _logger.LogInformation($"✅ Attendance sync completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    }
                    else
                    {
                        _logger.LogInformation($"✅ Today's sync already completed. Waiting for next check...");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in {ServiceName}", ServiceName);
                    LastError = ex.Message;
                    ErrorCount++;
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                finally
                {
                    _isRunning = false;
                }
            }

            _logger.LogInformation("⏹️ {ServiceName} stopped", ServiceName);
        }

        /// <summary>
        /// Bugün için sync gerekli mi kontrol et
        /// Mantık: Bugünün sync zamanlarından herhangi biri geçmişse ve o zamandan sonra sync yapılmamışsa true döner
        /// </summary>
        private async Task<bool> IsSyncNeededTodayAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();

            var now = DateTime.Now;
            var today = now.Date;

            // Bugünün geçmiş sync zamanlarını bul
            var passedSyncTimes = _syncTimes
                .Where(t => today.Add(t) <= now)
                .OrderByDescending(t => t)
                .ToList();

            if (!passedSyncTimes.Any())
            {
                // Bugün henüz hiçbir sync zamanı gelmedi
                return false;
            }

            // En son geçen sync zamanı
            var lastScheduledSyncTime = today.Add(passedSyncTimes.First());

            // Tüm aktif cihazları al
            var activeDevices = await deviceService.GetActiveDevicesAsync();
            if (!activeDevices.Any())
            {
                _logger.LogInformation("ℹ️ No active devices found");
                return false;
            }

            // Herhangi bir cihaz bugün sync edilmemiş mi kontrol et
            foreach (var device in activeDevices)
            {
                // Cihaz hiç sync edilmemiş veya son sync bugünden önce
                if (device.LastSyncTime == null || device.LastSyncTime.Value.Date < today)
                {
                    _logger.LogInformation($"📌 Device needs sync: {device.DeviceName} (Last sync: {device.LastSyncTime?.ToString("yyyy-MM-dd HH:mm") ?? "Never"})");
                    return true;
                }

                // Cihazın son sync'i, en son geçen sync zamanından önce
                if (device.LastSyncTime.Value < lastScheduledSyncTime)
                {
                    _logger.LogInformation($"📌 Device needs sync: {device.DeviceName} (Last sync: {device.LastSyncTime.Value:yyyy-MM-dd HH:mm}, Scheduled: {lastScheduledSyncTime:HH:mm})");
                    return true;
                }
            }

            // Tüm cihazlar bugün sync edilmiş
            return false;
        }

        /// <summary>
        /// Tüm aktif cihazları sırayla senkronize et (her cihaz arası 5 dakika bekle)
        /// </summary>
        private async Task SyncAllDevicesSequentiallyAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
            var attendanceService = scope.ServiceProvider.GetRequiredService<IZKTecoAttendanceService>();

            try
            {
                _logger.LogInformation("🔄 Starting sequential attendance sync for all active devices...");

                var activeDevices = await deviceService.GetActiveDevicesAsync();

                if (!activeDevices.Any())
                {
                    _logger.LogInformation("ℹ️ No active devices found for sync");
                    return;
                }

                _logger.LogInformation($"📡 Found {activeDevices.Count} active devices");
                _logger.LogInformation($"⏱️ Estimated total sync time: {activeDevices.Count * _deviceSyncInterval.TotalMinutes:F0} minutes");

                int successCount = 0;
                int failCount = 0;
                int deviceIndex = 0;

                foreach (var device in activeDevices)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("⚠️ Sync cancelled by user");
                        break;
                    }

                    deviceIndex++;

                    try
                    {
                        _logger.LogInformation($"🔄 [{deviceIndex}/{activeDevices.Count}] Syncing device: {device.DeviceName} ({device.IpAddress})");

                        // Retry mekanizması: Başarısız olursa 3 kez daha dene
                        bool success = false;
                        int maxRetries = 3;
                        int retryCount = 0;

                        while (!success && retryCount <= maxRetries)
                        {
                            if (retryCount > 0)
                            {
                                _logger.LogInformation($"🔁 Retry {retryCount}/{maxRetries} for device: {device.DeviceName}");
                                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken); // 30 saniye bekle
                            }

                            success = await attendanceService.SyncRecordsFromDeviceToDbAsync(device.DeviceId);
                            
                            if (!success)
                            {
                                retryCount++;
                            }
                        }

                        if (success)
                        {
                            successCount++;
                            if (retryCount > 0)
                            {
                                _logger.LogInformation($"✅ [{deviceIndex}/{activeDevices.Count}] Device synced successfully after {retryCount} retries: {device.DeviceName}");
                            }
                            else
                            {
                                _logger.LogInformation($"✅ [{deviceIndex}/{activeDevices.Count}] Device synced successfully: {device.DeviceName}");
                            }
                        }
                        else
                        {
                            failCount++;
                            _logger.LogWarning($"⚠️ [{deviceIndex}/{activeDevices.Count}] Device sync failed after {maxRetries} retries: {device.DeviceName}");
                        }

                        // Son cihaz değilse, bir sonraki cihaza geçmeden önce bekle
                        if (deviceIndex < activeDevices.Count)
                        {
                            _logger.LogInformation($"⏳ Waiting {_deviceSyncInterval.TotalMinutes} minutes before next device...");
                            await Task.Delay(_deviceSyncInterval, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        _logger.LogError(ex, $"❌ [{deviceIndex}/{activeDevices.Count}] Error syncing device: {device.DeviceName} ({device.IpAddress})");
                        
                        // Hata olsa bile bir sonraki cihaza geçmeden önce bekle
                        if (deviceIndex < activeDevices.Count)
                        {
                            _logger.LogInformation($"⏳ Waiting {_deviceSyncInterval.TotalMinutes} minutes before next device...");
                            await Task.Delay(_deviceSyncInterval, cancellationToken);
                        }
                    }
                }

                _logger.LogInformation($"✅ Sequential attendance sync completed. Success: {successCount}, Failed: {failCount}, Total: {activeDevices.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Fatal error in SyncAllDevicesSequentiallyAsync");
            }
        }
    }
}

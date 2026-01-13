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

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// Günde 2 kez (00:00 ve 12:00) tüm aktif cihazlardan attendance kayıtlarını çeken background service
    /// Her cihaz 5 dakika arayla sırayla senkronize edilir
    /// appsettings.json'dan aktif/pasif ve başlama saatleri yapılandırılabilir
    /// </summary>
    public class AttendanceSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceSyncBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly bool _isEnabled;
        private readonly List<TimeSpan> _syncTimes;
        private readonly TimeSpan _deviceSyncInterval;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("⏸️ Attendance Sync Background Service is DISABLED (appsettings.json)");
                return;
            }

            if (_syncTimes.Count == 0)
            {
                _logger.LogWarning("⚠️ No sync times configured. Service will not run.");
                return;
            }

            _logger.LogInformation($"🔄 Attendance Sync Background Service started");
            _logger.LogInformation($"📅 Scheduled sync times: {string.Join(", ", _syncTimes.Select(t => t.ToString(@"hh\:mm")))}");
            _logger.LogInformation($"⏱️ Device sync interval: {_deviceSyncInterval.TotalMinutes} minutes");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Bir sonraki sync zamanını hesapla
                    var nextSyncTime = GetNextSyncTime();
                    var now = DateTime.Now;
                    var delay = nextSyncTime - now;

                    if (delay.TotalSeconds > 0)
                    {
                        _logger.LogInformation($"⏰ Next sync scheduled at: {nextSyncTime:yyyy-MM-dd HH:mm:ss} (in {delay.TotalHours:F1} hours)");
                        await Task.Delay(delay, stoppingToken);
                    }

                    if (stoppingToken.IsCancellationRequested)
                        break;

                    // Sync işlemini başlat
                    _logger.LogInformation($"🚀 Starting scheduled attendance sync at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    await SyncAllDevicesSequentiallyAsync(stoppingToken);
                    _logger.LogInformation($"✅ Scheduled attendance sync completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in Attendance Sync Background Service");
                    // Hata durumunda 1 dakika bekle ve devam et
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("⏹️ Attendance Sync Background Service stopped");
        }

        /// <summary>
        /// Bir sonraki sync zamanını hesapla
        /// </summary>
        private DateTime GetNextSyncTime()
        {
            var now = DateTime.Now;
            var today = now.Date;

            // Bugünün sync zamanlarını kontrol et
            foreach (var syncTime in _syncTimes.OrderBy(t => t))
            {
                var scheduledTime = today.Add(syncTime);
                if (scheduledTime > now)
                {
                    return scheduledTime;
                }
            }

            // Bugün için tüm sync zamanları geçmişse, yarının ilk sync zamanını al
            var tomorrow = today.AddDays(1);
            var firstSyncTime = _syncTimes.OrderBy(t => t).First();
            return tomorrow.Add(firstSyncTime);
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

                        var success = await attendanceService.SyncRecordsFromDeviceToDbAsync(device.DeviceId);

                        if (success)
                        {
                            successCount++;
                            _logger.LogInformation($"✅ [{deviceIndex}/{activeDevices.Count}] Device synced successfully: {device.DeviceName}");
                        }
                        else
                        {
                            failCount++;
                            _logger.LogWarning($"⚠️ [{deviceIndex}/{activeDevices.Count}] Device sync failed: {device.DeviceName}");
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

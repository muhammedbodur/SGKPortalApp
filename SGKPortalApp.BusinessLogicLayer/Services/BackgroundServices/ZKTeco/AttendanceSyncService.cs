using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServices.ZKTeco
{
    /// <summary>
    /// Günde 2 kez (00:00 ve 12:00) tüm aktif cihazlardan attendance kayıtlarını çeken background service
    /// Her cihaz 5 dakika arayla sırayla senkronize edilir
    /// appsettings.json'dan aktif/pasif ve başlama saatleri yapılandırılabilir
    /// </summary>
    public class AttendanceSyncService : BackgroundService, IManagedBackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceSyncService> _logger;
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

        public AttendanceSyncService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AttendanceSyncService> logger,
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
                LastRunTime = DateTimeHelper.Now;
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

            _logger.LogInformation("🔄 {ServiceName} started", ServiceName);
            _logger.LogInformation("📅 Scheduled sync times: {SyncTimes}", string.Join(", ", _syncTimes.Select(t => t.ToString(@"hh\:mm"))));
            _logger.LogInformation("⏱️ Device sync interval: {Interval} minutes", _deviceSyncInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTimeHelper.Now;
                    var nextHour = now.Date.AddHours(now.Hour + 1);
                    var delayUntilNextHour = nextHour - now;
                    NextRunTime = nextHour;

                    _logger.LogInformation("⏰ Next check at: {NextHour:HH:mm} (in {Delay:F0} minutes)", nextHour, delayUntilNextHour.TotalMinutes);
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
                        _logger.LogInformation("🚀 Sync needed! Starting attendance sync at {Time:yyyy-MM-dd HH:mm:ss}", DateTimeHelper.Now);
                        _isRunning = true;
                        await SyncAllDevicesSequentiallyAsync(stoppingToken);
                        LastRunTime = DateTimeHelper.Now;
                        LastError = null;
                        SuccessCount++;
                        _logger.LogInformation("✅ Attendance sync completed at {Time:yyyy-MM-dd HH:mm:ss}", DateTimeHelper.Now);
                    }
                    else
                    {
                        _logger.LogInformation("✅ Today's sync already completed. Waiting for next check...");
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

        private async Task<bool> IsSyncNeededTodayAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();

            var now = DateTimeHelper.Now;
            var today = now.Date;

            var passedSyncTimes = _syncTimes
                .Where(t => today.Add(t) <= now)
                .OrderByDescending(t => t)
                .ToList();

            if (!passedSyncTimes.Any())
            {
                return false;
            }

            var lastScheduledSyncTime = today.Add(passedSyncTimes.First());

            var activeDevices = await deviceService.GetActiveDevicesAsync();
            if (!activeDevices.Any())
            {
                _logger.LogInformation("ℹ️ No active devices found");
                return false;
            }

            foreach (var device in activeDevices)
            {
                if (device.LastSyncTime == null || device.LastSyncTime.Value.Date < today)
                {
                    _logger.LogInformation("📌 Device needs sync: {DeviceName} (Last sync: {LastSync})", 
                        device.DeviceName, device.LastSyncTime?.ToString("yyyy-MM-dd HH:mm") ?? "Never");
                    return true;
                }

                if (device.LastSyncTime.Value < lastScheduledSyncTime)
                {
                    _logger.LogInformation("📌 Device needs sync: {DeviceName} (Last sync: {LastSync}, Scheduled: {Scheduled:HH:mm})", 
                        device.DeviceName, device.LastSyncTime.Value.ToString("yyyy-MM-dd HH:mm"), lastScheduledSyncTime);
                    return true;
                }
            }

            return false;
        }

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

                _logger.LogInformation("📡 Found {Count} active devices", activeDevices.Count);
                _logger.LogInformation("⏱️ Estimated total sync time: {Time:F0} minutes", activeDevices.Count * _deviceSyncInterval.TotalMinutes);

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
                        _logger.LogInformation("🔄 [{Index}/{Total}] Syncing device: {DeviceName} ({IpAddress})", 
                            deviceIndex, activeDevices.Count, device.DeviceName, device.IpAddress);

                        bool success = false;
                        int maxRetries = 3;
                        int retryCount = 0;

                        while (!success && retryCount <= maxRetries)
                        {
                            if (retryCount > 0)
                            {
                                _logger.LogInformation("🔁 Retry {Retry}/{MaxRetries} for device: {DeviceName}", retryCount, maxRetries, device.DeviceName);
                                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
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
                                _logger.LogInformation("✅ [{Index}/{Total}] Device synced successfully after {Retries} retries: {DeviceName}", 
                                    deviceIndex, activeDevices.Count, retryCount, device.DeviceName);
                            }
                            else
                            {
                                _logger.LogInformation("✅ [{Index}/{Total}] Device synced successfully: {DeviceName}", 
                                    deviceIndex, activeDevices.Count, device.DeviceName);
                            }
                        }
                        else
                        {
                            failCount++;
                            _logger.LogWarning("⚠️ [{Index}/{Total}] Device sync failed after {MaxRetries} retries: {DeviceName}", 
                                deviceIndex, activeDevices.Count, maxRetries, device.DeviceName);
                        }

                        if (deviceIndex < activeDevices.Count)
                        {
                            _logger.LogInformation("⏳ Waiting {Interval} minutes before next device...", _deviceSyncInterval.TotalMinutes);
                            await Task.Delay(_deviceSyncInterval, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        _logger.LogError(ex, "❌ [{Index}/{Total}] Error syncing device: {DeviceName} ({IpAddress})", 
                            deviceIndex, activeDevices.Count, device.DeviceName, device.IpAddress);
                        
                        if (deviceIndex < activeDevices.Count)
                        {
                            _logger.LogInformation("⏳ Waiting {Interval} minutes before next device...", _deviceSyncInterval.TotalMinutes);
                            await Task.Delay(_deviceSyncInterval, cancellationToken);
                        }
                    }
                }

                _logger.LogInformation("✅ Sequential attendance sync completed. Success: {Success}, Failed: {Failed}, Total: {Total}", 
                    successCount, failCount, activeDevices.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Fatal error in SyncAllDevicesSequentiallyAsync");
                throw;
            }
        }
    }
}

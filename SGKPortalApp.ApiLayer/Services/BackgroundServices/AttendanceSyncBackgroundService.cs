using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SGKPortalApp.BusinessObjectLayer.Services.ZKTeco;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// Periyodik olarak tüm aktif cihazlardan attendance kayıtlarını çeken background service
    /// appsettings.json'dan aktif/pasif ve interval yapılandırılabilir
    /// </summary>
    public class AttendanceSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AttendanceSyncBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _syncInterval;
        private readonly bool _isEnabled;

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
            var intervalMinutes = configuration.GetValue<int>("ZKTecoApi:AttendanceSync:IntervalMinutes", 30);
            _syncInterval = TimeSpan.FromMinutes(intervalMinutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("⏸️ Attendance Sync Background Service is DISABLED (appsettings.json)");
                return;
            }

            _logger.LogInformation($"🔄 Attendance Sync Background Service started (Interval: {_syncInterval.TotalMinutes} minutes)");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncAllDevicesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in Attendance Sync Background Service");
                }

                // Bir sonraki sync zamanına kadar bekle
                await Task.Delay(_syncInterval, stoppingToken);
            }

            _logger.LogInformation("⏹️ Attendance Sync Background Service stopped");
        }

        private async Task SyncAllDevicesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var deviceService = scope.ServiceProvider.GetRequiredService<IZKTecoDeviceService>();
            var attendanceService = scope.ServiceProvider.GetRequiredService<IZKTecoAttendanceService>();

            try
            {
                _logger.LogInformation("🔄 Starting attendance sync for all active devices...");

                var activeDevices = await deviceService.GetActiveDevicesAsync();

                if (!activeDevices.Any())
                {
                    _logger.LogInformation("ℹ️ No active devices found for sync");
                    return;
                }

                _logger.LogInformation($"📡 Found {activeDevices.Count} active devices");

                int successCount = 0;
                int failCount = 0;

                foreach (var device in activeDevices)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        _logger.LogInformation($"🔄 Syncing device: {device.DeviceName} ({device.IpAddress})");

                        var success = await attendanceService.SyncRecordsFromDeviceToDbAsync(device.Id);

                        if (success)
                        {
                            successCount++;
                            _logger.LogInformation($"✅ Device synced successfully: {device.DeviceName}");
                        }
                        else
                        {
                            failCount++;
                            _logger.LogWarning($"⚠️ Device sync failed: {device.DeviceName}");
                        }

                        // Cihazlar arasında kısa bir gecikme (API yükünü azaltmak için)
                        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        _logger.LogError(ex, $"❌ Error syncing device: {device.DeviceName} ({device.IpAddress})");
                    }
                }

                _logger.LogInformation($"✅ Attendance sync completed. Success: {successCount}, Failed: {failCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Fatal error in SyncAllDevicesAsync");
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.ApiLayer.Services.Hubs;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// ZKTeco cihazlarından realtime event'leri dinleyen background service
    /// Uygulama başladığında aktif cihazlara abone olur
    /// Gelen event'leri PdksHub'a broadcast eder
    /// Opsiyonel olarak veritabanına kaydeder (configuration ile kontrol)
    /// </summary>
    public class ZKTecoRealtimeListenerService : BackgroundService
    {
        private readonly ILogger<ZKTecoRealtimeListenerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IZKTecoRealtimeService _realtimeService;
        private readonly IHubContext<PdksHub> _pdksHubContext;
        private readonly IConfiguration _configuration;
        private readonly bool _saveToDatabase;

        public ZKTecoRealtimeListenerService(
            ILogger<ZKTecoRealtimeListenerService> logger,
            IServiceProvider serviceProvider,
            IZKTecoRealtimeService realtimeService,
            IHubContext<PdksHub> pdksHubContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _realtimeService = realtimeService;
            _pdksHubContext = pdksHubContext;
            _configuration = configuration;

            // Configuration'dan veritabanına kaydetme seçeneğini oku
            _saveToDatabase = _configuration.GetValue<bool>("ZKTecoApi:Realtime:SaveToDatabase", false);
            _logger.LogInformation($"ZKTeco Realtime - SaveToDatabase: {_saveToDatabase}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ZKTeco Realtime Listener Service starting...");

            try
            {
                // SignalR bağlantısını başlat
                await _realtimeService.StartAsync();

                // Event handler'ı kaydet
                _realtimeService.OnRealtimeEvent += async (sender, evt) =>
                {
                    await ProcessRealtimeEventAsync(evt);
                };

                // Veritabanından aktif cihazları al ve her birine abone ol
                await InitializeDeviceMonitoringAsync();

                _logger.LogInformation("ZKTeco Realtime Listener Service started successfully");

                // Servis durdurulana kadar çalış
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ZKTeco Realtime Listener Service is stopping due to cancellation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ZKTeco Realtime Listener Service failed");
                throw;
            }
        }

        /// <summary>
        /// Veritabanından aktif cihazları al ve monitoring'i başlat
        /// </summary>
        private async Task InitializeDeviceMonitoringAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SGKDbContext>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IZKTecoApiClient>();

                var activeDevices = await dbContext.Devices
                    .Where(d => d.IsActive)
                    .ToListAsync();

                _logger.LogInformation($"Found {activeDevices.Count} active ZKTeco devices");

                foreach (var device in activeDevices)
                {
                    try
                    {
                        // ZKTecoApi'de realtime monitoring'i başlat
                        var port = int.TryParse(device.Port, out var p) ? p : 4370;
                        var started = await apiClient.StartRealtimeMonitoringAsync(device.IpAddress, port);

                        if (started)
                        {
                            // SignalR'da cihaza abone ol
                            await _realtimeService.SubscribeToDeviceAsync(device.IpAddress);
                            _logger.LogInformation(
                                $"Started monitoring device: {device.DeviceName} ({device.IpAddress})");
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Failed to start monitoring device: {device.DeviceName} ({device.IpAddress})");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            $"Error starting monitoring for device: {device.DeviceName} ({device.IpAddress})");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing device monitoring");
                throw;
            }
        }

        /// <summary>
        /// Realtime event'i işle:
        /// 1. PdksHub'a broadcast et (realtime gösterim için)
        /// 2. Opsiyonel olarak veritabanına kaydet
        /// </summary>
        private async Task ProcessRealtimeEventAsync(RealtimeEventDto evt)
        {
            try
            {
                _logger.LogInformation(
                    $"Processing realtime event: EnrollNumber={evt.EnrollNumber}, " +
                    $"EventTime={evt.EventTime}, Device={evt.DeviceIp}, " +
                    $"VerifyMethod={evt.VerifyMethod}, InOutMode={evt.InOutMode}");

                // 1. PdksHub'a broadcast et (tüm bağlı client'lara gönder)
                await BroadcastToPdksHubAsync(evt);

                // 2. Opsiyonel: Veritabanına kaydet
                if (_saveToDatabase)
                {
                    await SaveToDatabaseAsync(evt);
                }
                else
                {
                    _logger.LogDebug("Event database'e kaydedilmedi (SaveToDatabase=false)");
                }

                // TODO: İlerisi için iş kuralları eklenebilir:
                // - Personel kontrolü (EnrollNumber ile Personel tablosundan sorgula)
                // - Mesai dışı giriş kontrolü
                // - E-posta/SMS bildirimi
                // - Geç kalma, erken çıkış hesaplamaları
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Failed to process realtime event: EnrollNumber={evt.EnrollNumber}, " +
                    $"EventTime={evt.EventTime}");
            }
        }

        /// <summary>
        /// Event'i PdksHub'a broadcast et
        /// </summary>
        private async Task BroadcastToPdksHubAsync(RealtimeEventDto evt)
        {
            try
            {
                // PdksHub'a gönder - tüm bağlı client'lar bu event'i alacak
                await _pdksHubContext.Clients.All.SendAsync("OnRealtimeEvent", evt);

                _logger.LogInformation(
                    $"Event broadcast to PdksHub: {evt.EnrollNumber} - {evt.EventTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event to PdksHub");
            }
        }

        /// <summary>
        /// Event'i veritabanına kaydet (opsiyonel)
        /// </summary>
        private async Task SaveToDatabaseAsync(RealtimeEventDto evt)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

                // CekilenData tablosuna kaydet
                var record = new CekilenData
                {
                    KayitNo = evt.EnrollNumber,
                    Tarih = evt.EventTime.Date,
                    Saat = evt.EventTime.TimeOfDay,
                    Dogrulama = evt.VerifyMethod.ToString(),
                    GirisCikisModu = evt.InOutMode.ToString(),
                    WorkCode = evt.WorkCode.ToString(),
                    CihazIp = evt.DeviceIp,
                    CekilmeTarihi = DateTime.Now,
                    IsProcessed = false
                };

                await dbContext.CekilenDatalar.AddAsync(record);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    $"Realtime event saved to database: {evt.EnrollNumber} - {evt.EventTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save event to database");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ZKTeco Realtime Listener Service stopping...");

            try
            {
                // Tüm cihazlardan aboneliği kaldır
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SGKDbContext>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IZKTecoApiClient>();

                var activeDevices = await dbContext.Devices
                    .Where(d => d.IsActive)
                    .ToListAsync(cancellationToken);

                foreach (var device in activeDevices)
                {
                    try
                    {
                        var port = int.TryParse(device.Port, out var p) ? p : 4370;
                        await apiClient.StopRealtimeMonitoringAsync(device.IpAddress, port);
                        await _realtimeService.UnsubscribeFromDeviceAsync(device.IpAddress);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            $"Error stopping monitoring for device: {device.DeviceName}");
                    }
                }

                // SignalR bağlantısını kapat
                await _realtimeService.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping ZKTeco Realtime Listener Service");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}

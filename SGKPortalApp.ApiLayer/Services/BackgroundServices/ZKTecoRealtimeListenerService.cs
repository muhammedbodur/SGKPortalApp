using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.ApiLayer.Services.Hubs;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PersonelIslemleri;
using System;
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
    public class ZKTecoRealtimeListenerService : BackgroundService, IManagedBackgroundService
    {
        private readonly ILogger<ZKTecoRealtimeListenerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IZKTecoRealtimeService _realtimeService;
        private readonly IHubContext<PdksHub> _pdksHubContext;
        private readonly IConfiguration _configuration;
        private readonly bool _saveToDatabase;
        private bool _isRunning;
        private TimeSpan _interval = TimeSpan.Zero; // Sürekli çalışır

        // IManagedBackgroundService properties
        public string ServiceName => "ZKTecoRealtimeService";
        public string DisplayName => "ZKTeco Realtime Event Dinleyici";
        public bool IsRunning => _isRunning;
        public bool IsPaused { get; set; }
        public DateTime? LastRunTime { get; private set; }
        public DateTime? NextRunTime => null; // Sürekli çalışır
        public TimeSpan Interval
        {
            get => _interval;
            set => _interval = value;
        }
        public string? LastError { get; private set; }
        public int SuccessCount { get; private set; }
        public int ErrorCount { get; private set; }

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

        public async Task TriggerAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔄 {ServiceName} manuel tetiklendi - cihaz monitoring yeniden başlatılıyor", ServiceName);
            try
            {
                await InitializeDeviceMonitoringAsync();
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🔄 {ServiceName} starting...", ServiceName);
            _isRunning = true;

            try
            {
                // SignalR bağlantısını başlat
                await _realtimeService.StartAsync();

                // Event handler'ı kaydet
                _realtimeService.OnRealtimeEvent += async (sender, evt) =>
                {
                    if (!IsPaused)
                    {
                        await ProcessRealtimeEventAsync(evt);
                        SuccessCount++;
                    }
                };

                // Veritabanından aktif cihazları al ve her birine abone ol
                await InitializeDeviceMonitoringAsync();
                LastRunTime = DateTime.Now;

                _logger.LogInformation("✅ {ServiceName} started successfully", ServiceName);

                // Servis durdurulana kadar çalış
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("⏹️ {ServiceName} is stopping due to cancellation", ServiceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {ServiceName} failed", ServiceName);
                LastError = ex.Message;
                ErrorCount++;
                throw;
            }
            finally
            {
                _isRunning = false;
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
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IZKTecoApiClient>();

                var deviceRepository = unitOfWork.GetRepository<IDeviceRepository>();
                var activeDevices = await deviceRepository.GetActiveDevicesAsync();

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

                // 1. Personel bilgilerini ekle (EnrollNumber ile Personel tablosundan sorgula)
                await EnrichEventWithPersonelDataAsync(evt);

                // 2. Cihaz bilgilerini ekle (DeviceIp ile Device tablosundan sorgula)
                await EnrichEventWithDeviceDataAsync(evt);

                // 3. PdksHub'a broadcast et (tüm bağlı client'lara gönder)
                await BroadcastToPdksHubAsync(evt);

                // 4. Opsiyonel: Veritabanına kaydet
                if (_saveToDatabase)
                {
                    await SaveToDatabaseAsync(evt);
                }
                else
                {
                    _logger.LogDebug("Event database'e kaydedilmedi (SaveToDatabase=false)");
                }

                // TODO: İlerisi için iş kuralları eklenebilir:
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
        /// Event'e personel bilgilerini ekle (EnrollNumber ile eşleştirme)
        /// </summary>
        private async Task EnrichEventWithPersonelDataAsync(RealtimeEventDto evt)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var personelRepository = unitOfWork.GetRepository<IPersonelRepository>();

                // EnrollNumber'ı int'e çevir ve PersonelKayitNo ile eşleştir
                if (!int.TryParse(evt.EnrollNumber, out int personelKayitNo))
                {
                    _logger.LogWarning($"EnrollNumber int'e çevrilemedi: {evt.EnrollNumber}");
                    return;
                }

                // EnrollNumber ile personel ara (PersonelKayitNo eşleşmesi)
                var personel = await personelRepository.GetByPersonelKayitNoAsync(personelKayitNo);
                
                if (personel != null)
                {
                    evt.PersonelAdSoyad = $"{personel.AdSoyad}";
                    evt.PersonelSicilNo = personel.SicilNo.ToString();
                    evt.PersonelTcKimlikNo = personel.TcKimlikNo;
                    evt.PersonelDepartman = personel.Departman?.DepartmanAdi;
                    evt.PersonelKayitNo = personel.PersonelKayitNo;

                    _logger.LogDebug($"Personel bilgisi eklendi: {evt.PersonelAdSoyad} (SicilNo: {evt.PersonelSicilNo}, PersonelKayitNo: {personelKayitNo})");
                }
                else
                {
                    _logger.LogWarning($"Personel bulunamadı: PersonelKayitNo={personelKayitNo} (EnrollNumber={evt.EnrollNumber})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Personel bilgisi eklenirken hata: EnrollNumber={evt.EnrollNumber}");
                // Hata olsa bile event'i gönder
            }
        }

        /// <summary>
        /// Event'e cihaz bilgilerini ekle (DeviceIp ile eşleştirme)
        /// </summary>
        private async Task EnrichEventWithDeviceDataAsync(RealtimeEventDto evt)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var deviceRepository = unitOfWork.GetRepository<IDeviceRepository>();

                // DeviceIp ile cihazı ara
                var device = await deviceRepository.GetDeviceByIpAsync(evt.DeviceIp);

                if (device != null)
                {
                    evt.DeviceName = device.DeviceName ?? string.Empty;
                    evt.HizmetBinasiId = device.HizmetBinasiId;
                    evt.HizmetBinasiAdi = device.HizmetBinasi?.HizmetBinasiAdi ?? string.Empty;

                    _logger.LogDebug($"Cihaz bilgisi eklendi: {evt.DeviceName} (IP: {evt.DeviceIp}, Hizmet Binası: {evt.HizmetBinasiAdi})");
                }
                else
                {
                    _logger.LogWarning($"Cihaz bulunamadı: DeviceIp={evt.DeviceIp}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cihaz bilgisi eklenirken hata: DeviceIp={evt.DeviceIp}");
                // Hata olsa bile event'i gönder
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
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var deviceRepository = unitOfWork.GetRepository<IDeviceRepository>();
                var cekilenDataRepository = unitOfWork.GetRepository<ICekilenDataRepository>();

                // Device'ı IP'den bul
                var device = await deviceRepository.GetDeviceByIpAsync(evt.DeviceIp);
                if (device == null)
                {
                    _logger.LogWarning($"Device not found for IP: {evt.DeviceIp}. Event will not be saved.");
                    return;
                }

                // CekilenData tablosuna kaydet
                var record = new CekilenData
                {
                    DeviceId = device.DeviceId,
                    KayitNo = evt.EnrollNumber,
                    Tarih = evt.EventTime,
                    Dogrulama = evt.VerifyMethod.ToString(),
                    GirisCikisModu = evt.InOutMode.ToString(),
                    WorkCode = evt.WorkCode.ToString(),
                    CihazIp = evt.DeviceIp,
                    CekilmeTarihi = DateTime.Now,
                    IsProcessed = false
                };

                await cekilenDataRepository.AddAsync(record);
                await unitOfWork.SaveChangesAsync();

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
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var apiClient = scope.ServiceProvider.GetRequiredService<IZKTecoApiClient>();

                var deviceRepository = unitOfWork.GetRepository<IDeviceRepository>();
                var activeDevices = await deviceRepository.GetActiveDevicesAsync(cancellationToken);

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

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.BusinessLogicLayer.Services.BackgroundServiceManager
{
    public class BackgroundServiceManager : IBackgroundServiceManager
    {
        private readonly IEnumerable<IManagedBackgroundService> _services;
        private readonly ILogger<BackgroundServiceManager> _logger;

        public BackgroundServiceManager(
            IEnumerable<IManagedBackgroundService> services,
            ILogger<BackgroundServiceManager> logger)
        {
            _services = services;
            _logger = logger;
        }

        public IEnumerable<BackgroundServiceStatus> GetAllServiceStatuses()
        {
            return _services.Select(s => new BackgroundServiceStatus
            {
                ServiceName = s.ServiceName,
                DisplayName = s.DisplayName,
                IsRunning = s.IsRunning,
                IsPaused = s.IsPaused,
                LastRunTime = s.LastRunTime,
                NextRunTime = s.NextRunTime,
                Interval = s.Interval,
                LastError = s.LastError,
                SuccessCount = s.SuccessCount,
                ErrorCount = s.ErrorCount
            });
        }

        public BackgroundServiceStatus? GetServiceStatus(string serviceName)
        {
            var service = _services.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null) return null;

            return new BackgroundServiceStatus
            {
                ServiceName = service.ServiceName,
                DisplayName = service.DisplayName,
                IsRunning = service.IsRunning,
                IsPaused = service.IsPaused,
                LastRunTime = service.LastRunTime,
                NextRunTime = service.NextRunTime,
                Interval = service.Interval,
                LastError = service.LastError,
                SuccessCount = service.SuccessCount,
                ErrorCount = service.ErrorCount
            };
        }

        public async Task<bool> TriggerServiceAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            var service = _services.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null)
            {
                _logger.LogWarning("Servis bulunamadı: {ServiceName}", serviceName);
                return false;
            }

            try
            {
                _logger.LogInformation("🔄 Servis manuel tetikleniyor: {ServiceName}", serviceName);
                await service.TriggerAsync(cancellationToken);
                _logger.LogInformation("✅ Servis başarıyla tetiklendi: {ServiceName}", serviceName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Servis tetiklenirken hata: {ServiceName}", serviceName);
                return false;
            }
        }

        public bool PauseService(string serviceName)
        {
            var service = _services.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null) return false;

            service.IsPaused = true;
            _logger.LogInformation("⏸️ Servis duraklatıldı: {ServiceName}", serviceName);
            return true;
        }

        public bool ResumeService(string serviceName)
        {
            var service = _services.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null) return false;

            service.IsPaused = false;
            _logger.LogInformation("▶️ Servis devam ettiriliyor: {ServiceName}", serviceName);
            return true;
        }

        public bool SetServiceInterval(string serviceName, TimeSpan interval)
        {
            var service = _services.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service == null) return false;

            service.Interval = interval;
            _logger.LogInformation("⏱️ Servis aralığı değiştirildi: {ServiceName} -> {Interval}", serviceName, interval);
            return true;
        }
    }
}

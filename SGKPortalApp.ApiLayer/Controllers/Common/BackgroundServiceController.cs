using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.BackgroundServiceManager;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BackgroundServiceController : ControllerBase
    {
        private readonly IBackgroundServiceManager _serviceManager;
        private readonly ILogger<BackgroundServiceController> _logger;

        public BackgroundServiceController(
            IBackgroundServiceManager serviceManager,
            ILogger<BackgroundServiceController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        /// <summary>
        /// Tüm background servislerin durumlarını getirir
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<BackgroundServiceStatus>> GetAll()
        {
            var statuses = _serviceManager.GetAllServiceStatuses();
            return Ok(statuses);
        }

        /// <summary>
        /// Belirli bir servisin durumunu getirir
        /// </summary>
        [HttpGet("{serviceName}")]
        public ActionResult<BackgroundServiceStatus> Get(string serviceName)
        {
            var status = _serviceManager.GetServiceStatus(serviceName);
            if (status == null)
                return NotFound(new { message = $"Servis bulunamadı: {serviceName}" });

            return Ok(status);
        }

        /// <summary>
        /// Servisi manuel olarak tetikler (hemen çalıştırır)
        /// </summary>
        [HttpPost("{serviceName}/trigger")]
        public async Task<IActionResult> Trigger(string serviceName, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔄 Servis tetikleme isteği: {ServiceName}, Kullanıcı: {User}", 
                serviceName, User.Identity?.Name);

            var result = await _serviceManager.TriggerServiceAsync(serviceName, cancellationToken);
            if (!result)
                return NotFound(new { message = $"Servis bulunamadı veya tetiklenemedi: {serviceName}" });

            return Ok(new { message = $"Servis tetiklendi: {serviceName}" });
        }

        /// <summary>
        /// Servisi duraklatır
        /// </summary>
        [HttpPost("{serviceName}/pause")]
        public IActionResult Pause(string serviceName)
        {
            _logger.LogInformation("⏸️ Servis duraklatma isteği: {ServiceName}, Kullanıcı: {User}", 
                serviceName, User.Identity?.Name);

            var result = _serviceManager.PauseService(serviceName);
            if (!result)
                return NotFound(new { message = $"Servis bulunamadı: {serviceName}" });

            return Ok(new { message = $"Servis duraklatıldı: {serviceName}" });
        }

        /// <summary>
        /// Duraklatılmış servisi devam ettirir
        /// </summary>
        [HttpPost("{serviceName}/resume")]
        public IActionResult Resume(string serviceName)
        {
            _logger.LogInformation("▶️ Servis devam ettirme isteği: {ServiceName}, Kullanıcı: {User}", 
                serviceName, User.Identity?.Name);

            var result = _serviceManager.ResumeService(serviceName);
            if (!result)
                return NotFound(new { message = $"Servis bulunamadı: {serviceName}" });

            return Ok(new { message = $"Servis devam ettiriliyor: {serviceName}" });
        }

        /// <summary>
        /// Servisin çalışma aralığını değiştirir
        /// </summary>
        [HttpPost("{serviceName}/interval")]
        public IActionResult SetInterval(string serviceName, [FromBody] SetIntervalRequest request)
        {
            _logger.LogInformation("⏱️ Servis aralığı değiştirme isteği: {ServiceName} -> {Minutes} dakika, Kullanıcı: {User}", 
                serviceName, request.IntervalMinutes, User.Identity?.Name);

            var interval = TimeSpan.FromMinutes(request.IntervalMinutes);
            var result = _serviceManager.SetServiceInterval(serviceName, interval);
            if (!result)
                return NotFound(new { message = $"Servis bulunamadı: {serviceName}" });

            return Ok(new { message = $"Servis aralığı değiştirildi: {serviceName} -> {request.IntervalMinutes} dakika" });
        }
    }

    public class SetIntervalRequest
    {
        public int IntervalMinutes { get; set; } = 5;
    }
}

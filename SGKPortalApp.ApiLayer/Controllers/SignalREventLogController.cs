using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;

namespace SGKPortalApp.ApiLayer.Controllers
{
    /// <summary>
    /// SignalR Event Log API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SignalREventLogController : ControllerBase
    {
        private readonly ISignalREventLogService _eventLogService;
        private readonly ILogger<SignalREventLogController> _logger;

        public SignalREventLogController(
            ISignalREventLogService eventLogService,
            ILogger<SignalREventLogController> logger)
        {
            _eventLogService = eventLogService;
            _logger = logger;
        }

        /// <summary>
        /// Event loglarını filtreli ve sayfalı olarak getirir
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult<PagedResultDto<SignalREventLogResponseDto>>> GetFiltered([FromBody] SignalREventLogFilterDto filter)
        {
            try
            {
                var result = await _eventLogService.GetFilteredAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR event log filtreleme hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// Son N dakikadaki eventleri getirir (canlı izleme için)
        /// </summary>
        [HttpGet("recent/{minutes:int}")]
        public async Task<ActionResult<List<SignalREventLogResponseDto>>> GetRecent(int minutes = 5)
        {
            try
            {
                var items = await _eventLogService.GetRecentAsync(minutes);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR recent events hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirli bir sıranın tüm eventlerini getirir
        /// </summary>
        [HttpGet("by-sira/{siraId:int}")]
        public async Task<ActionResult<List<SignalREventLogResponseDto>>> GetBySira(int siraId)
        {
            try
            {
                var items = await _eventLogService.GetBySiraAsync(siraId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR by-sira hatası: {SiraId}", siraId);
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// İstatistikleri getirir
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<SignalREventLogStatsDto>> GetStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var stats = await _eventLogService.GetStatsAsync(startDate, endDate);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR stats hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

    }
}

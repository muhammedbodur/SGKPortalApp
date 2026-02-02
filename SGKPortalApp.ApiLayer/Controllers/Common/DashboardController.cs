using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Dashboard için tüm verileri tek seferde getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var result = await _dashboardService.GetDashboardDataAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Slider duyurularını getirir
        /// </summary>
        [HttpGet("slider-duyurular")]
        public async Task<IActionResult> GetSliderDuyurular([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetSliderDuyurularAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Liste duyurularını getirir
        /// </summary>
        [HttpGet("liste-duyurular")]
        public async Task<IActionResult> GetListeDuyurular([FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetListeDuyurularAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Sık kullanılan programları getirir
        /// </summary>
        [HttpGet("sik-kullanilan-programlar")]
        public async Task<IActionResult> GetSikKullanilanProgramlar([FromQuery] int count = 8)
        {
            var result = await _dashboardService.GetSikKullanilanProgramlarAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Önemli linkleri getirir
        /// </summary>
        [HttpGet("onemli-linkler")]
        public async Task<IActionResult> GetOnemliLinkler([FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetOnemliLinklerAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Günün menüsünü getirir
        /// </summary>
        [HttpGet("gunun-menusu")]
        public async Task<IActionResult> GetGununMenusu()
        {
            var result = await _dashboardService.GetGununMenusuAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bugün doğanları getirir
        /// </summary>
        [HttpGet("bugun-doganlar")]
        public async Task<IActionResult> GetBugunDoganlar()
        {
            var result = await _dashboardService.GetBugunDoganlarAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

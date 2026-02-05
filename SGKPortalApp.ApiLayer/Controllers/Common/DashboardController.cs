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
        /// Slider haberlerini getirir
        /// </summary>
        [HttpGet("slider-haberler")]
        public async Task<IActionResult> GetSliderHaberler([FromQuery] int count = 5)
        {
            var result = await _dashboardService.GetSliderHaberleriAsync(count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Liste haberlerini getirir
        /// </summary>
        [HttpGet("liste-haberler")]
        public async Task<IActionResult> GetListeHaberler([FromQuery] int count = 10)
        {
            var result = await _dashboardService.GetListeHaberleriAsync(count);
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

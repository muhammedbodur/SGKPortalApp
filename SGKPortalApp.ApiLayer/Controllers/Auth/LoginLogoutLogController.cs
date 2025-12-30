using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;

namespace SGKPortalApp.ApiLayer.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginLogoutLogController : ControllerBase
    {
        private readonly ILoginLogoutLogService _loginLogoutLogService;
        private readonly ILogger<LoginLogoutLogController> _logger;

        public LoginLogoutLogController(
            ILoginLogoutLogService loginLogoutLogService,
            ILogger<LoginLogoutLogController> logger)
        {
            _loginLogoutLogService = loginLogoutLogService;
            _logger = logger;
        }

        /// <summary>
        /// Login/Logout loglarını filtreli listele
        /// </summary>
        /// <param name="filter">Filtre parametreleri</param>
        /// <returns>Sayfalanmış log listesi</returns>
        [HttpPost("logs")]
        public async Task<IActionResult> GetLogs([FromBody] LoginLogoutLogFilterDto filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _loginLogoutLogService.GetLogsAsync(filter);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre log detayı getir
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>Log detayı</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogById(int id)
        {
            var result = await _loginLogoutLogService.GetLogByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Aktif oturum sayısı
        /// </summary>
        /// <returns>Aktif oturum sayısı</returns>
        [HttpGet("active-session-count")]
        public async Task<IActionResult> GetActiveSessionCount()
        {
            var result = await _loginLogoutLogService.GetActiveSessionCountAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Bugünkü login sayısı
        /// </summary>
        /// <returns>Bugünkü login sayısı</returns>
        [HttpGet("today-login-count")]
        public async Task<IActionResult> GetTodayLoginCount()
        {
            var result = await _loginLogoutLogService.GetTodayLoginCountAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Session hala geçerli mi kontrol et (LogoutTime set edilmemiş mi?)
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>True: Session geçerli, False: Session sonlanmış</returns>
        [HttpGet("is-session-valid/{sessionId}")]
        [AllowAnonymous] // Middleware'den çağrılacak, authentication öncesi
        public async Task<IActionResult> IsSessionValid(string sessionId)
        {
            try
            {
                var result = await _loginLogoutLogService.IsSessionValidAsync(sessionId);

                if (!result.Success)
                {
                    return Ok(result); // Success=false döndür ama HTTP 200
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Session validity kontrolü hatası - SessionID: {SessionId}", sessionId);
                return Ok(new { success = false, data = false, message = "Session kontrolü başarısız" });
            }
        }
    }
}

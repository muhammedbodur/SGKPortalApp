using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.ApiLayer.Services.Hubs;
using SGKPortalApp.ApiLayer.Services.Hubs.Constants;
using SGKPortalApp.ApiLayer.Services.Hubs.Interfaces;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;

namespace SGKPortalApp.ApiLayer.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBankoModeService _bankoModeService;
        private readonly IHubConnectionService _hubConnectionService;
        private readonly IHubContext<SiramatikHub> _hubContext;
        private readonly ILogger<AuthController> _logger;
        private readonly ILoginLogoutLogService _loginLogoutLogService;

        public AuthController(
            IAuthService authService,
            IBankoModeService bankoModeService,
            IHubConnectionService hubConnectionService,
            IHubContext<SiramatikHub> hubContext,
            ILogger<AuthController> logger,
            ILoginLogoutLogService loginLogoutLogService)
        {
            _authService = authService;
            _bankoModeService = bankoModeService;
            _hubConnectionService = hubConnectionService;
            _hubContext = hubContext;
            _logger = logger;
            _loginLogoutLogService = loginLogoutLogService;
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        /// <param name="request">TC Kimlik No ve Şifre</param>
        /// <returns>Login sonucu ve kullanıcı bilgileri</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // HTTP context bilgilerini ekle
            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(result);
        }

        /// <summary>
        /// Şifre sıfırlama için kimlik doğrulama
        /// TC, Sicil No, Doğum Tarihi ve Email ile doğrulama
        /// </summary>
        /// <param name="request">Kimlik doğrulama bilgileri</param>
        /// <returns>Doğrulama sonucu</returns>
        [HttpPost("verify-identity")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyIdentity([FromBody] VerifyIdentityRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyIdentityAsync(request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        /// <summary>
        /// Şifre sıfırlama
        /// Kimlik doğrulandıktan sonra yeni şifre belirleme
        /// </summary>
        /// <param name="request">Yeni şifre bilgileri</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(request);

            if (!result)
            {
                return BadRequest(new { message = "Şifre sıfırlama başarısız oldu. Lütfen tekrar deneyin." });
            }

            return Ok(new { message = "Şifreniz başarıyla değiştirildi. Giriş yapabilirsiniz." });
        }

        /// <summary>
        /// Session ID ile logout time güncelleme
        /// Başka cihazdan login yapıldığında eski session'ı kapatmak için kullanılır
        /// </summary>
        [HttpPost("logout-by-session/{sessionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutBySessionId(string sessionId)
        {
            try
            {
                _logger.LogInformation("🔄 Logout by SessionID: {SessionId}", sessionId);

                var result = await _loginLogoutLogService.UpdateLogoutTimeBySessionIdAsync(sessionId);
                if (result.Success && result.Data)
                {
                    _logger.LogInformation("✅ Logout time güncellendi - SessionID: {SessionId}", sessionId);
                    return Ok(new { success = true, message = "Logout time güncellendi" });
                }

                return Ok(new { success = false, message = "Logout time güncellenemedi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout by SessionID hatası - SessionID: {SessionId}", sessionId);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Çıkış işlemi
        /// Banko modundan çıkış ve session temizleme
        /// </summary>
        [HttpPost("logout")]
        [AllowAnonymous] // Cookie authentication zaten logout'ta temizlenmiş olabilir
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request)
        {
            try
            {
                // TcKimlikNo: Önce request'ten, yoksa User claim'den al
                var tcKimlikNo = request?.TcKimlikNo ?? User?.FindFirst("TcKimlikNo")?.Value;

                if (!string.IsNullOrEmpty(tcKimlikNo))
                {
                    _logger.LogInformation("🔄 Logout: {TcKimlikNo} çıkış yapıyor...", tcKimlikNo);

                    // LoginLogoutLog kaydını güncelle
                    var result = await _loginLogoutLogService.UpdateLogoutTimeByTcKimlikNoAsync(tcKimlikNo);
                    if (result.Success && result.Data)
                    {
                        _logger.LogInformation("✅ Logout log kaydı güncellendi - TcKimlikNo: {TcKimlikNo}", tcKimlikNo);
                    }

                    // Banko modundan çık (flag temizle)
                    try
                    {
                        await _bankoModeService.ExitBankoModeAsync(tcKimlikNo);
                        _logger.LogInformation("✅ Logout: Banko modu temizlendi - {TcKimlikNo}", tcKimlikNo);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "⚠️ Logout: Banko modu temizlenirken hata - {TcKimlikNo}", tcKimlikNo);
                    }

                    await ForceLogoutActiveSessionsAsync(tcKimlikNo);
                }

                return Ok(new { message = "Çıkış başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout sırasında hata oluştu");
                return Ok(new { message = "Çıkış tamamlandı (bazı hatalarla)" });
            }
        }

        /// <summary>
        /// Kullanıcı aktivitesini güncelle (heartbeat için)
        /// JavaScript heartbeat tarafından çağrılır
        /// </summary>
        [HttpPost("ping-activity")]
        public async Task<IActionResult> PingActivity()
        {
            try
            {
                // HttpContext'ten TC Kimlik No al
                var tcKimlikNo = User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    return Unauthorized(new { message = "TC Kimlik No bulunamadı" });
                }

                return await UpdateUserActivity(tcKimlikNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ping activity hatası");
                return StatusCode(500, new { message = "Aktivite güncellenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Belirli bir kullanıcının son aktivite zamanını güncelle
        /// Middleware tarafından çağrılır
        /// </summary>
        [HttpPost("update-activity/{tcKimlikNo}")]
        public async Task<IActionResult> UpdateActivity(string tcKimlikNo)
        {
            try
            {
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    return BadRequest(new { message = "TC Kimlik No gerekli" });
                }

                return await UpdateUserActivity(tcKimlikNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Update activity hatası - TC: {TcKimlikNo}", tcKimlikNo);
                return StatusCode(500, new { message = "Aktivite güncellenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Ortak aktivite güncelleme metodu
        /// </summary>
        private async Task<IActionResult> UpdateUserActivity(string tcKimlikNo)
        {
            try
            {
                // User repository'den kullanıcıyı al ve son aktivite zamanını güncelle
                var userService = HttpContext.RequestServices.GetService<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.IUnitOfWork>();
                if (userService == null)
                {
                    return StatusCode(500, new { message = "User service bulunamadı" });
                }

                var userRepo = userService.GetRepository<SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common.IUserRepository>();
                var user = await userRepo.GetByTcKimlikNoAsync(tcKimlikNo);

                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }

                // Son aktivite zamanını güncelle
                user.SonAktiviteZamani = DateTime.Now;
                await userService.SaveChangesAsync();

                _logger.LogDebug("✅ Son aktivite zamanı güncellendi - TC: {TcKimlikNo}", tcKimlikNo);
                return Ok(new { success = true, message = "Aktivite güncellendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Aktivite güncelleme hatası - TC: {TcKimlikNo}", tcKimlikNo);
                return StatusCode(500, new { message = "Aktivite güncellenirken hata oluştu" });
            }
        }

        private async Task ForceLogoutActiveSessionsAsync(string tcKimlikNo)
        {
            try
            {
                var connections = await _hubConnectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
                if (connections == null || connections.Count == 0)
                    return;

                foreach (var connection in connections)
                {
                    _logger.LogInformation("📡 Logout: ForceLogout gönderiliyor - ConnectionId={ConnectionId}", connection.ConnectionId);

                    await _hubContext.Clients.Client(connection.ConnectionId)
                        .SendAsync(SignalREvents.ForceLogout, "Oturumunuz sonlandırıldı. Lütfen tekrar giriş yapın.");

                    await _hubContext.Clients.Client(connection.ConnectionId)
                        .SendAsync(SignalREvents.BankoModeDeactivated, new { reason = "logout" });

                    await _hubConnectionService.DisconnectAsync(connection.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Logout: force logout yayınlanırken hata - {TcKimlikNo}", tcKimlikNo);
            }
        }
    }
}

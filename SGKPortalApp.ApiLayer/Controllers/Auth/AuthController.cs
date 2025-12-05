using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IBankoModeService bankoModeService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _bankoModeService = bankoModeService;
            _logger = logger;
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
                }

                return Ok(new { message = "Çıkış başarılı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Logout sırasında hata oluştu");
                return Ok(new { message = "Çıkış tamamlandı (bazı hatalarla)" });
            }
        }
    }
}

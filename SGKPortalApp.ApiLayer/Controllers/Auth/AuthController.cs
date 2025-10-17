using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;

namespace SGKPortalApp.ApiLayer.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
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
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Session ID'yi temizle (opsiyonel - gerekirse implement edilir)
            return Ok(new { message = "Çıkış başarılı" });
        }
    }
}

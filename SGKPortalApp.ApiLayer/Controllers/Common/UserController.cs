using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Account;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // 📋 KULLANICI SORGULAMA
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// TC Kimlik No ile kullanıcı getir
        /// </summary>
        [HttpGet("{tcKimlikNo}")]
        public async Task<IActionResult> GetByTcKimlikNo(string tcKimlikNo)
        {
            var result = await _userService.GetByTcKimlikNoAsync(tcKimlikNo);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Tüm kullanıcıları listele
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Aktif kullanıcıları listele
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _userService.GetActiveUsersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Kilitli kullanıcıları listele
        /// </summary>
        [HttpGet("locked")]
        public async Task<IActionResult> GetLockedUsers()
        {
            var result = await _userService.GetLockedUsersAsync();
            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // ✏️ KULLANICI GÜNCELLEME
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Kullanıcı güncelle (Sadece AktifMi)
        /// </summary>
        [HttpPut("{tcKimlikNo}")]
        public async Task<IActionResult> Update(string tcKimlikNo, [FromBody] UserUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateAsync(tcKimlikNo, request);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcı sil
        /// </summary>
        [HttpDelete("{tcKimlikNo}")]
        public async Task<IActionResult> Delete(string tcKimlikNo)
        {
            var result = await _userService.DeleteAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // 🔐 ŞİFRE İŞLEMLERİ
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Şifre değiştir (Eski şifre gerekli)
        /// </summary>
        [Authorize]
        [HttpPost("{tcKimlikNo}/change-password")]
        public async Task<IActionResult> ChangePassword(
            string tcKimlikNo,
            [FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentTc = User.FindFirst("TcKimlikNo")?.Value;
            if (string.IsNullOrWhiteSpace(currentTc) || !string.Equals(currentTc, tcKimlikNo, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var result = await _userService.ChangePasswordAsync(
                tcKimlikNo, 
                request.CurrentPassword, 
                request.NewPassword);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Şifre sıfırla (TC Kimlik No'ya döner)
        /// </summary>
        [HttpPost("{tcKimlikNo}/reset-password")]
        public async Task<IActionResult> ResetPassword(string tcKimlikNo)
        {
            var result = await _userService.ResetPasswordAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // 🔒 HESAP YÖNETİMİ
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Kullanıcıyı kilitle
        /// </summary>
        [HttpPost("{tcKimlikNo}/lock")]
        public async Task<IActionResult> LockUser(string tcKimlikNo)
        {
            var result = await _userService.LockUserAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcı kilidini aç
        /// </summary>
        [HttpPost("{tcKimlikNo}/unlock")]
        public async Task<IActionResult> UnlockUser(string tcKimlikNo)
        {
            var result = await _userService.UnlockUserAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcıyı aktif et
        /// </summary>
        [HttpPost("{tcKimlikNo}/activate")]
        public async Task<IActionResult> ActivateUser(string tcKimlikNo)
        {
            var result = await _userService.ActivateUserAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcıyı pasif et
        /// </summary>
        [HttpPost("{tcKimlikNo}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string tcKimlikNo)
        {
            var result = await _userService.DeactivateUserAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // 🔄 OTURUM YÖNETİMİ
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Oturumu temizle
        /// </summary>
        [HttpPost("{tcKimlikNo}/clear-session")]
        public async Task<IActionResult> ClearSession(string tcKimlikNo)
        {
            var result = await _userService.ClearSessionAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Session ID ile kullanıcı getir
        /// </summary>
        [HttpGet("by-session/{sessionId}")]
        public async Task<IActionResult> GetBySessionId(string sessionId)
        {
            var result = await _userService.GetBySessionIdAsync(sessionId);
            
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        // ═══════════════════════════════════════════════════════════════════════════════
        // 🏦 BANKO MODU YÖNETİMİ
        // ═══════════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Banko modunu aktif et
        /// </summary>
        [HttpPost("{tcKimlikNo}/banko-mode/activate")]
        public async Task<IActionResult> ActivateBankoMode(string tcKimlikNo, [FromBody] int bankoId)
        {
            var result = await _userService.ActivateBankoModeAsync(tcKimlikNo, bankoId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Banko modunu deaktif et
        /// </summary>
        [HttpPost("{tcKimlikNo}/banko-mode/deactivate")]
        public async Task<IActionResult> DeactivateBankoMode(string tcKimlikNo)
        {
            var result = await _userService.DeactivateBankoModeAsync(tcKimlikNo);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Banko modu aktif mi kontrol et
        /// </summary>
        [HttpGet("{tcKimlikNo}/banko-mode/is-active")]
        public async Task<IActionResult> IsBankoModeActive(string tcKimlikNo)
        {
            var result = await _userService.IsBankoModeActiveAsync(tcKimlikNo);
            return Ok(result);
        }

        /// <summary>
        /// Aktif banko ID'sini getir
        /// </summary>
        [HttpGet("{tcKimlikNo}/banko-mode/active-banko-id")]
        public async Task<IActionResult> GetActiveBankoId(string tcKimlikNo)
        {
            var result = await _userService.GetActiveBankoIdAsync(tcKimlikNo);
            return Ok(result);
        }
    }
}

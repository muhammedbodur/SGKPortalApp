using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using System.Security.Claims;
using System.Text.Json;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    /// <summary>
    /// SignalR callback'inde cookie yazılamadığı için bu endpoint kullanılır.
    /// JS tarafından çağrılarak permissions claim'i güncellenir.
    /// </summary>
    [Authorize]
    public class RefreshPermissionsModel : PageModel
    {
        private readonly IPersonelYetkiApiService _personelYetkiApiService;
        private readonly ILogger<RefreshPermissionsModel> _logger;

        public RefreshPermissionsModel(
            IPersonelYetkiApiService personelYetkiApiService,
            ILogger<RefreshPermissionsModel> logger)
        {
            _personelYetkiApiService = personelYetkiApiService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var tcKimlikNo = User.FindFirst("TcKimlikNo")?.Value;
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    _logger.LogWarning("TcKimlikNo bulunamadı");
                    return new JsonResult(new { success = false, error = "TcKimlikNo bulunamadı" });
                }

                // DB'den güncel yetkileri çek
                var permsResult = await _personelYetkiApiService.GetByTcKimlikNoAsync(tcKimlikNo);
                if (!permsResult.Success || permsResult.Data == null)
                {
                    _logger.LogWarning("Yetkiler alınamadı: {Error}", permsResult.Message);
                    return new JsonResult(new { success = false, error = permsResult.Message });
                }

                // Dictionary'e çevir
                var permissions = permsResult.Data
                    .Where(p => !string.IsNullOrEmpty(p.PermissionKey))
                    .ToDictionary(p => p.PermissionKey, p => (int)p.YetkiSeviyesi);

                // Mevcut claims'leri al (Permissions hariç)
                var existingClaims = User.Claims
                    .Where(c => c.Type != "Permissions")
                    .Select(c => new Claim(c.Type, c.Value))
                    .ToList();

                // Yeni Permissions claim'i ekle
                var permissionsJson = JsonSerializer.Serialize(permissions);
                existingClaims.Add(new Claim("Permissions", permissionsJson));

                // Yeni ClaimsIdentity oluştur
                var claimsIdentity = new ClaimsIdentity(
                    existingClaims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                };

                // Cookie'yi yeniden yaz
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("🔑 Permissions claim güncellendi: {Count} yetki - {TcKimlikNo}", 
                    permissions.Count, tcKimlikNo);

                return new JsonResult(new { success = true, count = permissions.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshPermissions hatası");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Auth;
using System.Security.Principal;

namespace SGKPortalApp.BusinessLogicLayer.Services.Auth
{
    public class WindowsUsernameService : IWindowsUsernameService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WindowsUsernameService> _logger;

        public WindowsUsernameService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<WindowsUsernameService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string? GetWindowsUsername()
        {
            try
            {
                // 1. Önce Windows Authentication ile gelen kullanıcı adını kontrol et (domain ortamında)
                var windowsIdentity = _httpContextAccessor?.HttpContext?.User?.Identity as WindowsIdentity;
                if (windowsIdentity != null && !string.IsNullOrEmpty(windowsIdentity.Name))
                {
                    _logger.LogDebug("Windows username (Windows Auth): {Username}", windowsIdentity.Name);
                    return windowsIdentity.Name; // DOMAIN\username
                }

                // 2. Eğer Windows Auth yoksa, lokal Windows kullanıcı adını al
                var localUsername = Environment.UserName;
                if (!string.IsNullOrEmpty(localUsername))
                {
                    _logger.LogDebug("Windows username (Environment): {Username}", localUsername);
                    return localUsername; // username (domain yok)
                }

                // 3. Her şey başarısız olduysa null dön
                _logger.LogWarning("Windows username alınamadı");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Windows username alınırken hata oluştu");
                return null;
            }
        }

        public string? GetUsernameOnly()
        {
            var fullUsername = GetWindowsUsername();
            if (string.IsNullOrEmpty(fullUsername))
                return null;

            // DOMAIN\username formatındaysa, sadece username kısmını al
            if (fullUsername.Contains("\\"))
            {
                return fullUsername.Split('\\').Last();
            }

            return fullUsername;
        }

        public bool IsDomainJoined()
        {
            try
            {
                // Bilgisayarın domain adı ile machine adı farklıysa, domain'e joined'dir
                var domainName = Environment.UserDomainName;
                var machineName = Environment.MachineName;

                _logger.LogDebug("Domain check - UserDomainName: {Domain}, MachineName: {Machine}",
                    domainName, machineName);

                // Eğer farklılarsa, domain joined
                return !string.Equals(domainName, machineName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Domain kontrol edilirken hata oluştu");
                return false;
            }
        }

        public string? GetDomainName()
        {
            try
            {
                if (!IsDomainJoined())
                    return null;

                return Environment.UserDomainName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Domain adı alınırken hata oluştu");
                return null;
            }
        }
    }
}

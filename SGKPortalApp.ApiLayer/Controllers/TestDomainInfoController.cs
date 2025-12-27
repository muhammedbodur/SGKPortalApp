using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace SGKPortalApp.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDomainInfoController : ControllerBase
    {
        private readonly ILogger<TestDomainInfoController> _logger;

        public TestDomainInfoController(ILogger<TestDomainInfoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Server-side domain bilgilerini test et
        /// </summary>
        [HttpGet("server-info")]
        public IActionResult GetServerDomainInfo()
        {
            try
            {
                var info = new
                {
                    // Environment bilgileri
                    UserDomainName = Environment.UserDomainName,
                    UserName = Environment.UserName,
                    MachineName = Environment.MachineName,

                    // Windows Identity (server process)
                    WindowsIdentity = new
                    {
                        Name = WindowsIdentity.GetCurrent().Name,
                        AuthenticationType = WindowsIdentity.GetCurrent().AuthenticationType,
                        IsAuthenticated = WindowsIdentity.GetCurrent().IsAuthenticated,
                        IsSystem = WindowsIdentity.GetCurrent().IsSystem
                    },

                    // HttpContext User
                    HttpContextUser = new
                    {
                        Identity = HttpContext.User?.Identity?.Name ?? "Anonymous",
                        IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false,
                        AuthenticationType = HttpContext.User?.Identity?.AuthenticationType ?? "None"
                    },

                    // Network bilgileri
                    RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),

                    Note = "Bu bilgiler SERVER tarafında alınmıştır. Client bilgisi değildir."
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting domain info");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Active Directory query test (email ile)
        /// </summary>
        [HttpGet("ad-query")]
        public IActionResult TestActiveDirectoryQuery([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email parameter required");

                // Active Directory query için System.DirectoryServices.AccountManagement gerekli
                // Şimdilik sadece email'den varsayım yapalım

                var username = email.Split('@')[0];
                var assumedDomainUser = $"SGKDOMAIN\\{username}";

                var info = new
                {
                    Email = email,
                    AssumedDomainUser = assumedDomainUser,
                    Method = "Email-based assumption",
                    Note = "Active Directory query için ek NuGet paketi gerekli: System.DirectoryServices.AccountManagement"
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AD query test");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Client bilgilerini topla (JavaScript'ten gelecek)
        /// </summary>
        [HttpPost("client-info")]
        public IActionResult ReceiveClientInfo([FromBody] ClientInfoRequest request)
        {
            try
            {
                var info = new
                {
                    ReceivedFromClient = request,
                    ServerInfo = new
                    {
                        RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
                    },
                    Note = "Client-side'da domain bilgisi JavaScript ile alınamaz (güvenlik kısıtlaması). " +
                           "Sadece machine name veya kullanıcının manuel gireceği bilgi gönderilebilir."
                };

                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving client info");
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }

    public class ClientInfoRequest
    {
        public string? UserAgent { get; set; }
        public string? Platform { get; set; }
        public string? Language { get; set; }
        public string? ScreenResolution { get; set; }
        public string? TimeZone { get; set; }
    }
}

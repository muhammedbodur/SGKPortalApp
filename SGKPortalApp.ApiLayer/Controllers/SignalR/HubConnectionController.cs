using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;

namespace SGKPortalApp.ApiLayer.Controllers.SignalR
{
    [ApiController]
    [Route("api/hub-connections")]
    public class HubConnectionController : ControllerBase
    {
        private readonly IHubConnectionBusinessService _hubConnectionService;
        private readonly ILogger<HubConnectionController> _logger;

        public HubConnectionController(
            IHubConnectionBusinessService hubConnectionService,
            ILogger<HubConnectionController> logger)
        {
            _hubConnectionService = hubConnectionService;
            _logger = logger;
        }

        [HttpPost("connect")]
        public async Task<IActionResult> Connect([FromBody] HubConnectionRequestDto request)
        {
            var result = await _hubConnectionService.CreateOrUpdateConnectionAsync(
                request.ConnectionId, 
                request.TcKimlikNo);
            
            return result ? Ok() : BadRequest();
        }

        [HttpDelete("{connectionId}")]
        public async Task<IActionResult> Disconnect(string connectionId)
        {
            var result = await _hubConnectionService.DisconnectAsync(connectionId);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("active/{tcKimlikNo}")]
        public async Task<IActionResult> GetActiveConnections(string tcKimlikNo)
        {
            var connections = await _hubConnectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
            return Ok(connections);
        }

        [HttpPost("banko/register")]
        public async Task<IActionResult> RegisterBankoConnection([FromBody] BankoConnectionRequestDto request)
        {
            var result = await _hubConnectionService.RegisterBankoConnectionAsync(
                request.BankoId,
                request.ConnectionId,
                request.TcKimlikNo);
            
            return result ? Ok() : BadRequest();
        }

        [HttpPost("banko/deactivate")]
        public async Task<IActionResult> DeactivateBankoConnection([FromBody] string tcKimlikNo)
        {
            var result = await _hubConnectionService.DeactivateBankoConnectionAsync(tcKimlikNo);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("banko/transfer")]
        public async Task<IActionResult> TransferBankoConnection([FromBody] BankoConnectionTransferRequestDto request)
        {
            var result = await _hubConnectionService.TransferBankoConnectionAsync(request.TcKimlikNo, request.ConnectionId);
            return result ? Ok() : BadRequest("Banko bağlantısı devredilemedi");
        }

        [HttpGet("personel/{tcKimlikNo}/active-banko")]
        public async Task<IActionResult> GetPersonelActiveBanko(string tcKimlikNo)
        {
            var bankoConnection = await _hubConnectionService.GetPersonelActiveBankoAsync(tcKimlikNo);
            
            if (bankoConnection == null)
                return NotFound();
            
            // Entity'den DTO'ya dönüştür
            var dto = new HubBankoConnectionResponseDto
            {
                HubBankoConnectionId = bankoConnection.HubBankoConnectionId,
                HubConnectionId = bankoConnection.HubConnectionId,
                BankoId = bankoConnection.BankoId,
                TcKimlikNo = bankoConnection.TcKimlikNo,
                BankoModuAktif = bankoConnection.BankoModuAktif,
                BankoModuBaslangic = bankoConnection.BankoModuBaslangic,
                BankoModuBitis = bankoConnection.BankoModuBitis
            };
            
            return Ok(dto);
        }

        [HttpPost("tv/register")]
        public async Task<IActionResult> RegisterTvConnection([FromBody] TvConnectionRequestDto request)
        {
            var result = await _hubConnectionService.RegisterTvConnectionAsync(
                request.TvId,
                request.ConnectionId,
                request.TcKimlikNo);
            
            return result ? Ok() : BadRequest();
        }

        // New Banko Mode Endpoints
        
        [HttpPost("banko")]
        public async Task<IActionResult> CreateBankoConnection([FromBody] CreateBankoConnectionRequest request)
        {
            var result = await _hubConnectionService.CreateBankoConnectionAsync(
                request.HubConnectionId,
                request.BankoId,
                request.TcKimlikNo);
            
            return result ? Ok() : BadRequest("HubBankoConnection oluşturulamadı");
        }

        [HttpDelete("banko/{hubConnectionId}")]
        public async Task<IActionResult> DeactivateBankoConnectionByHubConnectionId(int hubConnectionId)
        {
            var result = await _hubConnectionService.DeactivateBankoConnectionByHubConnectionIdAsync(hubConnectionId);
            return result ? Ok() : BadRequest("Banko modundan çıkış başarısız");
        }

        [HttpGet("non-banko/{tcKimlikNo}")]
        public async Task<IActionResult> GetNonBankoConnections(string tcKimlikNo)
        {
            var connections = await _hubConnectionService.GetNonBankoConnectionsByTcKimlikNoAsync(tcKimlikNo);
            
            // Entity'den DTO'ya dönüştür
            var dtos = connections.Select(c => new HubConnectionResponseDto
            {
                HubConnectionId = c.HubConnectionId,
                TcKimlikNo = c.TcKimlikNo,
                ConnectionId = c.ConnectionId,
                ConnectionType = c.ConnectionType,
                ConnectionStatus = c.ConnectionStatus,
                ConnectedAt = c.ConnectedAt,
                LastActivityAt = c.LastActivityAt
            }).ToList();
            
            return Ok(dtos);
        }

        [HttpPut("{connectionId}/type")]
        public async Task<IActionResult> UpdateConnectionType(string connectionId, [FromBody] UpdateConnectionTypeRequest request)
        {
            var result = await _hubConnectionService.UpdateConnectionTypeAsync(connectionId, request.ConnectionType);
            return result ? Ok() : BadRequest("ConnectionType güncellenemedi");
        }

        [HttpGet("{connectionId}")]
        public async Task<IActionResult> GetByConnectionId(string connectionId)
        {
            var connection = await _hubConnectionService.GetByConnectionIdAsync(connectionId);
            
            if (connection == null)
                return NotFound();
            
            // Entity'den DTO'ya dönüştür
            var dto = new HubConnectionResponseDto
            {
                HubConnectionId = connection.HubConnectionId,
                TcKimlikNo = connection.TcKimlikNo,
                ConnectionId = connection.ConnectionId,
                ConnectionType = connection.ConnectionType,
                ConnectionStatus = connection.ConnectionStatus,
                ConnectedAt = connection.ConnectedAt,
                LastActivityAt = connection.LastActivityAt
            };
            
            return Ok(dto);
        }

        [HttpGet("banko-connection/{hubConnectionId}")]
        public async Task<IActionResult> GetBankoConnectionByHubConnectionId(int hubConnectionId)
        {
            var bankoConnection = await _hubConnectionService.GetBankoConnectionByHubConnectionIdAsync(hubConnectionId);
            
            if (bankoConnection == null)
                return NotFound();
            
            // Entity'den DTO'ya dönüştür
            var dto = new HubBankoConnectionResponseDto
            {
                HubBankoConnectionId = bankoConnection.HubBankoConnectionId,
                HubConnectionId = bankoConnection.HubConnectionId,
                BankoId = bankoConnection.BankoId,
                TcKimlikNo = bankoConnection.TcKimlikNo,
                BankoModuAktif = bankoConnection.BankoModuAktif,
                BankoModuBaslangic = bankoConnection.BankoModuBaslangic,
                BankoModuBitis = bankoConnection.BankoModuBitis
            };
            
            return Ok(dto);
        }
        
        [HttpGet("banko/{bankoId}/active-personel")]
        public async Task<IActionResult> GetBankoActivePersonel(int bankoId)
        {
            var user = await _hubConnectionService.GetBankoActivePersonelAsync(bankoId);
            
            if (user == null)
                return NotFound();
            
            // Entity'den DTO'ya dönüştür
            var dto = new UserResponseDto
            {
                TcKimlikNo = user.TcKimlikNo,
                AktifMi = user.AktifMi,
                SonGirisTarihi = user.SonGirisTarihi,
                BasarisizGirisSayisi = user.BasarisizGirisSayisi,
                HesapKilitTarihi = user.HesapKilitTarihi,
                PersonelAdSoyad = user.Personel?.AdSoyad,
                Email = user.Personel?.Email,
                CepTelefonu = user.Personel?.CepTelefonu,
                SicilNo = user.Personel?.SicilNo,
                DepartmanAdi = user.Personel?.Departman?.DepartmanAdi,
                ServisAdi = user.Personel?.Servis?.ServisAdi,
                EklenmeTarihi = user.EklenmeTarihi,
                DuzenlenmeTarihi = user.DuzenlenmeTarihi
            };
            
            return Ok(dto);
        }

        [HttpGet("banko/{bankoId}/is-in-use")]
        public async Task<IActionResult> IsBankoInUse(int bankoId)
        {
            var isInUse = await _hubConnectionService.IsBankoInUseAsync(bankoId);
            return Ok(isInUse);
        }

        // ═══════════════════════════════════════════════════════
        // TV MODE ENDPOINTS (mirroring Banko pattern)
        // ═══════════════════════════════════════════════════════

        [HttpPost("tv")]
        public async Task<IActionResult> CreateTvConnection([FromBody] CreateTvConnectionRequest request)
        {
            var result = await _hubConnectionService.CreateTvConnectionAsync(
                request.HubConnectionId,
                request.TvId,
                request.TcKimlikNo);

            return result ? Ok() : BadRequest("HubTvConnection oluşturulamadı");
        }

        [HttpDelete("tv/{hubConnectionId}")]
        public async Task<IActionResult> DeactivateTvConnectionByHubConnectionId(int hubConnectionId)
        {
            var result = await _hubConnectionService.DeactivateTvConnectionByHubConnectionIdAsync(hubConnectionId);
            return result ? Ok() : BadRequest("TV bağlantısı kapatılamadı");
        }

        [HttpPost("tv/transfer")]
        public async Task<IActionResult> TransferTvConnection([FromBody] TvConnectionTransferRequestDto request)
        {
            var result = await _hubConnectionService.TransferTvConnectionAsync(request.TcKimlikNo, request.ConnectionId);
            return result ? Ok() : BadRequest("TV bağlantısı devredilemedi");
        }

        [HttpGet("personel/{tcKimlikNo}/active-tv")]
        public async Task<IActionResult> GetPersonelActiveTv(string tcKimlikNo)
        {
            var tvConnection = await _hubConnectionService.GetActiveTvByTcKimlikNoAsync(tcKimlikNo);

            if (tvConnection == null)
                return NotFound();

            // Entity'den DTO'ya dönüştür
            var dto = new HubTvConnectionResponseDto
            {
                HubTvConnectionId = tvConnection.HubTvConnectionId,
                HubConnectionId = tvConnection.HubConnectionId,
                TvId = tvConnection.TvId,
                TcKimlikNo = tvConnection.HubConnection?.TcKimlikNo ?? string.Empty
            };

            return Ok(dto);
        }

        [HttpGet("tv/{tvId}/active-user")]
        public async Task<IActionResult> GetTvActiveUser(int tvId)
        {
            var user = await _hubConnectionService.GetTvActiveUserAsync(tvId);

            if (user == null)
                return NotFound();

            // Entity'den DTO'ya dönüştür
            var dto = new UserResponseDto
            {
                TcKimlikNo = user.TcKimlikNo,
                AktifMi = user.AktifMi,
                SonGirisTarihi = user.SonGirisTarihi,
                BasarisizGirisSayisi = user.BasarisizGirisSayisi,
                HesapKilitTarihi = user.HesapKilitTarihi,
                PersonelAdSoyad = user.Personel?.AdSoyad,
                Email = user.Personel?.Email,
                CepTelefonu = user.Personel?.CepTelefonu,
                SicilNo = user.Personel?.SicilNo,
                DepartmanAdi = user.Personel?.Departman?.DepartmanAdi,
                ServisAdi = user.Personel?.Servis?.ServisAdi,
                EklenmeTarihi = user.EklenmeTarihi,
                DuzenlenmeTarihi = user.DuzenlenmeTarihi
            };

            return Ok(dto);
        }

        [HttpGet("tv/{tvId}/is-in-use")]
        public async Task<IActionResult> IsTvInUse(int tvId)
        {
            var isInUse = await _hubConnectionService.IsTvInUseByTvUserAsync(tvId);
            return Ok(isInUse);
        }

        [HttpGet("non-tv/{tcKimlikNo}")]
        public async Task<IActionResult> GetNonTvConnections(string tcKimlikNo)
        {
            var connections = await _hubConnectionService.GetNonTvConnectionsByTcKimlikNoAsync(tcKimlikNo);

            // Entity'den DTO'ya dönüştür
            var dtos = connections.Select(c => new HubConnectionResponseDto
            {
                HubConnectionId = c.HubConnectionId,
                TcKimlikNo = c.TcKimlikNo,
                ConnectionId = c.ConnectionId,
                ConnectionType = c.ConnectionType,
                ConnectionStatus = c.ConnectionStatus,
                ConnectedAt = c.ConnectedAt,
                LastActivityAt = c.LastActivityAt
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("tv-connection/{hubConnectionId}")]
        public async Task<IActionResult> GetTvConnectionByHubConnectionId(int hubConnectionId)
        {
            var tvConnection = await _hubConnectionService.GetTvConnectionByHubConnectionIdAsync(hubConnectionId);

            if (tvConnection == null)
                return NotFound();

            // Entity'den DTO'ya dönüştür
            var dto = new HubTvConnectionResponseDto
            {
                HubTvConnectionId = tvConnection.HubTvConnectionId,
                HubConnectionId = tvConnection.HubConnectionId,
                TvId = tvConnection.TvId,
                TcKimlikNo = tvConnection.HubConnection?.TcKimlikNo ?? string.Empty
            };

            return Ok(dto);
        }

        [HttpGet("tv/{tvId}/is-in-use-by-other/{currentTcKimlikNo}")]
        public async Task<IActionResult> IsTvInUseByOtherTvUser(int tvId, string currentTcKimlikNo)
        {
            var isInUse = await _hubConnectionService.IsTvInUseByOtherTvUserAsync(tvId, currentTcKimlikNo);
            return Ok(isInUse);
        }

        // ═══════════════════════════════════════════════════════
        // CLEANUP ENDPOINTS (Background Service için)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Uygulama başlangıcında tüm online connection'ları offline yapar
        /// Frontend BackgroundService tarafından çağrılır
        /// </summary>
        [HttpPost("cleanup/startup")]
        [AllowAnonymous] // BackgroundService'ten authentication olmadan çağrılıyor
        public async Task<IActionResult> CleanupAllOnStartup()
        {
            try
            {
                var count = await _hubConnectionService.CleanupAllOnStartupAsync();
                _logger.LogInformation("Cleanup startup: {Count} connection temizlendi", count);
                return Ok(new { cleanedCount = count, message = $"{count} connection offline yapıldı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup startup hatası");
                return StatusCode(500, new { message = "Cleanup startup hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Stale connection'ları temizle
        /// Frontend BackgroundService tarafından periyodik olarak çağrılır
        /// </summary>
        /// <param name="staleThresholdMinutes">Stale kabul edilme süresi (dakika). Default: 10</param>
        [HttpPost("cleanup/stale")]
        [AllowAnonymous] // BackgroundService'ten authentication olmadan çağrılıyor
        public async Task<IActionResult> CleanupStaleConnections([FromQuery] int staleThresholdMinutes = 10)
        {
            try
            {
                var count = await _hubConnectionService.CleanupStaleConnectionsAsync(staleThresholdMinutes);
                _logger.LogDebug("Cleanup stale: {Count} connection temizlendi", count);
                return Ok(new { cleanedCount = count, message = $"{count} stale connection offline yapıldı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup stale hatası");
                return StatusCode(500, new { message = "Cleanup stale hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Orphan HubBankoConnection kayıtlarını temizle
        /// HubConnection offline/silinmiş ama HubBankoConnection aktif olanları temizler
        /// Frontend BackgroundService tarafından periyodik olarak çağrılır
        /// </summary>
        [HttpPost("cleanup/orphan-banko")]
        [AllowAnonymous] // BackgroundService'ten authentication olmadan çağrılıyor
        public async Task<IActionResult> CleanupOrphanBankoConnections()
        {
            try
            {
                var count = await _hubConnectionService.CleanupOrphanBankoConnectionsAsync();
                if (count > 0)
                {
                    _logger.LogInformation("Cleanup orphan banko: {Count} kayıt temizlendi", count);
                }
                return Ok(new { cleanedCount = count, message = $"{count} orphan HubBankoConnection temizlendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup orphan banko hatası");
                return StatusCode(500, new { message = "Cleanup orphan banko hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Orphan HubTvConnection kayıtlarını temizle
        /// HubConnection offline/silinmiş ama HubTvConnection aktif olanları temizler
        /// Frontend BackgroundService tarafından periyodik olarak çağrılır
        /// </summary>
        [HttpPost("cleanup/orphan-tv")]
        [AllowAnonymous] // BackgroundService'ten authentication olmadan çağrılıyor
        public async Task<IActionResult> CleanupOrphanTvConnections()
        {
            try
            {
                var count = await _hubConnectionService.CleanupOrphanTvConnectionsAsync();
                if (count > 0)
                {
                    _logger.LogInformation("Cleanup orphan TV: {Count} kayıt temizlendi", count);
                }
                return Ok(new { cleanedCount = count, message = $"{count} orphan HubTvConnection temizlendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup orphan TV hatası");
                return StatusCode(500, new { message = "Cleanup orphan TV hatası", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateBankoConnectionRequest
    {
        public int HubConnectionId { get; set; }
        public int BankoId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
    }

    // TV Request DTOs
    public class CreateTvConnectionRequest
    {
        public int HubConnectionId { get; set; }
        public int TvId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
    }
}


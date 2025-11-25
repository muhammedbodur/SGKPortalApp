using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;

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

        [HttpPost("tv/register")]
        public async Task<IActionResult> RegisterTvConnection([FromBody] TvConnectionRequestDto request)
        {
            var result = await _hubConnectionService.RegisterTvConnectionAsync(
                request.TvId,
                request.ConnectionId,
                request.TcKimlikNo);
            
            return result ? Ok() : BadRequest();
        }
    }
}

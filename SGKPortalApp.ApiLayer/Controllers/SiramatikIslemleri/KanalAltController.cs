using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalAltController : ControllerBase
    {
        private readonly IKanalAltService _kanalAltService;
        private readonly ILogger<KanalAltController> _logger;

        public KanalAltController(IKanalAltService kanalAltService, ILogger<KanalAltController> logger)
        {
            _kanalAltService = kanalAltService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kanalAltService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kanalAltService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _kanalAltService.GetActiveAsync();
            return Ok(result);
        }

        [HttpGet("kanal/{kanalId}")]
        public async Task<IActionResult> GetByKanalId(int kanalId)
        {
            var result = await _kanalAltService.GetByKanalIdAsync(kanalId);
            return Ok(result);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            var result = await _kanalAltService.GetWithDetailsAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KanalAltKanalCreateRequestDto request)
        {
            var result = await _kanalAltService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KanalAltKanalUpdateRequestDto request)
        {
            var result = await _kanalAltService.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kanalAltService.DeleteAsync(id);
            return Ok(result);
        }
    }
}

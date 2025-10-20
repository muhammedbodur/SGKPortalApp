using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalController : ControllerBase
    {
        private readonly IKanalService _kanalService;
        private readonly ILogger<KanalController> _logger;

        public KanalController(
            IKanalService kanalService,
            ILogger<KanalController> logger)
        {
            _kanalService = kanalService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm kanalları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kanalService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kanal getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kanalService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni kanal oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KanalCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalService.CreateAsync(request);
            return result.Success 
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.KanalId }, result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Kanal günceller
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] KanalUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal siler (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kanalService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif kanalları getirir
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _kanalService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

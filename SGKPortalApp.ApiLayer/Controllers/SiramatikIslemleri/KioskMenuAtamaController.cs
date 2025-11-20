using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/kiosk-menu-atama")]
    public class KioskMenuAtamaController : ControllerBase
    {
        private readonly IKioskMenuAtamaService _kioskMenuAtamaService;
        private readonly ILogger<KioskMenuAtamaController> _logger;

        public KioskMenuAtamaController(
            IKioskMenuAtamaService kioskMenuAtamaService,
            ILogger<KioskMenuAtamaController> logger)
        {
            _kioskMenuAtamaService = kioskMenuAtamaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kioskMenuAtamaService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("kiosk/{kioskId:int}")]
        public async Task<IActionResult> GetByKiosk(int kioskId)
        {
            var result = await _kioskMenuAtamaService.GetByKioskAsync(kioskId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("kiosk/{kioskId:int}/active")]
        public async Task<IActionResult> GetActiveByKiosk(int kioskId)
        {
            var result = await _kioskMenuAtamaService.GetActiveByKioskAsync(kioskId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kioskMenuAtamaService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KioskMenuAtamaCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kioskMenuAtamaService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] KioskMenuAtamaUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.KioskMenuAtamaId)
                return BadRequest("ID mismatch");

            var result = await _kioskMenuAtamaService.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kioskMenuAtamaService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

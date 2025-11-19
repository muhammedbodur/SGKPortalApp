using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KioskController : ControllerBase
    {
        private readonly IKioskService _kioskService;
        private readonly ILogger<KioskController> _logger;

        public KioskController(
            IKioskService kioskService,
            ILogger<KioskController> logger)
        {
            _kioskService = kioskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kioskService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kioskService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:int}/with-menu")]
        public async Task<IActionResult> GetWithMenu(int id)
        {
            var result = await _kioskService.GetWithMenuAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("aktif")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _kioskService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("bina/{binaId:int}")]
        public async Task<IActionResult> GetByHizmetBinasi(int binaId)
        {
            var result = await _kioskService.GetByHizmetBinasiAsync(binaId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KioskCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kioskService.CreateAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.KioskId }, result)
                : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] KioskUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kioskService.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kioskService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

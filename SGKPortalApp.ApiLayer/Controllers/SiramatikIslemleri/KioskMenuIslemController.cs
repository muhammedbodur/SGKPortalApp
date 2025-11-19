using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/kiosk-menu-islem")]
    public class KioskMenuIslemController : ControllerBase
    {
        private readonly IKioskMenuIslemService _kioskMenuIslemService;
        private readonly ILogger<KioskMenuIslemController> _logger;

        public KioskMenuIslemController(
            IKioskMenuIslemService kioskMenuIslemService,
            ILogger<KioskMenuIslemController> logger)
        {
            _kioskMenuIslemService = kioskMenuIslemService;
            _logger = logger;
        }

        [HttpGet("menu/{kioskMenuId:int}")]
        public async Task<IActionResult> GetByKioskMenu(int kioskMenuId)
        {
            var result = await _kioskMenuIslemService.GetByKioskMenuAsync(kioskMenuId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kioskMenuIslemService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KioskMenuIslemCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kioskMenuIslemService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] KioskMenuIslemUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kioskMenuIslemService.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kioskMenuIslemService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/sira/{yeniSira:int}")]
        public async Task<IActionResult> UpdateSira(int id, int yeniSira)
        {
            var result = await _kioskMenuIslemService.UpdateSiraAsync(id, yeniSira);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalPersonelController : ControllerBase
    {
        private readonly IKanalPersonelService _kanalPersonelService;
        private readonly ILogger<KanalPersonelController> _logger;

        public KanalPersonelController(
            IKanalPersonelService kanalPersonelService,
            ILogger<KanalPersonelController> logger)
        {
            _kanalPersonelService = kanalPersonelService;
            _logger = logger;
        }

        /// <summary>
        /// Hizmet binasına göre kanal personel atamalarını getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasiId(int hizmetBinasiId)
        {
            var result = await _kanalPersonelService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personele göre kanal atamalarını getirir
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}")]
        public async Task<IActionResult> GetByPersonelTc(string tcKimlikNo)
        {
            var result = await _kanalPersonelService.GetByPersonelTcAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal alt işlemine göre personel atamalarını getirir
        /// </summary>
        [HttpGet("kanal-alt-islem/{kanalAltIslemId:int}")]
        public async Task<IActionResult> GetByKanalAltIslemId(int kanalAltIslemId)
        {
            var result = await _kanalPersonelService.GetByKanalAltIslemIdAsync(kanalAltIslemId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kanal personel atamasını getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kanalPersonelService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni kanal personel ataması oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KanalPersonelCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalPersonelService.CreateAsync(request);
            return result.Success 
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.KanalPersonelId }, result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Kanal personel atamasını günceller
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] KanalPersonelUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalPersonelService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal personel atamasını siler (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kanalPersonelService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalIslemController : ControllerBase
    {
        private readonly IKanalIslemService _kanalIslemService;
        private readonly ILogger<KanalIslemController> _logger;

        public KanalIslemController(
            IKanalIslemService kanalIslemService,
            ILogger<KanalIslemController> logger)
        {
            _kanalIslemService = kanalIslemService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm kanal işlemleri getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kanalIslemService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kanal işlem getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _kanalIslemService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni kanal işlem oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KanalIslemCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalIslemService.CreateAsync(request);
            return result.Success 
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.KanalIslemId }, result) 
                : BadRequest(result);
        }

        /// <summary>
        /// Kanal işlem günceller
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] KanalIslemUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _kanalIslemService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal işlem siler (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _kanalIslemService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif kanal işlemleri getirir
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _kanalIslemService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal ID'ye göre kanal işlemleri getirir
        /// </summary>
        [HttpGet("kanal/{kanalId:int}")]
        public async Task<IActionResult> GetByKanalId(int kanalId)
        {
            var result = await _kanalIslemService.GetByKanalIdAsync(kanalId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Hizmet binası ID'ye göre kanal işlemleri getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasiId(int hizmetBinasiId)
        {
            var result = await _kanalIslemService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

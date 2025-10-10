using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/atanma-nedeni")]
    public class AtanmaNedeniController : ControllerBase
    {
        private readonly IAtanmaNedeniService _atanmaNedeniService;
        private readonly ILogger<AtanmaNedeniController> _logger;

        public AtanmaNedeniController(
            IAtanmaNedeniService atanmaNedeniService,
            ILogger<AtanmaNedeniController> logger)
        {
            _atanmaNedeniService = atanmaNedeniService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _atanmaNedeniService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _atanmaNedeniService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AtanmaNedeniCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _atanmaNedeniService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.AtanmaNedeniId }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AtanmaNedeniUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _atanmaNedeniService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _atanmaNedeniService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

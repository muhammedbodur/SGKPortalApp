using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class GununMenusuController : ControllerBase
    {
        private readonly IGununMenusuService _gununMenusuService;
        private readonly ILogger<GununMenusuController> _logger;

        public GununMenusuController(
            IGununMenusuService gununMenusuService,
            ILogger<GununMenusuController> logger)
        {
            _gununMenusuService = gununMenusuService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _gununMenusuService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _gununMenusuService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var result = await _gununMenusuService.GetByDateAsync(date);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GununMenusuCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gununMenusuService.CreateAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.MenuId }, result)
                : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GununMenusuUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _gununMenusuService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _gununMenusuService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] GununMenusuFilterRequestDto filter)
        {
            var result = await _gununMenusuService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

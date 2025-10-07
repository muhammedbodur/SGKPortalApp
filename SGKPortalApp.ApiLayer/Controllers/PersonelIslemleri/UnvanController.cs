using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnvanController : ControllerBase
    {
        private readonly IUnvanService _unvanService;
        private readonly ILogger<UnvanController> _logger;

        public UnvanController(
            IUnvanService unvanService,
            ILogger<UnvanController> logger)
        {
            _unvanService = unvanService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _unvanService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unvanService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UnvanCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unvanService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.UnvanId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UnvanUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unvanService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unvanService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _unvanService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] UnvanFilterRequestDto filter)
        {
            var result = await _unvanService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}/personel-count")]
        public async Task<IActionResult> GetPersonelCount(int id)
        {
            var result = await _unvanService.GetPersonelCountAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _unvanService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
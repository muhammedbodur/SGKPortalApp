using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class YetkiController : ControllerBase
    {
        private readonly IYetkiService _yetkiService;
        private readonly ILogger<YetkiController> _logger;

        public YetkiController(
            IYetkiService yetkiService,
            ILogger<YetkiController> logger)
        {
            _yetkiService = yetkiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _yetkiService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _yetkiService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] YetkiCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _yetkiService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.YetkiId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] YetkiUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _yetkiService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _yetkiService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRoot()
        {
            var result = await _yetkiService.GetRootAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{ustYetkiId:int}/children")]
        public async Task<IActionResult> GetChildren(int ustYetkiId)
        {
            var result = await _yetkiService.GetChildrenAsync(ustYetkiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _yetkiService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModulControllerController : ControllerBase
    {
        private readonly IModulControllerService _modulControllerService;
        private readonly ILogger<ModulControllerController> _logger;

        public ModulControllerController(
            IModulControllerService modulControllerService,
            ILogger<ModulControllerController> logger)
        {
            _modulControllerService = modulControllerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _modulControllerService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _modulControllerService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-modul/{modulId:int}")]
        public async Task<IActionResult> GetByModulId(int modulId)
        {
            var result = await _modulControllerService.GetByModulIdAsync(modulId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ModulControllerCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _modulControllerService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.ModulControllerId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ModulControllerUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _modulControllerService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _modulControllerService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _modulControllerService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown/by-modul/{modulId:int}")]
        public async Task<IActionResult> GetDropdownByModulId(int modulId)
        {
            var result = await _modulControllerService.GetDropdownByModulIdAsync(modulId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

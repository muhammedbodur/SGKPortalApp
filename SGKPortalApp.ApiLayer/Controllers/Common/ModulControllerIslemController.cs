using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModulControllerIslemController : ControllerBase
    {
        private readonly IModulControllerIslemService _modulControllerIslemService;
        private readonly ILogger<ModulControllerIslemController> _logger;

        public ModulControllerIslemController(
            IModulControllerIslemService modulControllerIslemService,
            ILogger<ModulControllerIslemController> logger)
        {
            _modulControllerIslemService = modulControllerIslemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _modulControllerIslemService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _modulControllerIslemService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-controller/{controllerId:int}")]
        public async Task<IActionResult> GetByControllerId(int controllerId)
        {
            var result = await _modulControllerIslemService.GetByControllerIdAsync(controllerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ModulControllerIslemCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _modulControllerIslemService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.ModulControllerIslemId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ModulControllerIslemUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _modulControllerIslemService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _modulControllerIslemService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _modulControllerIslemService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown/by-controller/{controllerId:int}")]
        public async Task<IActionResult> GetDropdownByControllerId(int controllerId)
        {
            var result = await _modulControllerIslemService.GetDropdownByControllerIdAsync(controllerId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

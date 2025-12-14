using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

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

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _modulControllerIslemService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

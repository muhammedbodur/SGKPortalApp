using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonelYetkiController : ControllerBase
    {
        private readonly IPersonelYetkiService _personelYetkiService;
        private readonly ILogger<PersonelYetkiController> _logger;

        public PersonelYetkiController(
            IPersonelYetkiService personelYetkiService,
            ILogger<PersonelYetkiController> logger)
        {
            _personelYetkiService = personelYetkiService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _personelYetkiService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-tc/{tcKimlikNo}")]
        public async Task<IActionResult> GetByTcKimlikNo(string tcKimlikNo)
        {
            var result = await _personelYetkiService.GetByTcKimlikNoAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-islem/{modulControllerIslemId:int}")]
        public async Task<IActionResult> GetByModulControllerIslemId(int modulControllerIslemId)
        {
            var result = await _personelYetkiService.GetByModulControllerIslemIdAsync(modulControllerIslemId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonelYetkiCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _personelYetkiService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.PersonelYetkiId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonelYetkiUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _personelYetkiService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _personelYetkiService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

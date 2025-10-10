using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendikaController : ControllerBase
    {
        private readonly ISendikaService _sendikaService;
        private readonly ILogger<SendikaController> _logger;

        public SendikaController(
            ISendikaService sendikaService,
            ILogger<SendikaController> logger)
        {
            _sendikaService = sendikaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sendikaService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sendikaService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SendikaCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sendikaService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.SendikaId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SendikaUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _sendikaService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sendikaService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

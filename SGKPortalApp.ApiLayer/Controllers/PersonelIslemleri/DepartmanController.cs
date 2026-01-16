using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmanController : ControllerBase
    {
        private readonly IDepartmanService _departmanService;
        private readonly ILogger<DepartmanController> _logger;

        public DepartmanController(
            IDepartmanService departmanService,
            ILogger<DepartmanController> logger)
        {
            _departmanService = departmanService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmanService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tüm departman listesi (dropdown/select için)
        /// </summary>
        [HttpGet("liste")]
        public async Task<IActionResult> GetListe()
        {
            var result = await _departmanService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _departmanService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmanCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmanService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.DepartmanId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmanUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmanService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmanService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _departmanService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] DepartmanFilterRequestDto filter)
        {
            var result = await _departmanService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}/personel-count")]
        public async Task<IActionResult> GetPersonelCount(int id)
        {
            var result = await _departmanService.GetPersonelCountAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmanHizmetBinasiController : ControllerBase
    {
        private readonly IDepartmanHizmetBinasiService _service;
        private readonly ILogger<DepartmanHizmetBinasiController> _logger;

        public DepartmanHizmetBinasiController(
            IDepartmanHizmetBinasiService service,
            ILogger<DepartmanHizmetBinasiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("departman/{departmanId}")]
        public async Task<IActionResult> GetByDepartman(int departmanId)
        {
            var result = await _service.GetByDepartmanAsync(departmanId);
            return Ok(result);
        }

        [HttpGet("hizmet-binasi/{hizmetBinasiId}")]
        public async Task<IActionResult> GetByHizmetBinasi(int hizmetBinasiId)
        {
            var result = await _service.GetByHizmetBinasiAsync(hizmetBinasiId);
            return Ok(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _service.GetAllForDropdownAsync();
            return Ok(result);
        }

        [HttpGet("dropdown/departman/{departmanId}")]
        public async Task<IActionResult> GetDropdownByDepartman(int departmanId)
        {
            var result = await _service.GetDropdownByDepartmanAsync(departmanId);
            return Ok(result);
        }

        [HttpGet("dropdown/hizmet-binasi/{hizmetBinasiId}")]
        public async Task<IActionResult> GetDropdownByHizmetBinasi(int hizmetBinasiId)
        {
            var result = await _service.GetDropdownByHizmetBinasiAsync(hizmetBinasiId);
            return Ok(result);
        }

        /// <summary>
        /// Departman ve Hizmet Binası kombinasyonundan DepartmanHizmetBinasiId bulur
        /// </summary>
        [HttpGet("find/{departmanId}/{hizmetBinasiId}")]
        public async Task<IActionResult> GetDepartmanHizmetBinasiId(int departmanId, int hizmetBinasiId)
        {
            var result = await _service.GetDepartmanHizmetBinasiIdAsync(departmanId, hizmetBinasiId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmanHizmetBinasiCreateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmanHizmetBinasiUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.UpdateAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }
    }
}

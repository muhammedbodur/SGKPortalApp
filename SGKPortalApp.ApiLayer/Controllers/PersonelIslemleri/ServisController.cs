using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServisController : ControllerBase
    {
        private readonly IServisService _servisService;
        private readonly ILogger<ServisController> _logger;

        public ServisController(
            IServisService servisService,
            ILogger<ServisController> logger)
        {
            _servisService = servisService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _servisService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tüm servis listesi (dropdown/select için)
        /// </summary>
        [HttpGet("liste")]
        public async Task<IActionResult> GetListe()
        {
            var result = await _servisService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu servis listesi (authorization ile filtrelenmiş)
        /// </summary>
        [HttpGet("yetkili-liste")]
        public async Task<IActionResult> GetYetkiliListe()
        {
            // Get user's servis from claims
            var servisIdClaim = User.FindFirst("ServisId")?.Value;

            var result = await _servisService.GetActiveAsync();

            if (!result.Success)
                return BadRequest(result);

            // If user has specific servis, filter to only that servis
            if (!string.IsNullOrEmpty(servisIdClaim) && int.TryParse(servisIdClaim, out var userServisId))
            {
                var filteredData = result.Data?.Where(s => s.ServisId == userServisId).ToList();
                result.Data = filteredData ?? new List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri.ServisResponseDto>();
            }
            // If user has no specific servis, return all active servis (admin or no servis assigned)

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _servisService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServisCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servisService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.ServisId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServisUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servisService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servisService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _servisService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] ServisFilterRequestDto filter)
        {
            var result = await _servisService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}/personel-count")]
        public async Task<IActionResult> GetPersonelCount(int id)
        {
            var result = await _servisService.GetPersonelCountAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _servisService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
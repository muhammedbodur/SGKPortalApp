using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/personel")]
    public class PersonelController : ControllerBase
    {
        private readonly IPersonelService _personelService;
        private readonly ILogger<PersonelController> _logger;

        public PersonelController(
            IPersonelService personelService,
            ILogger<PersonelController> logger)
        {
            _personelService = personelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personelService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{tcKimlikNo}")]
        public async Task<IActionResult> GetByTcKimlikNo(string tcKimlikNo)
        {
            var result = await _personelService.GetByTcKimlikNoAsync(tcKimlikNo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{tcKimlikNo}")]
        public async Task<IActionResult> Update(string tcKimlikNo, [FromBody] PersonelUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _personelService.UpdateAsync(tcKimlikNo, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{tcKimlikNo}")]
        public async Task<IActionResult> Delete(string tcKimlikNo)
        {
            var result = await _personelService.DeleteAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _personelService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] PersonelFilterRequestDto filter)
        {
            var result = await _personelService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("departman/{departmanId:int}")]
        public async Task<IActionResult> GetByDepartman(int departmanId)
        {
            var result = await _personelService.GetByDepartmanAsync(departmanId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("servis/{servisId:int}")]
        public async Task<IActionResult> GetByServis(int servisId)
        {
            var result = await _personelService.GetByServisAsync(servisId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Toplu kayıt endpoint'leri (Transaction)
        [HttpPost("complete")]
        public async Task<IActionResult> CreateComplete([FromBody] PersonelCompleteRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _personelService.CreateCompleteAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetByTcKimlikNo), new { tcKimlikNo = result.Data?.TcKimlikNo }, result) : BadRequest(result);
        }

        [HttpPut("{tcKimlikNo}/complete")]
        public async Task<IActionResult> UpdateComplete(string tcKimlikNo, [FromBody] PersonelCompleteRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _personelService.UpdateCompleteAsync(tcKimlikNo, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

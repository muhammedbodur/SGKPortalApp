using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [ApiController]
    [Route("api/resmitatil")]
    public class ResmiTatilController : ControllerBase
    {
        private readonly IResmiTatilService _resmiTatilService;
        private readonly ILogger<ResmiTatilController> _logger;

        public ResmiTatilController(
            IResmiTatilService resmiTatilService,
            ILogger<ResmiTatilController> logger)
        {
            _resmiTatilService = resmiTatilService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _resmiTatilService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _resmiTatilService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("year/{year:int}")]
        public async Task<IActionResult> GetByYear(int year)
        {
            var result = await _resmiTatilService.GetByYearAsync(year);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResmiTatilCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _resmiTatilService.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.TatilId }, result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ResmiTatilUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.TatilId)
                return BadRequest("ID uyuşmazlığı");

            var result = await _resmiTatilService.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _resmiTatilService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("is-holiday")]
        public async Task<IActionResult> IsHoliday([FromQuery] DateTime date)
        {
            var result = await _resmiTatilService.IsHolidayAsync(date);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("holiday-name")]
        public async Task<IActionResult> GetHolidayName([FromQuery] DateTime date)
        {
            var result = await _resmiTatilService.GetHolidayNameAsync(date);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncHolidaysFromGoogleCalendar([FromBody] ResmiTatilSyncRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _resmiTatilService.SyncHolidaysFromGoogleCalendarAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

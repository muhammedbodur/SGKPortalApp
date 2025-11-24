using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [Route("api/[controller]")]
    [ApiController]
    public class TvController : ControllerBase
    {
        private readonly ITvService _tvService;

        public TvController(ITvService tvService)
        {
            _tvService = tvService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tvService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _tvService.GetWithDetailsAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            var result = await _tvService.GetWithDetailsAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("byhizmetbinasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasi(int hizmetBinasiId)
        {
            var result = await _tvService.GetByHizmetBinasiAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("bykattipi/{katTipi:int}")]
        public async Task<IActionResult> GetByKatTipi(KatTipi katTipi)
        {
            var result = await _tvService.GetByKatTipiAsync(katTipi);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _tvService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            var result = await _tvService.GetDropdownAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("dropdown/byhizmetbinasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasiDropdown(int hizmetBinasiId)
        {
            var result = await _tvService.GetByHizmetBinasiDropdownAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TvCreateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _tvService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TvUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.TvId)
            {
                return BadRequest("ID uyuşmazlığı");
            }

            var result = await _tvService.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tvService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // TV-BANKO İLİŞKİLERİ
        // ═══════════════════════════════════════════════════════

        [HttpPost("{tvId:int}/banko/{bankoId:int}")]
        public async Task<IActionResult> AddBankoToTv(int tvId, int bankoId)
        {
            var result = await _tvService.AddBankoToTvAsync(tvId, bankoId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{tvId:int}/banko/{bankoId:int}")]
        public async Task<IActionResult> RemoveBankoFromTv(int tvId, int bankoId)
        {
            var result = await _tvService.RemoveBankoFromTvAsync(tvId, bankoId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

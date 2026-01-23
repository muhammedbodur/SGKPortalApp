using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankoController : ControllerBase
    {
        private readonly IBankoService _bankoService;
        private readonly ILogger<BankoController> _logger;

        public BankoController(
            IBankoService bankoService,
            ILogger<BankoController> logger)
        {
            _bankoService = bankoService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tüm bankoları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bankoService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre banko getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bankoService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni banko oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BankoCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bankoService.CreateAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.BankoId }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Banko günceller
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BankoUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bankoService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Banko siler (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bankoService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // QUERY OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Departman-Hizmet binasına göre bankoları getirir
        /// </summary>
        [HttpGet("departman-hizmet-binasi/{dhbId:int}")]
        public async Task<IActionResult> GetByDepartmanHizmetBinasi(int dhbId)
        {
            var result = await _bankoService.GetByDepartmanHizmetBinasiAsync(dhbId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Departman-Hizmet binasına göre kat gruplu bankoları getirir
        /// </summary>
        [HttpGet("departman-hizmet-binasi/{dhbId:int}/grouped")]
        public async Task<IActionResult> GetGroupedByKat(int dhbId)
        {
            var result = await _bankoService.GetGroupedByKatAsync(dhbId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Departman-Hizmet binasındaki boş bankoları getirir
        /// </summary>
        [HttpGet("departman-hizmet-binasi/{dhbId:int}/bos")]
        public async Task<IActionResult> GetAvailableBankos(int dhbId)
        {
            var result = await _bankoService.GetAvailableBankosAsync(dhbId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif bankoları getirir
        /// </summary>
        [HttpGet("aktif")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _bankoService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL ATAMA OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personeli bankoya atar
        /// </summary>
        [HttpPost("ata")]
        public async Task<IActionResult> AssignPersonelToBanko([FromBody] BankoPersonelAtaDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bankoService.AssignPersonelToBankoAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personeli bankodan çıkarır
        /// </summary>
        [HttpDelete("personel/{tcKimlikNo}")]
        public async Task<IActionResult> UnassignPersonelFromBanko(string tcKimlikNo)
        {
            var result = await _bankoService.UnassignPersonelFromBankoAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Bankoyu boşaltır
        /// </summary>
        [HttpDelete("{bankoId:int}/bosalt")]
        public async Task<IActionResult> UnassignBanko(int bankoId)
        {
            var result = await _bankoService.UnassignBankoAsync(bankoId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personelin şu anki bankosunu getirir
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}/banko")]
        public async Task<IActionResult> GetPersonelCurrentBanko(string tcKimlikNo)
        {
            var result = await _bankoService.GetPersonelCurrentBankoAsync(tcKimlikNo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ═══════════════════════════════════════════════════════
        // AKTIFLIK OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Banko aktiflik durumunu değiştirir
        /// </summary>
        [HttpPatch("{id:int}/toggle-aktiflik")]
        public async Task<IActionResult> ToggleAktiflik(int id)
        {
            var result = await _bankoService.ToggleAktiflikAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

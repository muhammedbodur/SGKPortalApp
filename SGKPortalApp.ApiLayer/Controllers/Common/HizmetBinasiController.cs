using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class HizmetBinasiController : ControllerBase
    {
        private readonly IHizmetBinasiService _hizmetBinasiService;
        private readonly ILogger<HizmetBinasiController> _logger;

        public HizmetBinasiController(
            IHizmetBinasiService hizmetBinasiService,
            ILogger<HizmetBinasiController> logger)
        {
            _hizmetBinasiService = hizmetBinasiService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm hizmet binalarını getirir
        /// </summary>
        /// <returns>Hizmet binaları listesi</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GetAll endpoint çağrıldı");
            var result = await _hizmetBinasiService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre hizmet binası getirir
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Hizmet binası detayı</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("GetById endpoint çağrıldı. ID: {Id}", id);
            var result = await _hizmetBinasiService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre hizmet binası detayını getirir (personeller ve bankolar ile)
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Hizmet binası detayı</returns>
        [HttpGet("{id:int}/detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetailById(int id)
        {
            _logger.LogInformation("GetDetailById endpoint çağrıldı. ID: {Id}", id);
            var result = await _hizmetBinasiService.GetDetailByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Aktif hizmet binalarını getirir
        /// </summary>
        /// <returns>Aktif hizmet binaları listesi</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActive()
        {
            _logger.LogInformation("GetActive endpoint çağrıldı");
            var result = await _hizmetBinasiService.GetActiveAsync();
            return Ok(result);
        }

        /// <summary>
        /// Departmana göre hizmet binalarını getirir
        /// </summary>
        /// <param name="departmanId">Departman ID</param>
        /// <returns>Hizmet binaları listesi</returns>
        [HttpGet("by-departman/{departmanId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDepartman(int departmanId)
        {
            _logger.LogInformation("GetByDepartman endpoint çağrıldı. Departman ID: {DepartmanId}", departmanId);
            var result = await _hizmetBinasiService.GetByDepartmanAsync(departmanId);
            return Ok(result);
        }

        /// <summary>
        /// Hizmet binasındaki personel sayısını getirir
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Personel sayısı</returns>
        [HttpGet("{id:int}/personel-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonelCount(int id)
        {
            _logger.LogInformation("GetPersonelCount endpoint çağrıldı. ID: {Id}", id);
            var result = await _hizmetBinasiService.GetPersonelCountAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Yeni hizmet binası oluşturur
        /// </summary>
        /// <param name="request">Hizmet binası bilgileri</param>
        /// <returns>Oluşturulan hizmet binası</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] HizmetBinasiCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Create endpoint çağrıldı. Hizmet Binası Adı: {Ad}", request.HizmetBinasiAdi);

            var result = await _hizmetBinasiService.CreateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data?.HizmetBinasiId },
                result);
        }

        /// <summary>
        /// Hizmet binası günceller
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <param name="request">Güncellenmiş hizmet binası bilgileri</param>
        /// <returns>Güncellenmiş hizmet binası</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] HizmetBinasiUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Update endpoint çağrıldı. ID: {Id}", id);

            var result = await _hizmetBinasiService.UpdateAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Hizmet binası siler
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Silme işlemi sonucu</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete endpoint çağrıldı. ID: {Id}", id);

            var result = await _hizmetBinasiService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Hizmet binasının durumunu değiştirir (Aktif/Pasif)
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Durum değiştirme sonucu</returns>
        [HttpPatch("{id:int}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            _logger.LogInformation("ToggleStatus endpoint çağrıldı. ID: {Id}", id);

            var result = await _hizmetBinasiService.ToggleStatusAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Hizmet Binasında çalışan personellerin servislerini getirir
        /// </summary>
        /// <param name="id">Hizmet Binası ID</param>
        /// <returns>Servis listesi (PersonelSayisi dahil)</returns>
        [HttpGet("{id:int}/servisler")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetServislerByHizmetBinasiId(int id)
        {
            _logger.LogInformation("GetServislerByHizmetBinasiId endpoint çağrıldı. Hizmet Binası ID: {Id}", id);
            var result = await _hizmetBinasiService.GetServislerByHizmetBinasiIdAsync(id);
            return Ok(result);
        }
    }
}
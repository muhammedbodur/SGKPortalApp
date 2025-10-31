using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.Siramatik
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalPersonelController : ControllerBase
    {
        private readonly IKanalPersonelService _kanalPersonelService;
        private readonly ILogger<KanalPersonelController> _logger;

        public KanalPersonelController(
            IKanalPersonelService kanalPersonelService,
            ILogger<KanalPersonelController> logger)
        {
            _kanalPersonelService = kanalPersonelService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Yeni personel ataması oluşturur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<KanalPersonelResponseDto>>> Create(
            [FromBody] KanalPersonelCreateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<KanalPersonelResponseDto>
                    .ErrorResult("Geçersiz veri"));
            }

            var result = await _kanalPersonelService.CreateAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personel ataması günceller
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<KanalPersonelResponseDto>>> Update(
            int id,
            [FromBody] KanalPersonelUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<KanalPersonelResponseDto>
                    .ErrorResult("Geçersiz veri"));
            }

            var result = await _kanalPersonelService.UpdateAsync(id, request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personel atamasını siler (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Delete(int id)
        {
            var result = await _kanalPersonelService.DeleteAsync(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre personel atamasını getirir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponseDto<KanalPersonelResponseDto>>> GetById(int id)
        {
            var result = await _kanalPersonelService.GetByIdAsync(id);

            return result.Success ? Ok(result) : NotFound(result);
        }

        // ═══════════════════════════════════════════════════════
        // QUERY OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Hizmet binasına göre personel atamalarını getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}")]
        public async Task<ActionResult<ApiResponseDto<List<KanalPersonelResponseDto>>>> GetByHizmetBinasiId(
            int hizmetBinasiId)
        {
            var result = await _kanalPersonelService.GetPersonellerByHizmetBinasiIdAsync(hizmetBinasiId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Personele göre atamalarını getirir
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}")]
        public async Task<ActionResult<ApiResponseDto<List<KanalPersonelResponseDto>>>> GetByPersonelTc(
            string tcKimlikNo)
        {
            var result = await _kanalPersonelService.GetByPersonelTcAsync(tcKimlikNo);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Kanal alt işleme göre personelleri getirir
        /// </summary>
        [HttpGet("kanal-alt-islem/{kanalAltIslemId:int}")]
        public async Task<ActionResult<ApiResponseDto<List<KanalPersonelResponseDto>>>> GetByKanalAltIslemId(
            int kanalAltIslemId)
        {
            var result = await _kanalPersonelService.GetByKanalAltIslemIdAsync(kanalAltIslemId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Personel Atama Matrix - Her personel bir satır, kanal atamaları collection olarak
        /// </summary>
        [HttpGet("matrix/{hizmetBinasiId:int}")]
        public async Task<ActionResult<ApiResponseDto<List<PersonelAtamaMatrixDto>>>> GetPersonelAtamaMatrix(
            int hizmetBinasiId)
        {
            var result = await _kanalPersonelService.GetPersonelAtamaMatrixAsync(hizmetBinasiId);

            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
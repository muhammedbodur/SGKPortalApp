using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/siramatik/sira-yonlendirme")]
    public class SiraYonlendirmeController : ControllerBase
    {
        private readonly ISiraYonlendirmeService _siraYonlendirmeService;
        private readonly ILogger<SiraYonlendirmeController> _logger;

        public SiraYonlendirmeController(
            ISiraYonlendirmeService siraYonlendirmeService,
            ILogger<SiraYonlendirmeController> logger)
        {
            _siraYonlendirmeService = siraYonlendirmeService;
            _logger = logger;
        }

        /// <summary>
        /// Çağrılmış bir sırayı başka bankoya, şefe veya uzman personele yönlendirir.
        /// </summary>
        /// <param name="request">Yönlendirme bilgileri</param>
        /// <returns>Yönlendirme işleminin sonucu</returns>
        [HttpPost("yonlendir")]
        public async Task<IActionResult> YonlendirSiraAsync([FromBody] SiraYonlendirmeDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _siraYonlendirmeService.YonlendirSiraAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Belirtilen bankoya yönlendirilmiş sıra sayısını getirir.
        /// </summary>
        /// <param name="bankoId">Banko ID</param>
        /// <returns>Yönlendirilmiş sıra sayısı</returns>
        [HttpGet("yonlendirilmis-sira-count/{bankoId:int}")]
        public async Task<IActionResult> GetYonlendirilmisSiraCountAsync(int bankoId)
        {
            var result = await _siraYonlendirmeService.GetYonlendirilmisSiraCountAsync(bankoId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}

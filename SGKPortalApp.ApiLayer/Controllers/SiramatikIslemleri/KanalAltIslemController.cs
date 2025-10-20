using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanalAltIslemController : ControllerBase
    {
        private readonly IKanalAltIslemService _kanalAltIslemService;
        private readonly ILogger<KanalAltIslemController> _logger;

        public KanalAltIslemController(
            IKanalAltIslemService kanalAltIslemService,
            ILogger<KanalAltIslemController> logger)
        {
            _kanalAltIslemService = kanalAltIslemService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm kanal alt işlemleri getirir (detaylı)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kanalAltIslemService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre kanal alt işlem getirir (detaylı)
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdWithDetails(int id)
        {
            var result = await _kanalAltIslemService.GetByIdWithDetailsAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Hizmet binasına göre kanal alt işlemleri getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasiId(int hizmetBinasiId)
        {
            var result = await _kanalAltIslemService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kanal işlemine göre kanal alt işlemleri getirir
        /// </summary>
        [HttpGet("kanal-islem/{kanalIslemId:int}")]
        public async Task<IActionResult> GetByKanalIslemId(int kanalIslemId)
        {
            var result = await _kanalAltIslemService.GetByKanalIslemIdAsync(kanalIslemId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Hizmet binasına göre kanal alt işlem personel sayılarını getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}/personel-sayilari")]
        public async Task<IActionResult> GetPersonelSayilari(int hizmetBinasiId)
        {
            var result = await _kanalAltIslemService.GetPersonelSayilariAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Hizmet binasına göre personel eşleştirilmemiş kanal alt işlemleri getirir
        /// </summary>
        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}/eslestirme-yapilmamis")]
        public async Task<IActionResult> GetEslestirmeYapilmamis(int hizmetBinasiId)
        {
            var result = await _kanalAltIslemService.GetEslestirmeYapilmamisAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

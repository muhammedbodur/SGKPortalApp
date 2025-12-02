using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    [ApiController]
    [Route("api/siramatik/sira-cagirma")]
    public class SiraCagirmaController : ControllerBase
    {
        private readonly ISiraCagirmaService _siraCagirmaService;
        private readonly ILogger<SiraCagirmaController> _logger;

        public SiraCagirmaController(
            ISiraCagirmaService siraCagirmaService,
            ILogger<SiraCagirmaController> logger)
        {
            _siraCagirmaService = siraCagirmaService;
            _logger = logger;
        }

        /// <summary>
        /// Beklemedeki tüm sıraları getirir.
        /// </summary>
        [HttpGet("bekleyen-siralar")]
        public async Task<IActionResult> GetBekleyenSiralarAsync()
        {
            var result = await _siraCagirmaService.GetBekleyenSiralarAsync();
            return Ok(result);
        }

        /// <summary>
        /// Personelin bekleyen sıralarını getirir.
        /// </summary>
        [HttpGet("personel-bekleyen-siralar/{tcKimlikNo}")]
        public async Task<IActionResult> GetPersonelBekleyenSiralarAsync(string tcKimlikNo)
        {
            var result = await _siraCagirmaService.GetPersonelBekleyenSiralarAsync(tcKimlikNo);
            return Ok(result);
        }

        /// <summary>
        /// Banko paneli için uzmanlık bazlı bekleyen sıraları getirir.
        /// </summary>
        [HttpGet("banko-panel/{tcKimlikNo}")]
        public async Task<IActionResult> GetBankoPanelSiralarAsync(string tcKimlikNo)
        {
            var result = await _siraCagirmaService.GetBankoPanelSiralarAsync(tcKimlikNo);
            return Ok(result);
        }

        /// <summary>
        /// Sıradaki vatandaşı çağırır.
        /// </summary>
        [HttpPost("siradaki-cagir/{siraId:int}")]
        public async Task<IActionResult> SiradakiCagirAsync(int siraId, [FromQuery] string personelTcKimlikNo)
        {
            if (string.IsNullOrWhiteSpace(personelTcKimlikNo))
            {
                return BadRequest("personelTcKimlikNo zorunludur.");
            }

            var result = await _siraCagirmaService.SiradakiCagirAsync(siraId, personelTcKimlikNo);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Sırayı tamamlar.
        /// </summary>
        [HttpPut("tamamla/{siraId:int}")]
        public async Task<IActionResult> SiraTamamlaAsync(int siraId)
        {
            var success = await _siraCagirmaService.SiraTamamlaAsync(siraId);
            return success ? Ok() : NotFound();
        }

        /// <summary>
        /// Sırayı iptal eder.
        /// </summary>
        [HttpDelete("iptal/{siraId:int}")]
        public async Task<IActionResult> SiraIptalAsync(int siraId, [FromQuery] string? iptalNedeni)
        {
            var success = await _siraCagirmaService.SiraIptalAsync(siraId, iptalNedeni ?? string.Empty);
            return success ? Ok() : NotFound();
        }
    }
}

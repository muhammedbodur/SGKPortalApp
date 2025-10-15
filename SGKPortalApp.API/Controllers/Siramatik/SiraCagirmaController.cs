using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLayer.Services.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.API.Controllers.Siramatik
{
    /// <summary>
    /// Sıra Çağırma API Controller
    /// </summary>
    [Authorize]
    [Route("api/siramatik/sira-cagirma")]
    [ApiController]
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
        /// Beklemedeki sıraları getirir
        /// </summary>
        [HttpGet("bekleyen-siralar")]
        [ProducesResponseType(typeof(List<SiraCagirmaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBekleyenSiralar()
        {
            try
            {
                var siralar = await _siraCagirmaService.GetBekleyenSiralarAsync();
                return Ok(siralar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bekleyen sıralar getirilirken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        /// <summary>
        /// Personelin bekleyen sıralarını getirir
        /// </summary>
        [HttpGet("personel-bekleyen-siralar/{tcKimlikNo}")]
        [ProducesResponseType(typeof(List<SiraCagirmaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonelBekleyenSiralar(string tcKimlikNo)
        {
            try
            {
                var siralar = await _siraCagirmaService.GetPersonelBekleyenSiralarAsync(tcKimlikNo);
                return Ok(siralar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel bekleyen sıraları getirilirken hata oluştu. TC: {TcKimlikNo}", tcKimlikNo);
                return StatusCode(500, "Sunucu hatası");
            }
        }

        /// <summary>
        /// Sıradaki vatandaşı çağırır
        /// </summary>
        [HttpPost("siradaki-cagir/{siraId}")]
        [ProducesResponseType(typeof(SiraCagirmaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SiradakiCagir(int siraId)
        {
            try
            {
                // TODO: Personel TC'yi session'dan al
                var personelTcKimlikNo = User.Identity?.Name ?? "11111111111";

                var sira = await _siraCagirmaService.SiradakiCagirAsync(siraId, personelTcKimlikNo);
                
                if (sira == null)
                {
                    return NotFound("Sıra bulunamadı veya çağrılamaz durumda");
                }

                return Ok(sira);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sıra çağrılırken hata oluştu. SiraId: {SiraId}", siraId);
                return StatusCode(500, "Sunucu hatası");
            }
        }

        /// <summary>
        /// Sırayı tamamlar
        /// </summary>
        [HttpPut("tamamla/{siraId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SiraTamamla(int siraId)
        {
            try
            {
                var result = await _siraCagirmaService.SiraTamamlaAsync(siraId);
                
                if (!result)
                {
                    return NotFound("Sıra bulunamadı");
                }

                return Ok("Sıra başarıyla tamamlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sıra tamamlanırken hata oluştu. SiraId: {SiraId}", siraId);
                return StatusCode(500, "Sunucu hatası");
            }
        }

        /// <summary>
        /// Sırayı iptal eder
        /// </summary>
        [HttpDelete("iptal/{siraId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SiraIptal(int siraId, [FromQuery] string iptalNedeni = "Belirtilmedi")
        {
            try
            {
                var result = await _siraCagirmaService.SiraIptalAsync(siraId, iptalNedeni);
                
                if (!result)
                {
                    return NotFound("Sıra bulunamadı");
                }

                return Ok("Sıra başarıyla iptal edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sıra iptal edilirken hata oluştu. SiraId: {SiraId}", siraId);
                return StatusCode(500, "Sunucu hatası");
            }
        }
    }
}

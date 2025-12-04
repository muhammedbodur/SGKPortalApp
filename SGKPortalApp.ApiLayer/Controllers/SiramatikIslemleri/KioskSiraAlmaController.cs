using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk Sıra Alma Controller
    /// Masaüstü kiosk uygulamasını simüle eden API endpoint'leri
    /// 
    /// Vatandaş Akışı:
    /// 1. GET /menuler/{hizmetBinasiId} → Kiosk menülerini listele
    /// 2. GET /alt-islemler/{hizmetBinasiId}/{kioskMenuId} → Seçilen menüdeki alt işlemleri listele
    /// 3. POST /sira-al → Seçilen işlem için sıra al
    /// </summary>
    [ApiController]
    [Route("api/siramatik/kiosk-sira-alma")]
    public class KioskSiraAlmaController : ControllerBase
    {
        private readonly IKioskSiraAlmaService _kioskSiraAlmaService;

        public KioskSiraAlmaController(IKioskSiraAlmaService kioskSiraAlmaService)
        {
            _kioskSiraAlmaService = kioskSiraAlmaService;
        }

        // ═══════════════════════════════════════════════════════
        // YENİ YAPILAR: KIOSK BAZLI İŞLEMLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir kiosk'un menülerini listele (Complex Query)
        /// GET api/siramatik/kiosk-sira-alma/menuler-by-kiosk/{kioskId}
        /// </summary>
        /// <remarks>
        /// Vatandaşın kiosk ekranında ilk gördüğü menü listesi.
        /// Sadece aktif personeli olan menüler döner.
        /// Complex query kullanarak performanslı sonuç döner.
        /// </remarks>
        [HttpGet("menuler-by-kiosk/{kioskId:int}")]
        public async Task<IActionResult> GetKioskMenulerByKioskIdAsync(int kioskId)
        {
            var result = await _kioskSiraAlmaService.GetKioskMenulerByKioskIdAsync(kioskId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Seçilen menüdeki alt işlemleri kiosk bazlı listele (Complex Query)
        /// GET api/siramatik/kiosk-sira-alma/alt-islemler-by-kiosk/{kioskId}/{kioskMenuId}
        /// </summary>
        /// <remarks>
        /// Vatandaş menü seçtikten sonra gördüğü alt işlem listesi.
        /// Sadece aktif personel (Yrd.Uzman+) olan ve banko modunda bulunan işlemler döner.
        /// Complex query kullanarak performanslı sonuç döner.
        /// </remarks>
        [HttpGet("alt-islemler-by-kiosk/{kioskId:int}/{kioskMenuId:int}")]
        public async Task<IActionResult> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId)
        {
            var result = await _kioskSiraAlmaService.GetKioskMenuAltIslemleriByKioskIdAsync(kioskId, kioskMenuId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // ESKİ YAPILAR: HİZMET BİNASI BAZLI İŞLEMLER (Geriye Uyumluluk)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// [ESKİ] Hizmet binasındaki kiosk menülerini listele
        /// GET api/siramatik/kiosk-sira-alma/menuler/{hizmetBinasiId}
        /// </summary>
        /// <remarks>
        /// Vatandaşın kiosk ekranında ilk gördüğü menü listesi.
        /// Sadece en az bir alt işleminde aktif personel (Yrd.Uzman+) olan menüler döner.
        /// </remarks>
        [HttpGet("menuler/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetKioskMenulerAsync(int hizmetBinasiId)
        {
            var result = await _kioskSiraAlmaService.GetKioskMenulerAsync(hizmetBinasiId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// [ESKİ] Seçilen kiosk menüsündeki alt işlemleri listele
        /// GET api/siramatik/kiosk-sira-alma/alt-islemler/{hizmetBinasiId}/{kioskMenuId}
        /// </summary>
        /// <remarks>
        /// Vatandaş menü seçtikten sonra gördüğü alt işlem listesi.
        /// Sadece aktif personel (Yrd.Uzman+) olan işlemler döner.
        /// </remarks>
        [HttpGet("alt-islemler/{hizmetBinasiId:int}/{kioskMenuId:int}")]
        public async Task<IActionResult> GetKioskMenuAltIslemleriAsync(int hizmetBinasiId, int kioskMenuId)
        {
            var result = await _kioskSiraAlmaService.GetKioskMenuAltIslemleriAsync(hizmetBinasiId, kioskMenuId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // ADIM 3: SIRA AL
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Kiosk'tan sıra al
        /// POST api/siramatik/kiosk-sira-alma/sira-al
        /// </summary>
        /// <remarks>
        /// Vatandaş alt işlem seçtikten sonra sıra alır:
        /// - KioskMenuIslemId üzerinden hangi işlem için sıra alınacağını belirler
        /// - HizmetBinasiId ile hangi binada sıra alınacağını belirler
        /// - Bu işlem için banko modunda aktif personel (Yrd.Uzman+) olup olmadığını kontrol eder
        /// - Yeni sıra numarası üretir ve kaydeder
        /// - SignalR ile banko panellerine bildirim gönderir
        /// </remarks>
        [HttpPost("sira-al")]
        public async Task<IActionResult> SiraAlAsync([FromBody] KioskSiraAlRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _kioskSiraAlmaService.SiraAlAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // YARDIMCI ENDPOINT'LER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir işlem için bekleyen sıra sayısını getir
        /// GET api/siramatik/kiosk-sira-alma/bekleyen-sayi
        /// </summary>
        [HttpGet("bekleyen-sayi")]
        public async Task<IActionResult> GetBekleyenSayisiAsync(
            [FromQuery] int hizmetBinasiId,
            [FromQuery] int kanalAltIslemId)
        {
            var sayi = await _kioskSiraAlmaService.GetBekleyenSiraSayisiAsync(hizmetBinasiId, kanalAltIslemId);
            return Ok(new { BekleyenSiraSayisi = sayi });
        }

        /// <summary>
        /// Belirli bir işlem için aktif personel (Yrd.Uzman+) var mı kontrol et
        /// GET api/siramatik/kiosk-sira-alma/aktif-personel-var
        /// </summary>
        [HttpGet("aktif-personel-var")]
        public async Task<IActionResult> HasAktifPersonelAsync(
            [FromQuery] int hizmetBinasiId,
            [FromQuery] int kanalAltIslemId)
        {
            var var = await _kioskSiraAlmaService.HasAktifPersonelAsync(hizmetBinasiId, kanalAltIslemId);
            return Ok(new { AktifPersonelVar = var });
        }
    }
}

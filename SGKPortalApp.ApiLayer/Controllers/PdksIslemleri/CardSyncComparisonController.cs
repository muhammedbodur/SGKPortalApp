using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [Authorize]
    [Route("api/pdks/card-sync-comparison")]
    [ApiController]
    public class CardSyncComparisonController : ControllerBase
    {
        private readonly ICardSyncComparisonService _cardSyncComparisonService;

        public CardSyncComparisonController(ICardSyncComparisonService cardSyncComparisonService)
        {
            _cardSyncComparisonService = cardSyncComparisonService;
        }

        /// <summary>
        /// Tüm cihazlardan kart bilgilerini çeker ve veritabanı ile karşılaştırarak uyumsuzluk raporu oluşturur
        /// </summary>
        [HttpGet("generate-report")]
        public async Task<IActionResult> GenerateReport()
        {
            var report = await _cardSyncComparisonService.GenerateCardSyncReportAsync();
            return Ok(report);
        }
    }
}

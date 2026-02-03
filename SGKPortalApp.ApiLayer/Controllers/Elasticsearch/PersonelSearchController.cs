using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Elasticsearch;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch;

namespace SGKPortalApp.ApiLayer.Controllers.Elasticsearch
{
    /// <summary>
    /// Elasticsearch personel arama API controller
    /// Fuzzy search, Türkçe karakter toleransı, yetki bazlı filtreleme
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonelSearchController : ControllerBase
    {
        private readonly IPersonelSearchService _searchService;
        private readonly IPersonelIndexSyncService _syncService;
        private readonly ILogger<PersonelSearchController> _logger;

        public PersonelSearchController(
            IPersonelSearchService searchService,
            IPersonelIndexSyncService syncService,
            ILogger<PersonelSearchController> logger)
        {
            _searchService = searchService;
            _syncService = syncService;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // 🔍 ARAMA ENDPOINTLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Fuzzy personel araması
        /// Türkçe karakter ve yanlış yazım toleranslı
        /// </summary>
        /// <param name="q">Arama terimi (min 2 karakter)</param>
        /// <param name="departmanIds">Yetki bazlı departman filtreleri (virgülle ayrılmış)</param>
        /// <param name="sadeceAktif">Sadece aktif personeller (default: true)</param>
        /// <param name="size">Maksimum sonuç sayısı (default: 20)</param>
        /// <returns>Eşleşen personeller</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PersonelElasticDto>>> Search(
            [FromQuery] string q,
            [FromQuery] string? departmanIds = null,
            [FromQuery] bool sadeceAktif = true,
            [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return Ok(Enumerable.Empty<PersonelElasticDto>());
            }

            // Departman ID'lerini parse et
            var departmanIdList = ParseDepartmanIds(departmanIds);

            var results = await _searchService.SearchAsync(q, departmanIdList, sadeceAktif, size);
            return Ok(results);
        }

        /// <summary>
        /// Autocomplete araması
        /// Yazdıkça sonuç gösterimi için
        /// </summary>
        /// <param name="prefix">Önek (min 2 karakter)</param>
        /// <param name="departmanIds">Yetki bazlı departman filtreleri</param>
        /// <param name="sadeceAktif">Sadece aktif personeller</param>
        /// <param name="size">Maksimum sonuç sayısı (default: 10)</param>
        [HttpGet("autocomplete")]
        public async Task<ActionResult<IEnumerable<PersonelElasticDto>>> Autocomplete(
            [FromQuery] string prefix,
            [FromQuery] string? departmanIds = null,
            [FromQuery] bool sadeceAktif = true,
            [FromQuery] int size = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
            {
                return Ok(Enumerable.Empty<PersonelElasticDto>());
            }

            var departmanIdList = ParseDepartmanIds(departmanIds);

            var results = await _searchService.AutocompleteAsync(prefix, departmanIdList, sadeceAktif, size);
            return Ok(results);
        }

        // ═══════════════════════════════════════════════════════
        // 🔧 INDEX YÖNETİM ENDPOINTLERİ (Admin)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Index durumu bilgisi
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<IndexStatusInfo>> GetStatus()
        {
            var status = await _syncService.GetIndexStatusAsync();
            return Ok(status);
        }

        /// <summary>
        /// Elasticsearch bağlantı testi
        /// </summary>
        [HttpGet("ping")]
        public async Task<ActionResult<object>> Ping()
        {
            var isAvailable = await _searchService.PingAsync();
            return Ok(new
            {
                Available = isAvailable,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Index'i oluşturur (yoksa)
        /// </summary>
        [HttpPost("create-index")]
        public async Task<ActionResult> CreateIndex()
        {
            var result = await _searchService.CreateIndexAsync();
            if (result)
            {
                return Ok(new { Message = "Index başarıyla oluşturuldu" });
            }
            return BadRequest(new { Message = "Index oluşturulamadı" });
        }

        /// <summary>
        /// Tüm personelleri yeniden indexler
        /// DİKKAT: Bu işlem uzun sürebilir!
        /// </summary>
        [HttpPost("full-reindex")]
        public async Task<ActionResult> FullReindex(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Full reindex isteği alındı");

            var indexedCount = await _syncService.FullReindexAsync(cancellationToken);

            return Ok(new
            {
                Message = "Full reindex tamamlandı",
                IndexedCount = indexedCount
            });
        }

        /// <summary>
        /// Belirli tarihten sonra güncellenen personelleri senkronize eder
        /// </summary>
        /// <param name="sinceHours">Son kaç saat içinde güncellenenler (default: 24)</param>
        [HttpPost("incremental-sync")]
        public async Task<ActionResult> IncrementalSync([FromQuery] int sinceHours = 24, CancellationToken cancellationToken = default)
        {
            var sinceDate = DateTime.UtcNow.AddHours(-sinceHours);
            var indexedCount = await _syncService.IncrementalSyncAsync(sinceDate, cancellationToken);

            return Ok(new
            {
                Message = "Incremental sync tamamlandı",
                IndexedCount = indexedCount,
                SinceDate = sinceDate
            });
        }

        /// <summary>
        /// Tek bir personeli senkronize eder
        /// </summary>
        [HttpPost("sync/{tcKimlikNo}")]
        public async Task<ActionResult> SyncPersonel(string tcKimlikNo)
        {
            var result = await _syncService.SyncPersonelAsync(tcKimlikNo);
            if (result)
            {
                return Ok(new { Message = $"Personel {tcKimlikNo} senkronize edildi" });
            }
            return BadRequest(new { Message = "Personel senkronize edilemedi" });
        }

        /// <summary>
        /// Index'teki toplam doküman sayısı
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<long>> GetDocumentCount()
        {
            var count = await _searchService.GetDocumentCountAsync();
            return Ok(new { Count = count });
        }

        /// <summary>
        /// Index'i siler
        /// DİKKAT: Tüm veriler kaybolur!
        /// </summary>
        [HttpDelete("delete-index")]
        public async Task<ActionResult> DeleteIndex()
        {
            _logger.LogWarning("Index silme isteği alındı");
            var result = await _searchService.DeleteIndexAsync();
            if (result)
            {
                return Ok(new { Message = "Index başarıyla silindi" });
            }
            return BadRequest(new { Message = "Index silinemedi" });
        }

        // ═══════════════════════════════════════════════════════
        // 🔧 YARDIMCI METODLAR
        // ═══════════════════════════════════════════════════════

        private static IEnumerable<int>? ParseDepartmanIds(string? departmanIds)
        {
            if (string.IsNullOrWhiteSpace(departmanIds))
                return null;

            return departmanIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
        }
    }
}

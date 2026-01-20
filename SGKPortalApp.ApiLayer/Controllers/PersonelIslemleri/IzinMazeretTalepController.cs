using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri API Controller
    /// CRUD, onay workflow ve raporlama endpoints
    /// </summary>
    [ApiController]
    [Route("api/izin-mazeret-talep")]
    public class IzinMazeretTalepController : ControllerBase
    {
        private readonly IIzinMazeretTalepService _service;
        private readonly ILogger<IzinMazeretTalepController> _logger;

        public IzinMazeretTalepController(
            IIzinMazeretTalepService service,
            ILogger<IzinMazeretTalepController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tüm talepleri getir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre talep getir
        /// 🔒 Kullanıcı sadece kendi taleplerini görebilir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Kullanıcının TC'sini claim'den al
            var currentUserTc = User?.FindFirst("TcKimlikNo")?.Value;

            var result = await _service.GetByIdAsync(id, currentUserTc);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Yeni izin/mazeret talebi oluştur (çakışma kontrolü ile)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IzinMazeretTalepCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data?.IzinMazeretTalepId }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// İzin/mazeret talebini güncelle (sadece beklemedeki talepler)
        /// 🔒 Kullanıcı sadece kendi taleplerini düzenleyebilir
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] IzinMazeretTalepUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcının TC'sini claim'den al
            var currentUserTc = User?.FindFirst("TcKimlikNo")?.Value;

            var result = await _service.UpdateAsync(id, request, currentUserTc);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin/mazeret talebini sil (soft delete)
        /// 🔒 Kullanıcı sadece kendi taleplerini silebilir
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Kullanıcının TC'sini claim'den al
            var currentUserTc = User?.FindFirst("TcKimlikNo")?.Value;

            var result = await _service.DeleteAsync(id, currentUserTc);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin/mazeret talebini iptal et
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] string iptalNedeni)
        {
            var result = await _service.CancelAsync(id, iptalNedeni);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // PERSONEL BAZINDA SORGULAR
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personele ait tüm talepler
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}")]
        public async Task<IActionResult> GetByPersonelTc(string tcKimlikNo, [FromQuery] bool includeInactive = false)
        {
            var result = await _service.GetByPersonelTcAsync(tcKimlikNo, includeInactive);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personele ait bekleyen talepler
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}/pending")]
        public async Task<IActionResult> GetPendingByPersonelTc(string tcKimlikNo)
        {
            var result = await _service.GetPendingByPersonelTcAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personele ait onaylanmış talepler (tarih aralığı ile)
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}/approved")]
        public async Task<IActionResult> GetApprovedByPersonelTc(
            string tcKimlikNo,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _service.GetApprovedByPersonelTcAsync(tcKimlikNo, startDate, endDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // ONAY İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Talebi onayla veya reddet (1. veya 2. onayci)
        /// </summary>
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> ApproveOrReject(
            int id,
            [FromBody] IzinMazeretTalepOnayRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Onayci TC'yi claim'den al
            var onayciTcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
            if (string.IsNullOrEmpty(onayciTcKimlikNo))
                return Unauthorized("Onayci bilgisi alınamadı");

            var result = await _service.ApproveOrRejectAsync(id, onayciTcKimlikNo, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 1. Onayci için bekleyen talepler
        /// </summary>
        [HttpGet("pending/first-approver/{onayciTcKimlikNo}")]
        public async Task<IActionResult> GetPendingForFirstApprover(string onayciTcKimlikNo)
        {
            var result = await _service.GetPendingForFirstApproverAsync(onayciTcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 2. Onayci için bekleyen talepler
        /// </summary>
        [HttpGet("pending/second-approver/{onayciTcKimlikNo}")]
        public async Task<IActionResult> GetPendingForSecondApprover(string onayciTcKimlikNo)
        {
            var result = await _service.GetPendingForSecondApproverAsync(onayciTcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Departman bazında bekleyen talepler (yönetici görünümü)
        /// </summary>
        [HttpGet("pending/departman/{departmanId:int}")]
        public async Task<IActionResult> GetPendingByDepartman(int departmanId)
        {
            var result = await _service.GetPendingByDepartmanAsync(departmanId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Servis bazında bekleyen talepler
        /// </summary>
        [HttpGet("pending/servis/{servisId:int}")]
        public async Task<IActionResult> GetPendingByServis(int servisId)
        {
            var result = await _service.GetPendingByServisAsync(servisId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // RAPORLAMA VE FİLTRELEME
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Filtrelenmiş talepler (raporlama için)
        /// Permission-based filtering destekler
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> GetFiltered([FromBody] IzinMazeretTalepFilterRequestDto filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.GetFilteredAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih aralığındaki tüm izin/mazeret talepleri
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? departmanId = null,
            [FromQuery] int? servisId = null)
        {
            var result = await _service.GetByDateRangeAsync(startDate, endDate, departmanId, servisId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // İSTATİSTİKLER
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Personelin yıllık izin kullanım sayısı
        /// </summary>
        [HttpGet("statistics/yillik-izin/{tcKimlikNo}/{year:int}")]
        public async Task<IActionResult> GetTotalYillikIzinDays(string tcKimlikNo, int year)
        {
            var result = await _service.GetTotalYillikIzinDaysAsync(tcKimlikNo, year);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personelin toplam kullanılan izin günü
        /// </summary>
        [HttpGet("statistics/total-days/{tcKimlikNo}")]
        public async Task<IActionResult> GetTotalUsedDays(
            string tcKimlikNo,
            [FromQuery] int? izinTuruValue = null,
            [FromQuery] int? year = null)
        {
            var result = await _service.GetTotalUsedDaysAsync(tcKimlikNo, izinTuruValue, year);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ═══════════════════════════════════════════════════════
        // ÇAKIŞMA KONTROLÜ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Tarih çakışması kontrolü (önemli!)
        /// </summary>
        [HttpPost("check-overlap")]
        public async Task<IActionResult> CheckOverlap([FromBody] IzinMazeretTalepCreateRequestDto request)
        {
            var result = await _service.CheckOverlapAsync(
                request.TcKimlikNo,
                request.BaslangicTarihi,
                request.BitisTarihi,
                request.MazeretTarihi,
                null);

            return Ok(new
            {
                hasOverlap = result.Data,
                message = result.Data
                    ? "⚠️ Bu tarih aralığında zaten bir izin/mazeret kaydı var"
                    : "✅ Çakışma yok, talep oluşturulabilir"
            });
        }
    }
}

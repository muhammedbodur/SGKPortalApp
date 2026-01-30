using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.ApiLayer.Controllers.PersonelIslemleri
{
    [ApiController]
    [Route("api/personel")]
    public class PersonelController : ControllerBase
    {
        private readonly IPersonelService _personelService;
        private readonly ILogger<PersonelController> _logger;

        public PersonelController(
            IPersonelService personelService,
            ILogger<PersonelController> logger)
        {
            _personelService = personelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personelService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{tcKimlikNo}")]
        public async Task<IActionResult> GetByTcKimlikNo(string tcKimlikNo)
        {
            var result = await _personelService.GetByTcKimlikNoAsync(tcKimlikNo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{tcKimlikNo}")]
        public async Task<IActionResult> Update(string tcKimlikNo, [FromBody] PersonelUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.RequestorTcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
            request.RequestorSessionId = User?.FindFirst("SessionID")?.Value;

            var result = await _personelService.UpdateAsync(tcKimlikNo, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{tcKimlikNo}")]
        public async Task<IActionResult> Delete(string tcKimlikNo)
        {
            var result = await _personelService.DeleteAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _personelService.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPaged([FromBody] PersonelFilterRequestDto filter)
        {
            var result = await _personelService.GetPagedAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("departman/{departmanId:int}")]
        public async Task<IActionResult> GetByDepartman(int departmanId)
        {
            var result = await _personelService.GetByDepartmanAsync(departmanId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("servis/{servisId:int}")]
        public async Task<IActionResult> GetByServis(int servisId)
        {
            var result = await _personelService.GetByServisAsync(servisId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("hizmet-binasi/{hizmetBinasiId:int}")]
        public async Task<IActionResult> GetByHizmetBinasi(int hizmetBinasiId)
        {
            var result = await _personelService.GetByHizmetBinasiIdAsync(hizmetBinasiId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Toplu kayıt endpoint'leri (Transaction)
        [HttpPost("complete")]
        public async Task<IActionResult> CreateComplete([FromBody] PersonelCompleteRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.RequestorTcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
            request.RequestorSessionId = User?.FindFirst("SessionID")?.Value;

            var result = await _personelService.CreateCompleteAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetByTcKimlikNo), new { tcKimlikNo = result.Data?.TcKimlikNo }, result) : BadRequest(result);
        }

        [HttpPut("{tcKimlikNo}/complete")]
        public async Task<IActionResult> UpdateComplete(string tcKimlikNo, [FromBody] PersonelCompleteRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.RequestorTcKimlikNo = User?.FindFirst("TcKimlikNo")?.Value;
            request.RequestorSessionId = User?.FindFirst("SessionID")?.Value;

            var result = await _personelService.UpdateCompleteAsync(tcKimlikNo, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string? term, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 1)
                return Ok(new List<object>());

            var filter = new PersonelFilterRequestDto
            {
                SearchTerm = term.Trim(),
                AktiflikDurum = PersonelAktiflikDurum.Aktif,
                PageNumber = 1,
                PageSize = limit
            };

            var result = await _personelService.GetPagedAsync(filter);
            
            if (!result.Success || result.Data?.Items == null)
                return Ok(new List<object>());

            var suggestions = result.Data.Items.Select(p => new
            {
                id = p.TcKimlikNo,
                sicilNo = p.SicilNo,
                adSoyad = p.AdSoyad,
                tcKimlikNo = p.TcKimlikNo,
                servisAdi = p.ServisAdi,
                departmanAdi = p.DepartmanAdi,
                displayText = $"{p.AdSoyad} ({p.SicilNo})"
            }).ToList();

            return Ok(suggestions);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? term, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Ok(new List<object>());

            var filter = new PersonelFilterRequestDto
            {
                SearchTerm = term.Trim(),
                PageNumber = 1,
                PageSize = limit
            };

            var result = await _personelService.GetPagedAsync(filter);
            
            if (!result.Success || result.Data?.Items == null)
                return Ok(new List<object>());

            var searchResults = result.Data.Items.Select(p => new
            {
                TcKimlikNo = p.TcKimlikNo,
                AdSoyad = p.AdSoyad,
                SicilNo = p.SicilNo,
                DepartmanAdi = p.DepartmanAdi,
                UnvanAdi = p.UnvanAdi,
                Resim = p.Resim,
                PersonelAktiflikDurum = p.PersonelAktiflikDurum
            }).ToList();

            return Ok(searchResults);
        }

        // ═══════════════════════════════════════════════════════
        // PDKS CİHAZ İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        [HttpPost("{tcKimlikNo}/pdks/send-to-all-devices")]
        public async Task<IActionResult> SendCardToAllDevices(string tcKimlikNo)
        {
            var result = await _personelService.SendCardToAllDevicesAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{tcKimlikNo}/pdks/delete-from-all-devices")]
        public async Task<IActionResult> DeleteCardFromAllDevices(string tcKimlikNo)
        {
            var result = await _personelService.DeleteCardFromAllDevicesAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

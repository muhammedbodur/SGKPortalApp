using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/izin-sorumlu")]
    public class IzinSorumluController : ControllerBase
    {
        private readonly IIzinSorumluService _service;
        private readonly ILogger<IzinSorumluController> _logger;

        public IzinSorumluController(
            IIzinSorumluService service,
            ILogger<IzinSorumluController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Tüm izin sorumlu atamalarını getir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif izin sorumlu atamalarını getir
        /// </summary>
        [HttpGet("aktif")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _service.GetActiveAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre izin sorumlu ataması getir
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Departman/Servis bazında sorumluları getir
        /// </summary>
        [HttpGet("departman-servis")]
        public async Task<IActionResult> GetByDepartmanServis([FromQuery] int? departmanId, [FromQuery] int? servisId)
        {
            var result = await _service.GetByDepartmanServisAsync(departmanId, servisId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personel için sorumluları getir
        /// </summary>
        [HttpGet("personel/{tcKimlikNo}")]
        public async Task<IActionResult> GetSorumluForPersonel(string tcKimlikNo)
        {
            var result = await _service.GetSorumluForPersonelAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni izin sorumlusu ata
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IzinSorumluCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.IzinSorumluId }, result) : BadRequest(result);
        }

        /// <summary>
        /// İzin sorumlusu güncelle
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] IzinSorumluUpdateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.IzinSorumluId)
                return BadRequest("ID uyuşmazlığı");

            var result = await _service.UpdateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin sorumlusunu pasif yap
        /// </summary>
        [HttpPatch("{id:int}/pasif")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _service.DeactivateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin sorumlusunu aktif yap
        /// </summary>
        [HttpPatch("{id:int}/aktif")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _service.ActivateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İzin sorumlusunu sil
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

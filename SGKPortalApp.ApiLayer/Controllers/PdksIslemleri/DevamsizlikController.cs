using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/devamsizlik")]
    public class DevamsizlikController : ControllerBase
    {
        private readonly IDevamsizlikService _service;

        public DevamsizlikController(IDevamsizlikService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devamsızlık/Mazeret listesi
        /// </summary>
        [HttpPost("liste")]
        public async Task<IActionResult> GetList([FromBody] DevamsizlikFilterDto filter)
        {
            var result = await _service.GetDevamsizlikListAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni devamsızlık/mazeret kaydı oluştur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DevamsizlikCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateDevamsizlikAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Devamsızlık/Mazeret onayla
        /// </summary>
        [HttpPut("{id}/onayla")]
        public async Task<IActionResult> Onayla(int id, [FromBody] int onaylayanSicilNo)
        {
            var result = await _service.OnaylaDevamsizlikAsync(id, onaylayanSicilNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Devamsızlık/Mazeret sil
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteDevamsizlikAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

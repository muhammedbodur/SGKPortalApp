using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/sgm-mesai")]
    public class SgmMesaiController : ControllerBase
    {
        private readonly ISgmMesaiService _service;

        public SgmMesaiController(ISgmMesaiService service)
        {
            _service = service;
        }

        /// <summary>
        /// SGM bazlı toplu mesai raporu
        /// </summary>
        [HttpPost("rapor")]
        public async Task<IActionResult> GetSgmMesaiReport([FromBody] SgmMesaiFilterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.GetSgmMesaiReportAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

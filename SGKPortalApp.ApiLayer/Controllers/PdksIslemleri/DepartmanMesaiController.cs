using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/departman-mesai")]
    public class DepartmanMesaiController : ControllerBase
    {
        private readonly IDepartmanMesaiService _service;

        public DepartmanMesaiController(IDepartmanMesaiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Departman bazlı toplu mesai raporu
        /// </summary>
        [HttpPost("rapor")]
        public async Task<IActionResult> GetDepartmanMesaiReport([FromBody] DepartmanMesaiFilterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.GetDepartmanMesaiReportAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

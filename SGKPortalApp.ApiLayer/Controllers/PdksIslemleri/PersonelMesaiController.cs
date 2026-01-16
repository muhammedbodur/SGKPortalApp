using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/personel-mesai")]
    public class PersonelMesaiController : ControllerBase
    {
        private readonly IPersonelMesaiService _service;

        public PersonelMesaiController(IPersonelMesaiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Personel mesai listesi
        /// </summary>
        [HttpPost("liste")]
        public async Task<IActionResult> GetMesaiList([FromBody] PersonelMesaiFilterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.GetPersonelMesaiListAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personel başlık bilgisi
        /// </summary>
        [HttpGet("baslik/{tcKimlikNo}")]
        public async Task<IActionResult> GetBaslikBilgi(string tcKimlikNo)
        {
            var result = await _service.GetPersonelBaslikBilgiAsync(tcKimlikNo);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

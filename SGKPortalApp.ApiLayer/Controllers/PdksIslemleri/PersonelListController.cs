using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/personel-list")]
    public class PersonelListController : ControllerBase
    {
        private readonly IPersonelListService _service;

        public PersonelListController(IPersonelListService service)
        {
            _service = service;
        }

        /// <summary>
        /// Personel listesi (yetki kontrolü ile)
        /// </summary>
        [HttpPost("liste")]
        public async Task<IActionResult> GetPersonelList([FromBody] PersonelListFilterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Get current user TC from claims
            var currentUserTc = "12345678901";

            var result = await _service.GetPersonelListAsync(request, currentUserTc);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Personel aktif/pasif durumu güncelleme
        /// </summary>
        [HttpPut("toggle-aktif")]
        public async Task<IActionResult> ToggleAktifDurum([FromBody] PersonelAktifDurumUpdateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Get current user TC from claims
            var currentUserTc = "12345678901";

            var result = await _service.UpdatePersonelAktifDurumAsync(request, currentUserTc);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

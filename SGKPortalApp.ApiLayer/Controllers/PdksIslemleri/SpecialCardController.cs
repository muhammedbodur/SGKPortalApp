using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [Authorize]
    [ApiController]
    [Route("api/pdks/special-cards")]
    public class SpecialCardController : ControllerBase
    {
        private readonly ISpecialCardBusinessService _specialCardService;

        public SpecialCardController(ISpecialCardBusinessService specialCardService)
        {
            _specialCardService = specialCardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _specialCardService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _specialCardService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("card-number/{cardNumber}")]
        public async Task<IActionResult> GetByCardNumber(long cardNumber)
        {
            var result = await _specialCardService.GetByCardNumberAsync(cardNumber);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("enroll-number/{enrollNumber}")]
        public async Task<IActionResult> GetByEnrollNumber(string enrollNumber)
        {
            var result = await _specialCardService.GetByEnrollNumberAsync(enrollNumber);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("type/{cardType}")]
        public async Task<IActionResult> GetByCardType(CardType cardType)
        {
            var result = await _specialCardService.GetByCardTypeAsync(cardType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialCardCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _specialCardService.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpecialCardUpdateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _specialCardService.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _specialCardService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ========== Device Operations ==========

        [HttpPost("{id}/send-to-device/{deviceId}")]
        public async Task<IActionResult> SendCardToDevice(int id, int deviceId)
        {
            var result = await _specialCardService.SendCardToDeviceAsync(id, deviceId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/send-to-all-devices")]
        public async Task<IActionResult> SendCardToAllDevices(int id)
        {
            var result = await _specialCardService.SendCardToAllDevicesAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/delete-from-device/{deviceId}")]
        public async Task<IActionResult> DeleteCardFromDevice(int id, int deviceId)
        {
            var result = await _specialCardService.DeleteCardFromDeviceAsync(id, deviceId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/delete-from-all-devices")]
        public async Task<IActionResult> DeleteCardFromAllDevices(int id)
        {
            var result = await _specialCardService.DeleteCardFromAllDevicesAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

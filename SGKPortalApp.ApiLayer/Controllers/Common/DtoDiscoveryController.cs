using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;

namespace SGKPortalApp.ApiLayer.Controllers.Common
{
    /// <summary>
    /// DTO Discovery Controller - DTO tiplerini ve property'lerini keşfeder
    /// Business logic DtoDiscoveryService'de
    /// </summary>
    [Route("api/dtodiscovery")]
    [ApiController]
    public class DtoDiscoveryController : ControllerBase
    {
        private readonly IDtoDiscoveryService _dtoDiscoveryService;

        public DtoDiscoveryController(IDtoDiscoveryService dtoDiscoveryService)
        {
            _dtoDiscoveryService = dtoDiscoveryService;
        }

        /// <summary>
        /// Tüm *RequestDto sınıflarını listeler
        /// </summary>
        [HttpGet("dto-types")]
        public IActionResult GetAllDtoTypes()
        {
            var result = _dtoDiscoveryService.GetAllDtoTypes();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Belirtilen DTO'nun tüm property'lerini döner
        /// </summary>
        [HttpGet("dto-properties/{dtoTypeName}")]
        public IActionResult GetDtoProperties(string dtoTypeName)
        {
            var result = _dtoDiscoveryService.GetDtoProperties(dtoTypeName);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Field Analysis - DTO'daki tüm field'ları + hangilerinin korumalı olduğunu döner
        /// </summary>
        [HttpGet("field-analysis")]
        public async Task<IActionResult> GetFieldAnalysis(string pageKey, string dtoTypeName)
        {
            var result = await _dtoDiscoveryService.GetFieldAnalysisAsync(pageKey, dtoTypeName);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

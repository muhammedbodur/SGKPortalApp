using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/pdks/attendance")]
    public class ZKTecoAttendanceController : ControllerBase
    {
        private readonly IZKTecoAttendanceService _attendanceService;

        public ZKTecoAttendanceController(IZKTecoAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceFilterDto? filter = null)
        {
            var records = await _attendanceService.GetAttendanceRecordsAsync(filter);
            return Ok(records);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var count = await _attendanceService.GetAttendanceCountAsync();
            return Ok(new { Count = count });
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var stats = await _attendanceService.GetStatisticsAsync(startDate, endDate);
            return Ok(stats);
        }

        [HttpGet("device/{deviceId}/from-device")]
        public async Task<IActionResult> GetFromDevice(int deviceId)
        {
            var records = await _attendanceService.GetRecordsFromDeviceAsync(deviceId);
            return Ok(records);
        }

        [HttpPost("device/{deviceId}/sync")]
        public async Task<IActionResult> SyncFromDevice(int deviceId)
        {
            var success = await _attendanceService.SyncRecordsFromDeviceToDbAsync(deviceId);
            return Ok(new { Success = success });
        }

        [HttpDelete("device/{deviceId}/clear")]
        public async Task<IActionResult> ClearDevice(int deviceId)
        {
            var success = await _attendanceService.ClearRecordsFromDeviceAsync(deviceId);
            return Ok(new { Success = success });
        }

        [HttpGet("device/{deviceId}/count")]
        public async Task<IActionResult> GetDeviceCount(int deviceId)
        {
            var count = await _attendanceService.GetRecordCountFromDeviceAsync(deviceId);
            return Ok(new { Count = count });
        }
    }
}

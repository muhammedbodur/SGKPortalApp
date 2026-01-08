using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceBusinessService _deviceService;

        public DeviceController(IDeviceBusinessService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null) return NotFound();
            return Ok(device);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var devices = await _deviceService.GetActiveDevicesAsync();
            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Device device)
        {
            var created = await _deviceService.CreateDeviceAsync(device);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Device device)
        {
            device.Id = id;
            var updated = await _deviceService.UpdateDeviceAsync(device);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _deviceService.DeleteDeviceAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var status = await _deviceService.GetDeviceStatusAsync(id);
            if (status == null) return NotFound();
            return Ok(status);
        }

        [HttpPost("{id}/test")]
        public async Task<IActionResult> TestConnection(int id)
        {
            var success = await _deviceService.TestConnectionAsync(id);
            return Ok(new { Success = success });
        }

        [HttpGet("{id}/time")]
        public async Task<IActionResult> GetTime(int id)
        {
            var time = await _deviceService.GetDeviceTimeAsync(id);
            if (time == null) return NotFound();
            return Ok(time);
        }

        [HttpPost("{id}/time/sync")]
        public async Task<IActionResult> SyncTime(int id)
        {
            var success = await _deviceService.SynchronizeDeviceTimeAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var success = await _deviceService.EnableDeviceAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var success = await _deviceService.DisableDeviceAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/restart")]
        public async Task<IActionResult> Restart(int id)
        {
            var success = await _deviceService.RestartDeviceAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/poweroff")]
        public async Task<IActionResult> PowerOff(int id)
        {
            var success = await _deviceService.PowerOffDeviceAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/time")]
        public async Task<IActionResult> SetTime(int id, [FromBody] DateTime? dateTime = null)
        {
            var success = await _deviceService.SetDeviceTimeAsync(id, dateTime);
            return Ok(new { Success = success });
        }

        // ========== User Management ==========

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetDeviceUsers(int id)
        {
            var users = await _deviceService.GetDeviceUsersAsync(id);
            return Ok(users);
        }

        [HttpGet("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> GetDeviceUser(int id, string enrollNumber)
        {
            var user = await _deviceService.GetDeviceUserAsync(id, enrollNumber);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("{id}/users/card/{cardNumber}")]
        public async Task<IActionResult> GetDeviceUserByCard(int id, long cardNumber)
        {
            var user = await _deviceService.GetDeviceUserByCardAsync(id, cardNumber);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("users/card/search")]
        public async Task<IActionResult> SearchUserByCard([FromBody] CardSearchRequest request)
        {
            var result = await _deviceService.SearchUserByCardAsync(request);
            return Ok(result);
        }

        [HttpPost("users/card/{cardNumber}/search-all")]
        public async Task<IActionResult> SearchUserByCardOnAllDevices(long cardNumber)
        {
            var result = await _deviceService.SearchUserByCardOnAllDevicesAsync(cardNumber);
            return Ok(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> CreateDeviceUser(int id, [FromBody] SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco.UserCreateUpdateDto request, [FromQuery] bool force = false)
        {
            var success = await _deviceService.CreateDeviceUserAsync(id, request, force);
            return Ok(new { Success = success });
        }

        [HttpPut("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> UpdateDeviceUser(int id, string enrollNumber, [FromBody] SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco.UserCreateUpdateDto request, [FromQuery] bool force = false)
        {
            var success = await _deviceService.UpdateDeviceUserAsync(id, enrollNumber, request, force);
            return Ok(new { Success = success });
        }

        [HttpDelete("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> DeleteDeviceUser(int id, string enrollNumber)
        {
            var success = await _deviceService.DeleteDeviceUserAsync(id, enrollNumber);
            return Ok(new { Success = success });
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> ClearAllDeviceUsers(int id)
        {
            var success = await _deviceService.ClearAllDeviceUsersAsync(id);
            return Ok(new { Success = success });
        }

        [HttpGet("{id}/users/count")]
        public async Task<IActionResult> GetDeviceUserCount(int id)
        {
            var count = await _deviceService.GetDeviceUserCountAsync(id);
            return Ok(new { Count = count });
        }

        [HttpDelete("{id}/users/{enrollNumber}/card")]
        public async Task<IActionResult> RemoveCardFromUser(int id, string enrollNumber)
        {
            var success = await _deviceService.RemoveCardFromUserAsync(id, enrollNumber);
            return Ok(new { Success = success });
        }

        // ========== Attendance Management ==========

        [HttpGet("{id}/attendance")]
        public async Task<IActionResult> GetAttendanceLogs(int id)
        {
            var logs = await _deviceService.GetAttendanceLogsAsync(id);
            return Ok(logs);
        }

        [HttpDelete("{id}/attendance")]
        public async Task<IActionResult> ClearAttendanceLogs(int id)
        {
            var success = await _deviceService.ClearAttendanceLogsAsync(id);
            return Ok(new { Success = success });
        }

        [HttpGet("{id}/attendance/count")]
        public async Task<IActionResult> GetAttendanceLogCount(int id)
        {
            var count = await _deviceService.GetAttendanceLogCountAsync(id);
            return Ok(new { Count = count });
        }

        // ========== Realtime Monitoring ==========

        [HttpPost("{id}/monitoring/start")]
        public async Task<IActionResult> StartMonitoring(int id)
        {
            var success = await _deviceService.StartRealtimeMonitoringAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/monitoring/stop")]
        public async Task<IActionResult> StopMonitoring(int id)
        {
            var success = await _deviceService.StopRealtimeMonitoringAsync(id);
            return Ok(new { Success = success });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Services.ZKTeco;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZKTecoDeviceController : ControllerBase
    {
        private readonly IZKTecoDeviceService _deviceService;

        public ZKTecoDeviceController(IZKTecoDeviceService deviceService)
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
        public async Task<IActionResult> Create([FromBody] ZKTecoDevice device)
        {
            var created = await _deviceService.CreateDeviceAsync(device);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ZKTecoDevice device)
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

        [HttpPost("{id}/restart")]
        public async Task<IActionResult> Restart(int id)
        {
            var success = await _deviceService.RestartDeviceAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/monitoring/start")]
        public async Task<IActionResult> StartMonitoring(int id)
        {
            var success = await _deviceService.StartMonitoringAsync(id);
            return Ok(new { Success = success });
        }

        [HttpPost("{id}/monitoring/stop")]
        public async Task<IActionResult> StopMonitoring(int id)
        {
            var success = await _deviceService.StopMonitoringAsync(id);
            return Ok(new { Success = success });
        }
    }
}

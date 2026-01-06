using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Services.ZKTeco;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZKTecoUserController : ControllerBase
    {
        private readonly IZKTecoUserService _userService;

        public ZKTecoUserController(IZKTecoUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDevice(int deviceId)
        {
            var users = await _userService.GetUsersByDeviceIdAsync(deviceId);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateZKTecoUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateZKTecoUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("device/{deviceId}/from-device")]
        public async Task<IActionResult> GetFromDevice(int deviceId)
        {
            var users = await _userService.GetUsersFromDeviceAsync(deviceId);
            return Ok(users);
        }

        [HttpPost("{userId}/sync-to-device/{deviceId}")]
        public async Task<IActionResult> SyncToDevice(int userId, int deviceId)
        {
            var success = await _userService.SyncUserToDeviceAsync(userId, deviceId);
            return Ok(new { Success = success });
        }

        [HttpPost("{userId}/sync-to-all-devices")]
        public async Task<IActionResult> SyncToAllDevices(int userId)
        {
            var success = await _userService.SyncUserToAllDevicesAsync(userId);
            return Ok(new { Success = success });
        }

        [HttpPost("device/{deviceId}/sync-from-device")]
        public async Task<IActionResult> SyncFromDevice(int deviceId)
        {
            var success = await _userService.SyncUsersFromDeviceToDbAsync(deviceId);
            return Ok(new { Success = success });
        }

        [HttpDelete("{enrollNumber}/delete-from-device/{deviceId}")]
        public async Task<IActionResult> DeleteFromDevice(string enrollNumber, int deviceId)
        {
            var success = await _userService.DeleteUserFromDeviceAsync(deviceId, enrollNumber);
            return Ok(new { Success = success });
        }

        [HttpDelete("{enrollNumber}/delete-from-all-devices")]
        public async Task<IActionResult> DeleteFromAllDevices(string enrollNumber)
        {
            var success = await _userService.DeleteUserFromAllDevicesAsync(enrollNumber);
            return Ok(new { Success = success });
        }

        [HttpDelete("device/{deviceId}/clear")]
        public async Task<IActionResult> ClearDevice(int deviceId)
        {
            var success = await _userService.ClearAllUsersFromDeviceAsync(deviceId);
            return Ok(new { Success = success });
        }
    }
}

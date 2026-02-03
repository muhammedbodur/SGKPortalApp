using Microsoft.AspNetCore.Mvc;
using SGKPortalApp.ApiLayer.Services.State;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Shared.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Controllers.PdksIslemleri
{
    [ApiController]
    [Route("api/pdks/device")]
    public class ZKTecoDeviceController : ControllerBase
    {
        private readonly IDeviceBusinessService _deviceService;
        private readonly DeviceMonitoringStateService _monitoringState;

        public ZKTecoDeviceController(
            IDeviceBusinessService deviceService,
            DeviceMonitoringStateService monitoringState)
        {
            _deviceService = deviceService;
            _monitoringState = monitoringState;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deviceService.GetAllDevicesAsync();

            // Monitoring durumunu set et
            if (result.Success && result.Data != null)
            {
                foreach (var device in result.Data)
                {
                    device.IsMonitoring = _monitoringState.IsMonitoring(device.DeviceId);
                }
            }

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _deviceService.GetDeviceByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _deviceService.GetActiveDevicesAsync();
            
            // Monitoring durumunu set et
            if (result.Success && result.Data != null)
            {
                foreach (var device in result.Data)
                {
                    device.IsMonitoring = _monitoringState.IsMonitoring(device.DeviceId);
                }
            }
            
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Device device)
        {
            var result = await _deviceService.CreateDeviceAsync(device);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.DeviceId }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Device device)
        {
            device.DeviceId = id;
            var result = await _deviceService.UpdateDeviceAsync(device);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _deviceService.DeleteDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var result = await _deviceService.GetDeviceStatusAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/test")]
        public async Task<IActionResult> TestConnection(int id)
        {
            var result = await _deviceService.TestConnectionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/time")]
        public async Task<IActionResult> GetTime(int id)
        {
            var result = await _deviceService.GetDeviceTimeAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/time/sync")]
        public async Task<IActionResult> SyncTime(int id)
        {
            var result = await _deviceService.SynchronizeDeviceTimeAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _deviceService.EnableDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _deviceService.DisableDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/restart")]
        public async Task<IActionResult> Restart(int id)
        {
            var result = await _deviceService.RestartDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/poweroff")]
        public async Task<IActionResult> PowerOff(int id)
        {
            var result = await _deviceService.PowerOffDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/time")]
        public async Task<IActionResult> SetTime(int id, [FromBody] DateTime? dateTime = null)
        {
            var result = await _deviceService.SetDeviceTimeAsync(id, dateTime);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ========== User Management ==========

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetDeviceUsers(int id)
        {
            var users = await _deviceService.GetDeviceUsersAsync(id);
            var result = ApiResponseDto<List<ApiUserDto>>.SuccessResult(users, "Cihaz kullanıcıları başarıyla getirildi");
            return Ok(result);
        }

        [HttpGet("{id}/users/with-mismatches")]
        public async Task<IActionResult> GetDeviceUsersWithMismatches(int id)
        {
            var matches = await _deviceService.GetAllDeviceUsersWithMismatchInfoAsync(id);
            var result = ApiResponseDto<List<DeviceUserMatch>>.SuccessResult(matches, "Cihaz kullanıcıları uyumsuzluk bilgisiyle getirildi");
            return Ok(result);
        }

        [HttpGet("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> GetDeviceUser(int id, string enrollNumber)
        {
            var user = await _deviceService.GetDeviceUserAsync(id, enrollNumber);
            if (user == null)
                return NotFound(ApiResponseDto<ApiUserDto>.ErrorResult("Kullanıcı bulunamadı"));
            
            var result = ApiResponseDto<ApiUserDto>.SuccessResult(user, "Kullanıcı başarıyla getirildi");
            return Ok(result);
        }

        [HttpGet("{id}/users/card/{cardNumber}")]
        public async Task<IActionResult> GetDeviceUserByCard(int id, long cardNumber)
        {
            var user = await _deviceService.GetDeviceUserByCardAsync(id, cardNumber);
            if (user == null)
                return NotFound(ApiResponseDto<ApiUserDto>.ErrorResult("Kullanıcı bulunamadı"));
            
            var result = ApiResponseDto<ApiUserDto>.SuccessResult(user, "Kullanıcı başarıyla getirildi");
            return Ok(result);
        }

        [HttpGet("{id}/users/{enrollNumber}/with-mismatch")]
        public async Task<IActionResult> GetDeviceUserWithMismatchInfo(int id, string enrollNumber)
        {
            var match = await _deviceService.GetDeviceUserWithMismatchInfoAsync(id, enrollNumber);
            var result = ApiResponseDto<DeviceUserMatch>.SuccessResult(match, "Kullanıcı eşleşme bilgisi getirildi");
            return Ok(result);
        }

        [HttpGet("{id}/users/card/{cardNumber}/with-mismatch")]
        public async Task<IActionResult> GetDeviceUserByCardWithMismatchInfo(int id, long cardNumber)
        {
            var match = await _deviceService.GetDeviceUserByCardWithMismatchInfoAsync(id, cardNumber);
            var result = ApiResponseDto<DeviceUserMatch>.SuccessResult(match, "Kullanıcı eşleşme bilgisi getirildi");
            return Ok(result);
        }

        [HttpPost("users/card/search")]
        public async Task<IActionResult> SearchUserByCard([FromBody] CardSearchRequest request)
        {
            var searchResult = await _deviceService.SearchUserByCardAsync(request);
            var result = ApiResponseDto<CardSearchResponse>.SuccessResult(searchResult, searchResult.Message);
            return searchResult.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("users/card/{cardNumber}/search-all")]
        public async Task<IActionResult> SearchUserByCardOnAllDevices(long cardNumber)
        {
            var searchResult = await _deviceService.SearchUserByCardOnAllDevicesAsync(cardNumber);
            var result = ApiResponseDto<CardSearchResponse>.SuccessResult(searchResult, searchResult.Message);
            return searchResult.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> CreateDeviceUser(int id, [FromBody] UserCreateUpdateDto request, [FromQuery] bool force = false)
        {
            var success = await _deviceService.CreateDeviceUserAsync(id, request, force);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Kullanıcı başarıyla oluşturuldu" : "Kullanıcı oluşturulamadı");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> UpdateDeviceUser(int id, string enrollNumber, [FromBody] UserCreateUpdateDto request, [FromQuery] bool force = false)
        {
            var success = await _deviceService.UpdateDeviceUserAsync(id, enrollNumber, request, force);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Kullanıcı başarıyla güncellendi" : "Kullanıcı güncellenemedi");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users/{enrollNumber}")]
        public async Task<IActionResult> DeleteDeviceUser(int id, string enrollNumber)
        {
            var success = await _deviceService.DeleteDeviceUserAsync(id, enrollNumber);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Kullanıcı başarıyla silindi" : "Kullanıcı silinemedi");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> ClearAllDeviceUsers(int id)
        {
            var success = await _deviceService.ClearAllDeviceUsersAsync(id);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Tüm kullanıcılar başarıyla silindi" : "Kullanıcılar silinemedi");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/users/count")]
        public async Task<IActionResult> GetDeviceUserCount(int id)
        {
            var count = await _deviceService.GetDeviceUserCountAsync(id);
            var result = ApiResponseDto<int>.SuccessResult(count, "Kullanıcı sayısı başarıyla getirildi");
            return Ok(result);
        }

        [HttpDelete("{id}/users/{enrollNumber}/card")]
        public async Task<IActionResult> RemoveCardFromUser(int id, string enrollNumber)
        {
            var success = await _deviceService.RemoveCardFromUserAsync(id, enrollNumber);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Kart başarıyla kaldırıldı" : "Kart kaldırılamadı");
            return success ? Ok(result) : BadRequest(result);
        }

        // ========== Attendance Management ==========

        [HttpGet("{id}/attendance")]
        public async Task<IActionResult> GetAttendanceLogs(int id)
        {
            var logs = await _deviceService.GetAttendanceLogsAsync(id);
            var result = ApiResponseDto<List<AttendanceLogDto>>.SuccessResult(logs, "Devam kayıtları başarıyla getirildi");
            return Ok(result);
        }

        [HttpDelete("{id}/attendance")]
        public async Task<IActionResult> ClearAttendanceLogs(int id)
        {
            var success = await _deviceService.ClearAttendanceLogsAsync(id);
            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Devam kayıtları başarıyla temizlendi" : "Devam kayıtları temizlenemedi");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/attendance/count")]
        public async Task<IActionResult> GetAttendanceLogCount(int id)
        {
            var count = await _deviceService.GetAttendanceLogCountAsync(id);
            var result = ApiResponseDto<int>.SuccessResult(count, "Devam kayıt sayısı başarıyla getirildi");
            return Ok(result);
        }

        // ========== Realtime Monitoring ==========

        [HttpPost("{id}/monitoring/start")]
        public async Task<IActionResult> StartMonitoring(int id)
        {
            var success = await _deviceService.StartRealtimeMonitoringAsync(id);

            // State'i güncelle
            if (success)
            {
                _monitoringState.StartMonitoring(id);
            }

            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Canlı izleme başlatıldı" : "Canlı izleme başlatılamadı");
            return success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/monitoring/stop")]
        public async Task<IActionResult> StopMonitoring(int id)
        {
            var success = await _deviceService.StopRealtimeMonitoringAsync(id);

            // State'i güncelle
            if (success)
            {
                _monitoringState.StopMonitoring(id);
            }

            var result = ApiResponseDto<bool>.SuccessResult(success, success ? "Canlı izleme durduruldu" : "Canlı izleme durdurulamadı");
            return success ? Ok(result) : BadRequest(result);
        }
    }
}

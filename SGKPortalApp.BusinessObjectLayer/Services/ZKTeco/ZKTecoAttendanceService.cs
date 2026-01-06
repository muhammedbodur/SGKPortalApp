using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    public class ZKTecoAttendanceService : IZKTecoAttendanceService
    {
        private readonly IZKTecoApiClient _apiClient;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<ZKTecoAttendanceService> _logger;

        public ZKTecoAttendanceService(
            IZKTecoApiClient apiClient,
            IDeviceService deviceService,
            ILogger<ZKTecoAttendanceService> logger)
        {
            _apiClient = apiClient;
            _deviceService = deviceService;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public Task<List<CekilenData>> GetAttendanceRecordsAsync(AttendanceFilterDto? filter = null)
        {
            throw new NotImplementedException("This method should be implemented in BusinessLogicLayer with repository access");
        }

        public Task<int> GetAttendanceCountAsync()
        {
            throw new NotImplementedException("This method should be implemented in BusinessLogicLayer with repository access");
        }

        public Task<AttendanceStatisticsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException("This method should be implemented in BusinessLogicLayer with repository access");
        }

        // ========== Device Sync Operations ==========

        public async Task<List<AttendanceRecordDto>> GetRecordsFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return new List<AttendanceRecordDto>();

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetAttendanceLogsFromDeviceAsync(device.IpAddress, port);
        }

        public Task<bool> SyncRecordsFromDeviceToDbAsync(int deviceId)
        {
            // This method needs to be implemented in BusinessLogicLayer with repository access
            throw new NotImplementedException("Database operations should be in BusinessLogicLayer");
        }

        public async Task<bool> ClearRecordsFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.ClearAttendanceLogsFromDeviceAsync(device.IpAddress, port);
        }

        public async Task<int> GetRecordCountFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return 0;

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetAttendanceLogCountFromDeviceAsync(device.IpAddress, port);
        }
    }
}

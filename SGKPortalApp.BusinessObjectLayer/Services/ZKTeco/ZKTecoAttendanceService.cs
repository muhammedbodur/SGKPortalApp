using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    public class ZKTecoAttendanceService : IZKTecoAttendanceService
    {
        private readonly SGKDbContext _dbContext;
        private readonly IZKTecoApiClient _apiClient;
        private readonly IZKTecoDeviceService _deviceService;
        private readonly ILogger<ZKTecoAttendanceService> _logger;

        public ZKTecoAttendanceService(
            SGKDbContext dbContext,
            IZKTecoApiClient apiClient,
            IZKTecoDeviceService deviceService,
            ILogger<ZKTecoAttendanceService> logger)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _deviceService = deviceService;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public async Task<List<CekilenData>> GetAttendanceRecordsAsync(AttendanceFilterDto? filter = null)
        {
            var query = _dbContext.CekilenDatalar.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.EnrollNumber))
                    query = query.Where(r => r.KayitNo == filter.EnrollNumber);

                if (filter.StartDate.HasValue)
                    query = query.Where(r => r.Tarih >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(r => r.Tarih <= filter.EndDate.Value);

                if (!string.IsNullOrEmpty(filter.DeviceIp))
                    query = query.Where(r => r.CihazIp == filter.DeviceIp);
            }

            return await query
                .OrderByDescending(r => r.Tarih)
                .ThenByDescending(r => r.Saat)
                .Take(filter?.PageSize ?? 100)
                .ToListAsync();
        }

        public async Task<int> GetAttendanceCountAsync()
        {
            return await _dbContext.CekilenDatalar.CountAsync();
        }

        public async Task<AttendanceStatisticsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.CekilenDatalar.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.Tarih >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.Tarih <= endDate.Value);

            var records = await query.ToListAsync();

            return new AttendanceStatisticsDto
            {
                TotalRecords = records.Count,
                FirstRecord = records.MinBy(r => r.Tarih)?.Tarih,
                LastRecord = records.MaxBy(r => r.Tarih)?.Tarih,
                UniqueUserCount = records.Select(r => r.KayitNo).Distinct().Count(),
                CheckInCount = records.Count(r => r.GirisCikisModu == "0"),
                CheckOutCount = records.Count(r => r.GirisCikisModu == "1"),
                BreakCount = records.Count(r => r.GirisCikisModu == "2" || r.GirisCikisModu == "3")
            };
        }

        // ========== Device Sync Operations ==========

        public async Task<List<AttendanceRecordDto>> GetRecordsFromDeviceAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return new List<AttendanceRecordDto>();

            var port = int.TryParse(device.Port, out var p) ? p : 4370;
            return await _apiClient.GetAttendanceLogsFromDeviceAsync(device.IpAddress, port);
        }

        public async Task<bool> SyncRecordsFromDeviceToDbAsync(int deviceId)
        {
            var device = await _deviceService.GetDeviceByIdAsync(deviceId);
            if (device == null) return false;

            var records = await GetRecordsFromDeviceAsync(deviceId);

            foreach (var record in records)
            {
                // Check if record already exists
                var exists = await _dbContext.CekilenDatalar
                    .AnyAsync(r =>
                        r.KayitNo == record.EnrollNumber &&
                        r.Tarih == record.EventTime.Date &&
                        r.Saat == record.EventTime.TimeOfDay);

                if (!exists)
                {
                    var cekilenData = new CekilenData
                    {
                        KayitNo = record.EnrollNumber,
                        Tarih = record.EventTime.Date,
                        Saat = record.EventTime.TimeOfDay,
                        Dogrulama = record.VerifyMethod.ToString(),
                        GirisCikisModu = record.InOutMode.ToString(),
                        CihazIp = record.DeviceIp,
                        DeviceId = deviceId,
                        CekilmeTarihi = DateTime.Now,
                        IsProcessed = false
                    };

                    _dbContext.CekilenDatalar.Add(cekilenData);
                }
            }

            var savedCount = await _dbContext.SaveChangesAsync();

            // Update device sync info
            device.LastSyncTime = DateTime.Now;
            device.LastSyncSuccess = true;
            device.SyncCount++;
            device.LastSyncStatus = $"{savedCount} kayıt senkronize edildi";
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Synced {savedCount} attendance records from device {deviceId}");
            return true;
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

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Base;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.PdksIslemleri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessLogicLayer.Services.PdksIslemleri
{
    public class ZKTecoAttendanceService : IZKTecoAttendanceService
    {
        private readonly IZKTecoApiClient _apiClient;
        private readonly IDeviceService _deviceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ZKTecoAttendanceService> _logger;

        public ZKTecoAttendanceService(
            IZKTecoApiClient apiClient,
            IDeviceService deviceService,
            IUnitOfWork unitOfWork,
            ILogger<ZKTecoAttendanceService> logger)
        {
            _apiClient = apiClient;
            _deviceService = deviceService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ========== Database Operations ==========

        public Task<List<CekilenData>> GetAttendanceRecordsAsync(AttendanceFilterDto? filter = null)
        {
            throw new NotImplementedException("This method should be implemented with repository access");
        }

        public Task<int> GetAttendanceCountAsync()
        {
            throw new NotImplementedException("This method should be implemented with repository access");
        }

        public Task<AttendanceStatisticsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException("This method should be implemented with repository access");
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
            try
            {
                // Cihaz bilgilerini al
                var device = await _deviceService.GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    _logger.LogWarning($"Device not found: {deviceId}");
                    return false;
                }

                _logger.LogInformation($"Syncing attendance records from device: {device.DeviceName} ({device.IpAddress})");

                // Cihazdan kayıtları çek
                var records = await GetRecordsFromDeviceAsync(deviceId);
                if (records == null || !records.Any())
                {
                    _logger.LogInformation($"No records found on device: {device.DeviceName}");
                    return true; // Kayıt yok ama hata da yok
                }

                _logger.LogInformation($"Found {records.Count} records on device: {device.DeviceName}");

                // Repository'yi al
                var cekilenDataRepository = _unitOfWork.GetRepository<ICekilenDataRepository>();

                // Duplicate check ile kayıtları filtrele
                var newRecords = new List<CekilenData>();
                int duplicateCount = 0;

                foreach (var record in records)
                {
                    // Tarih zaten DateTime tipinde, direkt kullan
                    var tarih = record.EventTime;

                    // Duplicate check (KayitNo + Tarih unique constraint)
                    var exists = await cekilenDataRepository.ExistsByKayitNoAndTarihAsync(record.EnrollNumber, tarih);
                    if (exists)
                    {
                        duplicateCount++;
                        continue; // Bu kayıt zaten var, atla
                    }

                    // Yeni kayıt oluştur
                    var cekilenData = new CekilenData
                    {
                        KayitNo = record.EnrollNumber,
                        Tarih = tarih,
                        Dogrulama = record.VerifyMethod.ToString(),
                        GirisCikisModu = record.InOutMode.ToString(),
                        WorkCode = record.WorkCode.ToString(),
                        Reserved = "0",
                        DeviceId = deviceId,
                        CihazIp = device.IpAddress,
                        CekilmeTarihi = DateTime.Now,
                        IsProcessed = false
                    };

                    newRecords.Add(cekilenData);
                }

                // Yeni kayıtları toplu olarak ekle
                if (newRecords.Any())
                {
                    var insertedCount = await cekilenDataRepository.BulkInsertAsync(newRecords);
                    _logger.LogInformation($"✅ Inserted {insertedCount} new records from device: {device.DeviceName} (Duplicates skipped: {duplicateCount})");
                }
                else
                {
                    _logger.LogInformation($"ℹ️ No new records to insert from device: {device.DeviceName} (All {duplicateCount} records already exist)");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error syncing records from device {deviceId}");
                return false;
            }
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

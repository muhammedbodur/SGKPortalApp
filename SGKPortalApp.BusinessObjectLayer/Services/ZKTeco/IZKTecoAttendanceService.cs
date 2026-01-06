using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Services.ZKTeco
{
    /// <summary>
    /// ZKTeco Attendance yönetim servisi
    /// Database (CekilenDatalar) + Device (ZKTecoApi) kombinasyonu
    /// </summary>
    public interface IZKTecoAttendanceService
    {
        // ========== Database Operations ==========

        Task<List<CekilenData>> GetAttendanceRecordsAsync(AttendanceFilterDto? filter = null);
        Task<int> GetAttendanceCountAsync();
        Task<AttendanceStatisticsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        // ========== Device Sync Operations ==========

        Task<List<AttendanceRecordDto>> GetRecordsFromDeviceAsync(int deviceId);
        Task<bool> SyncRecordsFromDeviceToDbAsync(int deviceId);
        Task<bool> ClearRecordsFromDeviceAsync(int deviceId);
        Task<int> GetRecordCountFromDeviceAsync(int deviceId);
    }
}

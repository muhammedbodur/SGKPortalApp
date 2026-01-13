using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    public class DeviceCardSyncReportDto
    {
        public int TotalDevicesChecked { get; set; }
        public int TotalCardsInDevices { get; set; }
        public int TotalCardsInDatabase { get; set; }
        public int TotalMismatches { get; set; }
        public int OnlyInDeviceCount { get; set; }
        public int OnlyInDatabaseCount { get; set; }
        public int DataMismatchCount { get; set; }
        public List<CardMismatchDto> Mismatches { get; set; } = new();
        public List<DeviceSyncStatus> DeviceStatuses { get; set; } = new();
        public DateTime ReportGeneratedAt { get; set; } = DateTime.Now;
        public TimeSpan TotalProcessingTime { get; set; }
    }

    public class DeviceSyncStatus
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceIp { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int CardCount { get; set; }
        public string? ErrorMessage { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }
}

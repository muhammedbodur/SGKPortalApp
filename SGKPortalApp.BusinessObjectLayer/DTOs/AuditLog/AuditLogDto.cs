using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog
{
    /// <summary>
    /// Audit log liste görünümü için DTO
    /// </summary>
    public class AuditLogDto
    {
        public int DatabaseLogId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string TabloAdi { get; set; } = string.Empty;
        public DatabaseAction IslemTuru { get; set; }
        public DateTime IslemZamani { get; set; }
        public LogStorageType StorageType { get; set; }
        public string? FileReference { get; set; }
        public Guid? TransactionId { get; set; }
        public bool IsGroupedLog { get; set; }
        public string? ChangedFields { get; set; }
        public int? ChangedFieldCount { get; set; }
        public int? DataSizeBytes { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public int? BulkOperationCount { get; set; }

        /// <summary>
        /// İşlemin yapıldığı kişinin TC Kimlik No (entity'deki TcKimlikNo field'ından)
        /// </summary>
        public string? TargetPersonTcKimlikNo { get; set; }

        /// <summary>
        /// İşlemin yapıldığı kişinin Ad Soyad
        /// </summary>
        public string? TargetPersonAdSoyad { get; set; }

        /// <summary>
        /// İşlem yapılan entity'nin tanımlayıcı bilgisi (TcKimlikNo yoksa: ServisAdi, DepartmanAdi, vs.)
        /// </summary>
        public string? TargetEntityInfo { get; set; }

        // Display helpers
        public string IslemTuruText => IslemTuru.ToString();
        public string IslemZamaniText => IslemZamani.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
        public string StorageTypeText => StorageType switch
        {
            LogStorageType.Database => "Veritabanı",
            LogStorageType.File => "Dosya",
            LogStorageType.Summary => "Özet",
            _ => "Bilinmiyor"
        };
        public string DataSizeText => DataSizeBytes.HasValue
            ? DataSizeBytes.Value < 1024
                ? $"{DataSizeBytes.Value} B"
                : $"{DataSizeBytes.Value / 1024.0:F1} KB"
            : "-";
    }
}

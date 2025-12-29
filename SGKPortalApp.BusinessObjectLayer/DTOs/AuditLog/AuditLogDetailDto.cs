using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog
{
    /// <summary>
    /// Audit log detay görünümü için DTO (Before/After data ile)
    /// </summary>
    public class AuditLogDetailDto
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

        /// <summary>
        /// İşlemin yapıldığı kişinin TC Kimlik No (entity'deki TcKimlikNo field'ından)
        /// </summary>
        public string? TargetPersonTcKimlikNo { get; set; }

        /// <summary>
        /// İşlemin yapıldığı kişinin Ad Soyad
        /// </summary>
        public string? TargetPersonAdSoyad { get; set; }

        /// <summary>
        /// Before data (JSON veya Dictionary olarak)
        /// </summary>
        public string? BeforeDataJson { get; set; }

        /// <summary>
        /// After data (JSON veya Dictionary olarak)
        /// </summary>
        public string? AfterDataJson { get; set; }

        /// <summary>
        /// Değişen field'ların before/after değerleri
        /// </summary>
        public List<FieldChangeDto> FieldChanges { get; set; } = new();

        /// <summary>
        /// Transaction içindeki diğer log'lar (gruplu işlemlerde)
        /// </summary>
        public List<AuditLogDto>? RelatedLogs { get; set; }
    }

    /// <summary>
    /// Field değişikliği detayı
    /// </summary>
    public class FieldChangeDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        /// <summary>
        /// Eski değerin kullanıcı dostu gösterimi (FK lookup, enum açıklama, vb)
        /// </summary>
        public string? OldValueDisplay { get; set; }

        /// <summary>
        /// Yeni değerin kullanıcı dostu gösterimi (FK lookup, enum açıklama, vb)
        /// </summary>
        public string? NewValueDisplay { get; set; }

        public bool IsChanged => OldValue != NewValue;
    }
}

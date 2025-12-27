using SGKPortalApp.BusinessObjectLayer.Attributes;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGKPortalApp.BusinessObjectLayer.Entities.Common
{
    /// <summary>
    /// Database log table - IMPORTANT: NoAuditLog prevents infinite recursion
    /// </summary>
    [NoAuditLog(Reason = "This is a log table - logging the log table would cause infinite recursion")]
    public class DatabaseLog : BaseEntity
    {
        [Key]
        public int DatabaseLogId { get; set; }

        // ═══════ APPLICATION USER (Form Authentication) ═══════
        [Required]
        [StringLength(11)]
        public required string TcKimlikNo { get; set; }

        // ═══════ BASIC LOG INFO ═══════
        [Required]
        public DatabaseAction DatabaseAction { get; set; }

        [Required]
        [StringLength(100)]
        public required string TableName { get; set; }

        [Required]
        public DateTime IslemZamani { get; set; } = DateTime.Now;

        [Obsolete("Use IslemZamani instead")]
        public DateTime ActionTime { get; set; }

        // ═══════ HYBRID STORAGE ═══════
        public LogStorageType StorageType { get; set; } = LogStorageType.Database;

        [StringLength(500)]
        public string? FileReference { get; set; }

        public string? BeforeData { get; set; } // JSON (nullable, max length in configuration)
        public string? AfterData { get; set; }  // JSON (nullable, max length in configuration)

        // ═══════ TRANSACTION GROUPING ═══════
        public Guid? TransactionId { get; set; }

        public bool IsGroupedLog { get; set; }

        [StringLength(50)]
        public string? OperationType { get; set; }  // "CREATE_COMPLETE", "UPDATE_COMPLETE"

        [StringLength(1000)]
        public string? ChangeSummary { get; set; }  // "1 Personel INSERT, 3 PersonelCocuk INSERT"

        public int? TotalChangeCount { get; set; }

        public int? SaveChangesCount { get; set; }

        [StringLength(1000)]
        public string? AffectedTables { get; set; }  // JSON array: ["Personel","User","PersonelCocuk"]

        // ═══════ DOMAIN USER (Gerçek Kullanıcı - Windows Authentication) ═══════
        [StringLength(200)]
        public string? DomainUser { get; set; }  // "DOMAIN\username"

        [StringLength(100)]
        public string? MachineName { get; set; }  // "PC-12345"

        public bool IsDomainUserMismatch { get; set; }  // TcKimlikNo ile DomainUser uyumsuz mu?

        // ═══════ NETWORK & CLIENT INFO ═══════
        [StringLength(100)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        // ═══════ METADATA & STATISTICS ═══════
        public int? ChangedFieldCount { get; set; }  // Kaç alan değişti?

        public int? DataSizeBytes { get; set; }  // JSON boyutu

        public int? BulkOperationCount { get; set; }  // Bulk işlemde kaç kayıt?
    }
}

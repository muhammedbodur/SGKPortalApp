using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog
{
    /// <summary>
    /// Audit log filtreleme parametreleri
    /// </summary>
    public class AuditLogFilterDto
    {
        /// <summary>
        /// Başlangıç tarihi (UTC)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Bitiş tarihi (UTC)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Kullanıcı TC Kimlik No
        /// </summary>
        public string? TcKimlikNo { get; set; }

        /// <summary>
        /// Arama metni (Ad Soyad, Sicil No)
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Departman ID (yetki bazlı filtreleme)
        /// </summary>
        public int? DepartmanId { get; set; }

        /// <summary>
        /// Servis ID (yetki bazlı filtreleme)
        /// </summary>
        public int? ServisId { get; set; }

        /// <summary>
        /// Tablo adı
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// İşlem türü
        /// </summary>
        public DatabaseAction? Action { get; set; }

        /// <summary>
        /// Transaction ID (gruplu log'lar için)
        /// </summary>
        public Guid? TransactionId { get; set; }

        /// <summary>
        /// Sayfa numarası (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa başına kayıt sayısı
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Sadece file-based log'ları mı göster?
        /// </summary>
        public bool? OnlyFileBased { get; set; }

        /// <summary>
        /// Sadece database-based log'ları mı göster?
        /// </summary>
        public bool? OnlyDatabaseBased { get; set; }
    }
}

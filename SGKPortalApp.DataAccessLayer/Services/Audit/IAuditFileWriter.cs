using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Audit log'larını dosyaya yazma ve okuma servisi
    /// </summary>
    public interface IAuditFileWriter
    {
        /// <summary>
        /// Tek bir audit entry'yi dosyaya yazar
        /// </summary>
        /// <returns>FileReference (örnek: "2025/12/26/audit.jsonl#123")</returns>
        Task<string> WriteAsync(AuditFileEntry entry);

        /// <summary>
        /// Birden fazla audit entry'yi toplu olarak yazar
        /// </summary>
        Task<string> WriteBulkAsync(List<AuditFileEntry> entries);

        /// <summary>
        /// FileReference ile dosyadan okuma yapar
        /// </summary>
        Task<AuditFileEntry?> ReadAsync(string fileReference);

        /// <summary>
        /// Transaction ID'ye göre tüm entry'leri okur
        /// </summary>
        Task<List<AuditFileEntry>> ReadByTransactionIdAsync(Guid transactionId);
    }

    /// <summary>
    /// Dosyaya yazılacak audit entry
    /// </summary>
    public class AuditFileEntry
    {
        public int LogId { get; set; }
        public Guid? TransactionId { get; set; }
        public string? TcKimlikNo { get; set; }
        public string? TableName { get; set; }
        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }
        public object? Before { get; set; }
        public object? After { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}

using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessObjectLayer.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Audit log'larını JSONL formatında dosyaya yazan ve okuyan servis.
    /// Thread-safe file operations ile günlük dosyalara yazmayı destekler.
    /// </summary>
    public class AuditFileWriter : IAuditFileWriter
    {
        private readonly AuditLoggingOptions _options;
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        public AuditFileWriter(IOptions<AuditLoggingOptions> options)
        {
            // BasePath, AuditLoggingServiceExtensions.AddAuditLogging metodunda
            // PostConfigure ile solution root'a göre resolve edilmiştir
            _options = options.Value;
        }

        /// <summary>
        /// Tek bir audit entry'yi dosyaya yazar
        /// </summary>
        /// <returns>FileReference (örnek: "2025/12/26/audit.jsonl#123")</returns>
        public async Task<string> WriteAsync(AuditFileEntry entry)
        {
            var filePath = GetDailyFilePath(entry.Timestamp);
            var lineNumber = await AppendToFileAsync(filePath, entry);

            // FileReference format: "2025/12/26/audit.jsonl#123"
            var relativePath = GetRelativePath(filePath);
            return $"{relativePath}#{lineNumber}";
        }

        /// <summary>
        /// Birden fazla audit entry'yi toplu olarak yazar
        /// </summary>
        public async Task<string> WriteBulkAsync(List<AuditFileEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return string.Empty;

            // İlk entry'nin tarihini kullanarak dosya yolu belirle
            var timestamp = entries.First().Timestamp;
            var filePath = GetDailyFilePath(timestamp);

            await _fileLock.WaitAsync();
            try
            {
                EnsureDirectoryExists(filePath);

                // Dosyadaki mevcut satır sayısını bul
                var currentLineCount = File.Exists(filePath)
                    ? File.ReadLines(filePath).Count()
                    : 0;

                // Tüm entry'leri JSON'a çevir
                var lines = entries.Select(e => JsonSerializer.Serialize(e, _jsonOptions));

                // Append to file
                await File.AppendAllLinesAsync(filePath, lines, Encoding.UTF8);

                var startLineNumber = currentLineCount + 1;
                var endLineNumber = currentLineCount + entries.Count;
                var relativePath = GetRelativePath(filePath);

                return $"{relativePath}#{startLineNumber}-{endLineNumber}";
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// FileReference ile dosyadan okuma yapar
        /// </summary>
        public async Task<AuditFileEntry?> ReadAsync(string fileReference)
        {
            if (string.IsNullOrWhiteSpace(fileReference))
                return null;

            // FileReference format: "2025/12/26/audit.jsonl#123"
            var parts = fileReference.Split('#');
            if (parts.Length != 2)
                return null;

            var relativePath = parts[0];
            if (!int.TryParse(parts[1], out var lineNumber))
                return null;

            var filePath = Path.Combine(_options.BasePath, relativePath);

            if (!File.Exists(filePath))
                return null;

            await _fileLock.WaitAsync();
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);

                // Line numbers are 1-based
                if (lineNumber < 1 || lineNumber > lines.Length)
                    return null;

                var jsonLine = lines[lineNumber - 1];
                return JsonSerializer.Deserialize<AuditFileEntry>(jsonLine, _jsonOptions);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Transaction ID'ye göre tüm entry'leri okur
        /// </summary>
        public async Task<List<AuditFileEntry>> ReadByTransactionIdAsync(Guid transactionId)
        {
            var results = new List<AuditFileEntry>();

            // Transaction genelde aynı gün içinde olur, ama yine de 2 günü kontrol et
            var dates = new[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(-1) };

            foreach (var date in dates)
            {
                var filePath = GetDailyFilePath(date);

                if (!File.Exists(filePath))
                    continue;

                await _fileLock.WaitAsync();
                try
                {
                    var lines = await File.ReadAllLinesAsync(filePath);

                    foreach (var line in lines)
                    {
                        try
                        {
                            var entry = JsonSerializer.Deserialize<AuditFileEntry>(line, _jsonOptions);
                            if (entry?.TransactionId == transactionId)
                            {
                                results.Add(entry);
                            }
                        }
                        catch
                        {
                            // Skip malformed lines
                            continue;
                        }
                    }
                }
                finally
                {
                    _fileLock.Release();
                }
            }

            return results;
        }

        #region Private Helper Methods

        /// <summary>
        /// Tarihe göre günlük dosya yolu oluşturur
        /// </summary>
        private string GetDailyFilePath(DateTime timestamp)
        {
            // Format: /AuditLogs/2025/12/26/audit.jsonl
            var date = timestamp.Date;
            var path = Path.Combine(
                _options.BasePath,
                date.Year.ToString("0000"),
                date.Month.ToString("00"),
                date.Day.ToString("00"),
                "audit.jsonl"
            );
            return path;
        }

        /// <summary>
        /// Absolute path'den relative path'e çevirir
        /// </summary>
        private string GetRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(_options.BasePath))
            {
                return absolutePath.Substring(_options.BasePath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            return absolutePath;
        }

        /// <summary>
        /// Dosyaya thread-safe şekilde append yapar
        /// </summary>
        private async Task<int> AppendToFileAsync(string filePath, AuditFileEntry entry)
        {
            await _fileLock.WaitAsync();
            try
            {
                EnsureDirectoryExists(filePath);

                // Dosyadaki mevcut satır sayısını bul
                var currentLineCount = File.Exists(filePath)
                    ? File.ReadLines(filePath).Count()
                    : 0;

                // JSON'a serialize et
                var jsonLine = JsonSerializer.Serialize(entry, _jsonOptions);

                // Append to file
                await File.AppendAllTextAsync(filePath, jsonLine + Environment.NewLine, Encoding.UTF8);

                return currentLineCount + 1; // 1-based line number
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Klasör yoksa oluşturur
        /// </summary>
        private void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        #endregion
    }
}

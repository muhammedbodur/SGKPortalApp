using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SGKPortalApp.DataAccessLayer.Scripts
{
    /// <summary>
    /// Veritabanı script'lerini çalıştıran servis
    /// Migration sonrası View, SP, Function gibi objeleri oluşturur
    /// </summary>
    public static class DatabaseScriptRunner
    {
        // GO batch separator regex - satır başında veya sonunda olabilir
        private static readonly Regex GoBatchRegex = new Regex(
            @"^\s*GO\s*$", 
            RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Embedded SQL script'lerini çalıştırır
        /// </summary>
        public static async Task RunScriptsAsync(DbContext context, ILogger? logger = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(name => name) // Alfabetik sıra (001_, 002_ prefix'leri için)
                .ToList();

            logger?.LogInformation("📜 {Count} SQL script bulundu", resourceNames.Count);

            foreach (var resourceName in resourceNames)
            {
                try
                {
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    if (stream == null)
                    {
                        logger?.LogWarning("⚠️ Script stream null: {ResourceName}", resourceName);
                        continue;
                    }

                    using var reader = new StreamReader(stream);
                    var sql = await reader.ReadToEndAsync();

                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        logger?.LogWarning("⚠️ Script boş: {ResourceName}", resourceName);
                        continue;
                    }

                    // GO ifadelerini ayır ve her birini ayrı çalıştır
                    var batches = sql.Split(new[] { "\nGO\n", "\nGO\r\n", "\r\nGO\r\n", "\r\nGO\n" }, 
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (var batch in batches)
                    {
                        var trimmedBatch = batch.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedBatch) && !trimmedBatch.StartsWith("--"))
                        {
                            await context.Database.ExecuteSqlRawAsync(trimmedBatch);
                        }
                    }

                    logger?.LogInformation("✅ Script çalıştırıldı: {ResourceName}", GetShortName(resourceName));
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "❌ Script hatası: {ResourceName}", resourceName);
                    // Hata olsa bile devam et (view zaten var olabilir)
                }
            }

            logger?.LogInformation("✅ Tüm SQL script'leri tamamlandı");
        }

        /// <summary>
        /// Dosya sistemindeki SQL script'lerini çalıştırır
        /// </summary>
        public static async Task RunScriptsFromFolderAsync(DbContext context, string folderPath, ILogger? logger = null)
        {
            if (!Directory.Exists(folderPath))
            {
                logger?.LogWarning("⚠️ Script klasörü bulunamadı: {FolderPath}", folderPath);
                return;
            }

            var sqlFiles = Directory.GetFiles(folderPath, "*.sql", SearchOption.AllDirectories)
                .OrderBy(f => f)
                .ToList();

            logger?.LogInformation("📜 {Count} SQL script bulundu: {FolderPath}", sqlFiles.Count, folderPath);

            foreach (var sqlFile in sqlFiles)
            {
                try
                {
                    var sql = await File.ReadAllTextAsync(sqlFile);

                    if (string.IsNullOrWhiteSpace(sql))
                    {
                        logger?.LogWarning("⚠️ Script boş: {SqlFile}", sqlFile);
                        continue;
                    }

                    // GO ifadelerini regex ile ayır
                    var batches = GoBatchRegex.Split(sql);

                    foreach (var batch in batches)
                    {
                        var trimmedBatch = batch.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedBatch) && 
                            !trimmedBatch.StartsWith("--") &&
                            !trimmedBatch.Equals("GO", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                await context.Database.ExecuteSqlRawAsync(trimmedBatch);
                            }
                            catch (Exception batchEx)
                            {
                                // View/SP zaten varsa devam et
                                if (!batchEx.Message.Contains("already an object named"))
                                {
                                    throw;
                                }
                                logger?.LogWarning("⚠️ Obje zaten mevcut, atlanıyor");
                            }
                        }
                    }

                    logger?.LogInformation("✅ Script çalıştırıldı: {SqlFile}", Path.GetFileName(sqlFile));
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "❌ Script hatası: {SqlFile}", sqlFile);
                    // Hata olsa bile devam et
                }
            }

            logger?.LogInformation("✅ Tüm SQL script'leri tamamlandı");
        }

        private static string GetShortName(string resourceName)
        {
            var parts = resourceName.Split('.');
            return parts.Length >= 2 ? $"{parts[^2]}.{parts[^1]}" : resourceName;
        }
    }
}

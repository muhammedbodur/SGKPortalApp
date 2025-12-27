using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Options;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.DataAccessLayer.Services.Audit
{
    /// <summary>
    /// Audit log'ları için background maintenance service.
    /// - Eski log'ları temizler (retention policy)
    /// - Dosyaları sıkıştırır
    /// - Aylık arşiv oluşturur
    /// </summary>
    public class AuditMaintenanceService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuditMaintenanceService> _logger;
        private readonly AuditLoggingOptions _options;

        public AuditMaintenanceService(
            IServiceProvider serviceProvider,
            ILogger<AuditMaintenanceService> logger,
            IOptions<AuditLoggingOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Maintenance.Enabled)
            {
                _logger.LogInformation("Audit maintenance service is disabled");
                return;
            }

            _logger.LogInformation("Audit maintenance service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Hedef çalışma saatini hesapla
                    var targetTime = CalculateNextRunTime();
                    var delay = targetTime - DateTime.Now;

                    if (delay.TotalMilliseconds > 0)
                    {
                        _logger.LogInformation("Next maintenance run scheduled at {TargetTime}", targetTime);
                        await Task.Delay(delay, stoppingToken);
                    }

                    // Maintenance işlemlerini çalıştır
                    await RunMaintenanceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in audit maintenance service");
                }

                // Bir sonraki güne kadar bekle
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("Audit maintenance service stopped");
        }

        /// <summary>
        /// Tüm maintenance işlemlerini çalıştırır
        /// </summary>
        private async Task RunMaintenanceAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting audit maintenance tasks");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

            try
            {
                // 1. Database retention policy
                await CleanupDatabaseLogsAsync(dbContext, cancellationToken);

                // 2. File compression
                if (_options.Maintenance.AutoCompression)
                {
                    await CompressOldFilesAsync(cancellationToken);
                }

                // 3. File retention policy
                await CleanupOldFilesAsync(cancellationToken);

                // 4. Monthly archive
                if (_options.Maintenance.CreateMonthlyArchive)
                {
                    await CreateMonthlyArchiveAsync(cancellationToken);
                }

                _logger.LogInformation("Audit maintenance tasks completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during audit maintenance");
            }
        }

        /// <summary>
        /// Database'den eski log'ları siler
        /// </summary>
        private async Task CleanupDatabaseLogsAsync(SGKDbContext dbContext, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_options.Retention.DatabaseDays);

            var deletedCount = await dbContext.DatabaseLogs
                .Where(log => log.IslemZamani < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedCount > 0)
            {
                _logger.LogInformation("Cleaned up {Count} old database logs (older than {Days} days)",
                    deletedCount, _options.Retention.DatabaseDays);
            }
        }

        /// <summary>
        /// Eski dosyaları sıkıştırır
        /// </summary>
        private async Task CompressOldFilesAsync(CancellationToken cancellationToken)
        {
            var compressAfterDate = DateTime.UtcNow.AddDays(-_options.Retention.CompressAfterDays);
            var basePath = _options.BasePath;

            if (!Directory.Exists(basePath))
                return;

            var years = Directory.GetDirectories(basePath);
            foreach (var yearDir in years)
            {
                var months = Directory.GetDirectories(yearDir);
                foreach (var monthDir in months)
                {
                    var days = Directory.GetDirectories(monthDir);
                    foreach (var dayDir in days)
                    {
                        // Klasör tarihini parse et
                        var dirInfo = new DirectoryInfo(dayDir);
                        var parts = dirInfo.FullName.Replace(basePath, "").Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

                        if (parts.Length != 3)
                            continue;

                        if (!int.TryParse(parts[0], out var year) ||
                            !int.TryParse(parts[1], out var month) ||
                            !int.TryParse(parts[2], out var day))
                            continue;

                        var folderDate = new DateTime(year, month, day);

                        if (folderDate >= compressAfterDate)
                            continue;

                        // .jsonl dosyalarını sıkıştır
                        var jsonlFiles = Directory.GetFiles(dayDir, "*.jsonl");
                        foreach (var file in jsonlFiles)
                        {
                            var gzFile = file + ".gz";
                            if (File.Exists(gzFile))
                                continue; // Zaten sıkıştırılmış

                            try
                            {
                                await CompressFileAsync(file, gzFile, cancellationToken);
                                File.Delete(file); // Orijinali sil
                                _logger.LogInformation("Compressed file: {File}", file);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error compressing file: {File}", file);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Eski dosyaları siler
        /// </summary>
        private async Task CleanupOldFilesAsync(CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_options.Retention.FileDays);
            var basePath = _options.BasePath;

            if (!Directory.Exists(basePath))
                return;

            var years = Directory.GetDirectories(basePath);
            foreach (var yearDir in years)
            {
                var months = Directory.GetDirectories(yearDir);
                foreach (var monthDir in months)
                {
                    var days = Directory.GetDirectories(monthDir);
                    foreach (var dayDir in days)
                    {
                        var dirInfo = new DirectoryInfo(dayDir);
                        var parts = dirInfo.FullName.Replace(basePath, "").Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

                        if (parts.Length != 3)
                            continue;

                        if (!int.TryParse(parts[0], out var year) ||
                            !int.TryParse(parts[1], out var month) ||
                            !int.TryParse(parts[2], out var day))
                            continue;

                        var folderDate = new DateTime(year, month, day);

                        if (folderDate < cutoffDate)
                        {
                            try
                            {
                                Directory.Delete(dayDir, true);
                                _logger.LogInformation("Deleted old log folder: {Folder}", dayDir);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error deleting folder: {Folder}", dayDir);
                            }
                        }
                    }

                    // Boş month klasörünü sil
                    if (Directory.Exists(monthDir) && !Directory.EnumerateFileSystemEntries(monthDir).Any())
                    {
                        Directory.Delete(monthDir);
                    }
                }

                // Boş year klasörünü sil
                if (Directory.Exists(yearDir) && !Directory.EnumerateFileSystemEntries(yearDir).Any())
                {
                    Directory.Delete(yearDir);
                }
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Aylık arşiv oluşturur
        /// </summary>
        private async Task CreateMonthlyArchiveAsync(CancellationToken cancellationToken)
        {
            // Geçen ay için arşiv oluştur
            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var year = lastMonth.Year;
            var month = lastMonth.Month;

            var basePath = _options.BasePath;
            var monthPath = Path.Combine(basePath, year.ToString("0000"), month.ToString("00"));

            if (!Directory.Exists(monthPath))
                return;

            var archivePath = Path.Combine(basePath, "Archives");
            Directory.CreateDirectory(archivePath);

            var archiveFile = Path.Combine(archivePath, $"audit-{year}-{month:00}.zip");

            if (File.Exists(archiveFile))
                return; // Zaten oluşturulmuş

            try
            {
                ZipFile.CreateFromDirectory(monthPath, archiveFile, CompressionLevel.Optimal, false);
                _logger.LogInformation("Created monthly archive: {Archive}", archiveFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating monthly archive");
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Dosyayı gzip ile sıkıştırır
        /// </summary>
        private async Task CompressFileAsync(string sourceFile, string targetFile, CancellationToken cancellationToken)
        {
            using var sourceStream = File.OpenRead(sourceFile);
            using var targetStream = File.Create(targetFile);
            using var gzipStream = new GZipStream(targetStream, CompressionLevel.Optimal);

            await sourceStream.CopyToAsync(gzipStream, cancellationToken);
        }

        /// <summary>
        /// Bir sonraki çalışma zamanını hesaplar
        /// </summary>
        private DateTime CalculateNextRunTime()
        {
            var runTime = _options.Maintenance.RunTime; // "02:00" formatında
            var parts = runTime.Split(':');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var hour) ||
                !int.TryParse(parts[1], out var minute))
            {
                // Default: 02:00
                hour = 2;
                minute = 0;
            }

            var now = DateTime.Now;
            var targetTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

            // Eğer bugünkü saat geçmişse, yarına ayarla
            if (targetTime <= now)
            {
                targetTime = targetTime.AddDays(1);
            }

            return targetTime;
        }
    }
}

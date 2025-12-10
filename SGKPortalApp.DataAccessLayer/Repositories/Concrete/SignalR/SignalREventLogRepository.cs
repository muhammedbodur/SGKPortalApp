using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.DataAccessLayer.Context;
using SGKPortalApp.DataAccessLayer.Repositories.Generic;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SignalR;

namespace SGKPortalApp.DataAccessLayer.Repositories.Concrete.SignalR
{
    /// <summary>
    /// SignalR Event Log Repository Implementation
    /// Thread-safe: Her audit işlemi için yeni context oluşturur
    /// </summary>
    public class SignalREventLogRepository : GenericRepository<SignalREventLog>, ISignalREventLogRepository
    {
        private readonly string _connectionString;

        public SignalREventLogRepository(SGKDbContext context, IConfiguration configuration) : base(context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string bulunamadı");
        }

        /// <summary>
        /// Thread-safe yeni context oluşturur
        /// </summary>
        private SGKDbContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<SGKDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
            return new SGKDbContext(options);
        }

        public async Task<SignalREventLog> LogEventAsync(SignalREventLog eventLog)
        {
            // Thread-safe: Yeni context oluştur
            await using var context = CreateNewContext();
            await context.Set<SignalREventLog>().AddAsync(eventLog);
            await context.SaveChangesAsync();
            return eventLog;
        }

        public async Task LogEventsAsync(IEnumerable<SignalREventLog> eventLogs)
        {
            // Thread-safe: Yeni context oluştur
            await using var context = CreateNewContext();
            await context.Set<SignalREventLog>().AddRangeAsync(eventLogs);
            await context.SaveChangesAsync();
        }

        public async Task<SignalREventLog?> GetByEventIdAsync(Guid eventId)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task<List<SignalREventLog>> GetBySiraIdAsync(int siraId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(e => e.SiraId == siraId)
                .OrderByDescending(e => e.SentAt)
                .ToListAsync();
        }

        public async Task<List<SignalREventLog>> GetByBankoIdAsync(int bankoId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsNoTracking().Where(e => e.BankoId == bankoId);

            if (startDate.HasValue)
                query = query.Where(e => e.SentAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.SentAt <= endDate.Value);

            return await query.OrderByDescending(e => e.SentAt).ToListAsync();
        }

        public async Task<List<SignalREventLog>> GetByTvIdAsync(int tvId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsNoTracking().Where(e => e.TvId == tvId);

            if (startDate.HasValue)
                query = query.Where(e => e.SentAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.SentAt <= endDate.Value);

            return await query.OrderByDescending(e => e.SentAt).ToListAsync();
        }

        public async Task<List<SignalREventLog>> GetByPersonelTcAsync(string personelTc, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsNoTracking().Where(e => e.PersonelTc == personelTc);

            if (startDate.HasValue)
                query = query.Where(e => e.SentAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.SentAt <= endDate.Value);

            return await query.OrderByDescending(e => e.SentAt).ToListAsync();
        }

        public async Task<List<SignalREventLog>> GetFailedEventsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsNoTracking()
                .Where(e => e.DeliveryStatus == SignalRDeliveryStatus.Failed || 
                           e.DeliveryStatus == SignalRDeliveryStatus.NoTarget);

            if (startDate.HasValue)
                query = query.Where(e => e.SentAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.SentAt <= endDate.Value);

            return await query.OrderByDescending(e => e.SentAt).ToListAsync();
        }

        public async Task<List<SignalREventLog>> GetByCorrelationIdAsync(Guid correlationId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(e => e.CorrelationId == correlationId)
                .OrderBy(e => e.SentAt)
                .ToListAsync();
        }

        public async Task<SignalREventStatistics> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _dbSet
                .AsNoTracking()
                .Where(e => e.SentAt >= startDate && e.SentAt <= endDate)
                .ToListAsync();

            var stats = new SignalREventStatistics
            {
                TotalEvents = events.Count,
                SentCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Sent),
                FailedCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Failed),
                NoTargetCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.NoTarget),
                AcknowledgedCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Acknowledged),
                AverageDurationMs = events.Where(e => e.DurationMs.HasValue).Select(e => e.DurationMs!.Value).DefaultIfEmpty(0).Average(),
                EventTypeCounts = events.GroupBy(e => e.EventType).ToDictionary(g => g.Key, g => g.Count()),
                TargetTypeCounts = events.GroupBy(e => e.TargetType).ToDictionary(g => g.Key, g => g.Count())
            };

            return stats;
        }

        public async Task UpdateStatusDirectAsync(Guid eventId, SignalRDeliveryStatus status, string? errorMessage, int? durationMs)
        {
            // Thread-safe: Yeni context oluştur ve doğrudan SQL ile güncelle
            await using var context = CreateNewContext();
            var sql = @"UPDATE [dbo].[SIG_EventLogs] 
                        SET DeliveryStatus = @p0, 
                            ErrorMessage = @p1, 
                            DurationMs = @p2 
                        WHERE EventId = @p3";

            await context.Database.ExecuteSqlRawAsync(sql, (int)status, errorMessage, durationMs, eventId);
        }

        public async Task<bool> AcknowledgeEventAsync(Guid eventId)
        {
            var eventLog = await _dbSet.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (eventLog == null)
                return false;

            eventLog.DeliveryStatus = SignalRDeliveryStatus.Acknowledged;
            eventLog.AcknowledgedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CleanupOldLogsAsync(int retentionDays)
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            var oldLogs = await _dbSet
                .Where(e => e.SentAt < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _dbSet.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
            }

            return oldLogs.Count;
        }
    }
}

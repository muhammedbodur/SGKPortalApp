using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Context;

namespace SGKPortalApp.ApiLayer.Controllers
{
    /// <summary>
    /// SignalR Event Log API Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // SignalR Log sayfası için authentication gerekli değil
    public class SignalREventLogController : ControllerBase
    {
        private readonly SGKDbContext _context;
        private readonly ILogger<SignalREventLogController> _logger;

        public SignalREventLogController(SGKDbContext context, ILogger<SignalREventLogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Event loglarını filtreli ve sayfalı olarak getirir
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult<PagedResultDto<SignalREventLogResponseDto>>> GetFiltered([FromBody] SignalREventLogFilterDto filter)
        {
            try
            {
                var query = _context.SignalREventLogs.AsNoTracking().AsQueryable();

                // Filters
                if (filter.StartDate.HasValue)
                    query = query.Where(e => e.SentAt >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(e => e.SentAt <= filter.EndDate.Value);

                if (filter.EventType.HasValue)
                    query = query.Where(e => e.EventType == filter.EventType.Value);

                if (filter.TargetType.HasValue)
                    query = query.Where(e => e.TargetType == filter.TargetType.Value);

                if (filter.DeliveryStatus.HasValue)
                    query = query.Where(e => e.DeliveryStatus == filter.DeliveryStatus.Value);

                if (filter.SiraId.HasValue)
                    query = query.Where(e => e.SiraId == filter.SiraId.Value);

                if (filter.SiraNo.HasValue)
                    query = query.Where(e => e.SiraNo == filter.SiraNo.Value);

                if (filter.BankoId.HasValue)
                    query = query.Where(e => e.BankoId == filter.BankoId.Value);

                if (filter.TvId.HasValue)
                    query = query.Where(e => e.TvId == filter.TvId.Value);

                if (!string.IsNullOrEmpty(filter.PersonelTc))
                    query = query.Where(e => e.PersonelTc == filter.PersonelTc);

                if (filter.HizmetBinasiId.HasValue)
                    query = query.Where(e => e.HizmetBinasiId == filter.HizmetBinasiId.Value);

                if (filter.CorrelationId.HasValue)
                    query = query.Where(e => e.CorrelationId == filter.CorrelationId.Value);

                // Total count
                var totalCount = await query.CountAsync();

                // Pagination
                var items = await query
                    .OrderByDescending(e => e.SentAt)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(e => new SignalREventLogResponseDto
                    {
                        EventLogId = e.EventLogId,
                        EventId = e.EventId,
                        EventType = e.EventType,
                        EventTypeName = GetEventTypeName(e.EventType),
                        EventName = e.EventName,
                        TargetType = e.TargetType,
                        TargetTypeName = GetTargetTypeName(e.TargetType),
                        TargetId = e.TargetId,
                        TargetCount = e.TargetCount,
                        SiraId = e.SiraId,
                        SiraNo = e.SiraNo,
                        BankoId = e.BankoId,
                        TvId = e.TvId,
                        PersonelTc = e.PersonelTc,
                        DeliveryStatus = e.DeliveryStatus,
                        DeliveryStatusName = GetDeliveryStatusName(e.DeliveryStatus),
                        ErrorMessage = e.ErrorMessage,
                        SentAt = e.SentAt,
                        AcknowledgedAt = e.AcknowledgedAt,
                        DurationMs = e.DurationMs,
                        PayloadSummary = e.PayloadSummary,
                        HizmetBinasiId = e.HizmetBinasiId,
                        CorrelationId = e.CorrelationId
                    })
                    .ToListAsync();

                return Ok(new PagedResultDto<SignalREventLogResponseDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR event log filtreleme hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// Son N dakikadaki eventleri getirir (canlı izleme için)
        /// </summary>
        [HttpGet("recent/{minutes:int}")]
        public async Task<ActionResult<List<SignalREventLogResponseDto>>> GetRecent(int minutes = 5)
        {
            try
            {
                var since = DateTime.Now.AddMinutes(-minutes);

                var items = await _context.SignalREventLogs
                    .AsNoTracking()
                    .Where(e => e.SentAt >= since)
                    .OrderByDescending(e => e.SentAt)
                    .Take(200)
                    .Select(e => new SignalREventLogResponseDto
                    {
                        EventLogId = e.EventLogId,
                        EventId = e.EventId,
                        EventType = e.EventType,
                        EventTypeName = GetEventTypeName(e.EventType),
                        EventName = e.EventName,
                        TargetType = e.TargetType,
                        TargetTypeName = GetTargetTypeName(e.TargetType),
                        TargetId = e.TargetId,
                        TargetCount = e.TargetCount,
                        SiraId = e.SiraId,
                        SiraNo = e.SiraNo,
                        BankoId = e.BankoId,
                        TvId = e.TvId,
                        PersonelTc = e.PersonelTc,
                        DeliveryStatus = e.DeliveryStatus,
                        DeliveryStatusName = GetDeliveryStatusName(e.DeliveryStatus),
                        ErrorMessage = e.ErrorMessage,
                        SentAt = e.SentAt,
                        DurationMs = e.DurationMs,
                        PayloadSummary = e.PayloadSummary,
                        CorrelationId = e.CorrelationId
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR recent events hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirli bir sıranın tüm eventlerini getirir
        /// </summary>
        [HttpGet("by-sira/{siraId:int}")]
        public async Task<ActionResult<List<SignalREventLogResponseDto>>> GetBySira(int siraId)
        {
            try
            {
                var items = await _context.SignalREventLogs
                    .AsNoTracking()
                    .Where(e => e.SiraId == siraId)
                    .OrderBy(e => e.SentAt)
                    .Select(e => new SignalREventLogResponseDto
                    {
                        EventLogId = e.EventLogId,
                        EventId = e.EventId,
                        EventType = e.EventType,
                        EventTypeName = GetEventTypeName(e.EventType),
                        EventName = e.EventName,
                        TargetType = e.TargetType,
                        TargetTypeName = GetTargetTypeName(e.TargetType),
                        TargetId = e.TargetId,
                        TargetCount = e.TargetCount,
                        SiraId = e.SiraId,
                        SiraNo = e.SiraNo,
                        BankoId = e.BankoId,
                        TvId = e.TvId,
                        PersonelTc = e.PersonelTc,
                        DeliveryStatus = e.DeliveryStatus,
                        DeliveryStatusName = GetDeliveryStatusName(e.DeliveryStatus),
                        ErrorMessage = e.ErrorMessage,
                        SentAt = e.SentAt,
                        DurationMs = e.DurationMs,
                        PayloadSummary = e.PayloadSummary,
                        CorrelationId = e.CorrelationId
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR by-sira hatası: {SiraId}", siraId);
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        /// <summary>
        /// İstatistikleri getirir
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<SignalREventLogStatsDto>> GetStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today;
                var end = endDate ?? DateTime.Now;

                var events = await _context.SignalREventLogs
                    .AsNoTracking()
                    .Where(e => e.SentAt >= start && e.SentAt <= end)
                    .ToListAsync();

                var stats = new SignalREventLogStatsDto
                {
                    TotalEvents = events.Count,
                    SentCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Sent),
                    FailedCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Failed),
                    NoTargetCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.NoTarget),
                    AcknowledgedCount = events.Count(e => e.DeliveryStatus == SignalRDeliveryStatus.Acknowledged),
                    AverageDurationMs = events.Where(e => e.DurationMs.HasValue).Select(e => e.DurationMs!.Value).DefaultIfEmpty(0).Average(),
                    EventTypeCounts = events.GroupBy(e => GetEventTypeName(e.EventType)).ToDictionary(g => g.Key, g => g.Count()),
                    TargetTypeCounts = events.GroupBy(e => GetTargetTypeName(e.TargetType)).ToDictionary(g => g.Key, g => g.Count())
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR stats hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }

        // Helper methods
        private static string GetEventTypeName(SignalREventType eventType) => eventType switch
        {
            SignalREventType.NewSira => "Yeni Sıra",
            SignalREventType.SiraCalled => "Sıra Çağrıldı",
            SignalREventType.SiraCompleted => "Sıra Tamamlandı",
            SignalREventType.SiraCancelled => "Sıra İptal",
            SignalREventType.SiraRedirected => "Sıra Yönlendirildi",
            SignalREventType.PanelUpdate => "Panel Güncelleme",
            SignalREventType.TvUpdate => "TV Güncelleme",
            SignalREventType.BankoModeActivated => "Banko Modu Aktif",
            SignalREventType.BankoModeDeactivated => "Banko Modu Pasif",
            SignalREventType.ForceLogout => "Zorla Çıkış",
            SignalREventType.Announcement => "Duyuru",
            _ => "Diğer"
        };

        private static string GetTargetTypeName(SignalRTargetType targetType) => targetType switch
        {
            SignalRTargetType.Connection => "Connection",
            SignalRTargetType.Connections => "Connections",
            SignalRTargetType.Group => "Group",
            SignalRTargetType.All => "All",
            SignalRTargetType.Personel => "Personel",
            SignalRTargetType.Tv => "TV",
            _ => "Bilinmiyor"
        };

        private static string GetDeliveryStatusName(SignalRDeliveryStatus status) => status switch
        {
            SignalRDeliveryStatus.Pending => "Beklemede",
            SignalRDeliveryStatus.Sent => "Gönderildi",
            SignalRDeliveryStatus.Failed => "Başarısız",
            SignalRDeliveryStatus.NoTarget => "Hedef Yok",
            SignalRDeliveryStatus.Acknowledged => "Alındı",
            _ => "Bilinmiyor"
        };
    }
}

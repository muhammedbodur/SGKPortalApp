using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.Common.Helpers;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SignalR;

namespace SGKPortalApp.BusinessLogicLayer.Services.SignalR
{
    /// <summary>
    /// SignalR Event Log Service Implementation
    /// </summary>
    public class SignalREventLogService : ISignalREventLogService
    {
        private readonly ISignalREventLogRepository _eventLogRepository;

        public SignalREventLogService(ISignalREventLogRepository eventLogRepository)
        {
            _eventLogRepository = eventLogRepository;
        }

        public async Task<PagedResultDto<SignalREventLogResponseDto>> GetFilteredAsync(SignalREventLogFilterDto filter)
        {
            var allEvents = (await _eventLogRepository.GetAllAsync()).ToList();
            
            var filteredEvents = allEvents
                .Where(e => (!filter.StartDate.HasValue || e.SentAt >= filter.StartDate.Value) &&
                           (!filter.EndDate.HasValue || e.SentAt <= filter.EndDate.Value) &&
                           (!filter.EventType.HasValue || e.EventType == filter.EventType.Value) &&
                           (!filter.TargetType.HasValue || e.TargetType == filter.TargetType.Value) &&
                           (!filter.DeliveryStatus.HasValue || e.DeliveryStatus == filter.DeliveryStatus.Value) &&
                           (!filter.SiraId.HasValue || e.SiraId == filter.SiraId.Value) &&
                           (!filter.SiraNo.HasValue || e.SiraNo == filter.SiraNo.Value) &&
                           (!filter.BankoId.HasValue || e.BankoId == filter.BankoId.Value) &&
                           (!filter.TvId.HasValue || e.TvId == filter.TvId.Value) &&
                           (string.IsNullOrEmpty(filter.PersonelTc) || e.PersonelTc == filter.PersonelTc) &&
                           (!filter.HizmetBinasiId.HasValue || e.HizmetBinasiId == filter.HizmetBinasiId.Value) &&
                           (!filter.CorrelationId.HasValue || e.CorrelationId == filter.CorrelationId.Value))
                .OrderByDescending(e => e.SentAt)
                .ToList();

            var totalCount = filteredEvents.Count;

            var items = filteredEvents
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
                .ToList();

            return new PagedResultDto<SignalREventLogResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<SignalREventLogResponseDto>> GetRecentAsync(int minutes = 5)
        {
            var since = DateTimeHelper.Now.AddMinutes(-minutes);
            var allEvents = (await _eventLogRepository.GetAllAsync()).ToList();

            var items = allEvents
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
                .ToList();

            return items;
        }

        public async Task<List<SignalREventLogResponseDto>> GetBySiraAsync(int siraId)
        {
            var events = await _eventLogRepository.GetBySiraIdAsync(siraId);

            var items = events
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
                .ToList();

            return items;
        }

        public async Task<SignalREventLogStatsDto> GetStatsAsync(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTimeHelper.Now;

            var stats = await _eventLogRepository.GetStatisticsAsync(start, end);

            return new SignalREventLogStatsDto
            {
                TotalEvents = stats.TotalEvents,
                SentCount = stats.SentCount,
                FailedCount = stats.FailedCount,
                NoTargetCount = stats.NoTargetCount,
                AcknowledgedCount = stats.AcknowledgedCount,
                AverageDurationMs = stats.AverageDurationMs,
                EventTypeCounts = stats.EventTypeCounts.ToDictionary(kv => GetEventTypeName(kv.Key), kv => kv.Value),
                TargetTypeCounts = stats.TargetTypeCounts.ToDictionary(kv => GetTargetTypeName(kv.Key), kv => kv.Value)
            };
        }

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

using System.Text.Json;
using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.Entities.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SignalR;

namespace SGKPortalApp.BusinessLogicLayer.Services.SignalR
{
    /// <summary>
    /// SignalR Audit Service Implementation
    /// Event loglarının oluşturulması ve yönetimi
    /// </summary>
    public class SignalRAuditService : ISignalRAuditService
    {
        private readonly ISignalREventLogRepository _eventLogRepository;
        private readonly ILogger<SignalRAuditService> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            MaxDepth = 3 // Derin nesneleri sınırla
        };

        public SignalRAuditService(
            ISignalREventLogRepository eventLogRepository,
            ILogger<SignalRAuditService> logger)
        {
            _eventLogRepository = eventLogRepository;
            _logger = logger;
        }

        public Guid CreateCorrelationId()
        {
            return Guid.NewGuid();
        }

        public async Task<SignalREventLog> LogEventAsync(SignalREventLogRequest request)
        {
            try
            {
                var eventLog = CreateEventLog(request);
                await _eventLogRepository.LogEventAsync(eventLog);
                return eventLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SignalR event log kaydedilemedi: {EventType}", request.EventType);
                // Log hatası ana akışı etkilememeli
                return new SignalREventLog { EventId = Guid.NewGuid() };
            }
        }

        public async Task LogEventsAsync(IEnumerable<SignalREventLogRequest> requests)
        {
            try
            {
                var eventLogs = requests.Select(CreateEventLog).ToList();
                await _eventLogRepository.LogEventsAsync(eventLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SignalR toplu event log kaydedilemedi");
            }
        }

        public async Task UpdateEventStatusAsync(Guid eventId, SignalRDeliveryStatus status, string? errorMessage = null, int? durationMs = null)
        {
            try
            {
                // Doğrudan SQL ile güncelle (daha hızlı ve bağımsız context)
                await _eventLogRepository.UpdateStatusDirectAsync(eventId, status, errorMessage, durationMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SignalR event status güncellenemedi: {EventId}", eventId);
            }
        }

        private SignalREventLog CreateEventLog(SignalREventLogRequest request)
        {
            string? payloadSummary = CreatePayloadSummary(request.Payload);

            return new SignalREventLog
            {
                EventId = Guid.NewGuid(),
                EventType = request.EventType,
                EventName = request.EventName,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                TargetCount = request.TargetCount,
                SiraId = request.SiraId,
                SiraNo = request.SiraNo,
                BankoId = request.BankoId,
                TvId = request.TvId,
                PersonelTc = request.PersonelTc,
                PayloadSummary = payloadSummary,
                HizmetBinasiId = request.HizmetBinasiId,
                CorrelationId = request.CorrelationId,
                DeliveryStatus = SignalRDeliveryStatus.Pending,
                SentAt = DateTime.Now
            };
        }

        /// <summary>
        /// Payload'dan okunabilir bir özet oluşturur
        /// Sadece önemli alanları içerir
        /// </summary>
        private string? CreatePayloadSummary(object? payload)
        {
            if (payload == null) return null;

            try
            {
                var type = payload.GetType();
                var summary = new Dictionary<string, object?>();

                // Önemli alanları çıkar
                var importantProps = new[] 
                { 
                    "UpdateType", "SiraId", "SiraNo", "BankoId", "TvId", 
                    "PersonelTc", "Pozisyon", "ToplamSiraSayisi", "ShowOverlay",
                    "KanalAltAdi", "BeklemeDurum", "HizmetBinasiAdi"
                };

                foreach (var propName in importantProps)
                {
                    var prop = type.GetProperty(propName);
                    var value = prop?.GetValue(payload);
                    if (value != null)
                    {
                        summary[propName] = value;
                    }
                }

                // Nested Sira'dan da önemli alanları çıkar
                var siraProp = type.GetProperty("Sira");
                var sira = siraProp?.GetValue(payload);
                if (sira != null)
                {
                    var siraType = sira.GetType();
                    var siraImportantProps = new[] { "SiraId", "SiraNo", "BankoId", "KanalAltAdi", "BeklemeDurum" };
                    
                    foreach (var propName in siraImportantProps)
                    {
                        if (!summary.ContainsKey(propName))
                        {
                            var prop = siraType.GetProperty(propName);
                            var value = prop?.GetValue(sira);
                            if (value != null)
                            {
                                summary[$"Sira.{propName}"] = value;
                            }
                        }
                    }
                }

                // CalledSira'dan da önemli alanları çıkar
                var calledSiraProp = type.GetProperty("CalledSira");
                var calledSira = calledSiraProp?.GetValue(payload);
                if (calledSira != null)
                {
                    var calledSiraType = calledSira.GetType();
                    var calledSiraProps = new[] { "SiraId", "SiraNo", "BankoId", "BankoAdi" };
                    
                    foreach (var propName in calledSiraProps)
                    {
                        if (!summary.ContainsKey(propName))
                        {
                            var prop = calledSiraType.GetProperty(propName);
                            var value = prop?.GetValue(calledSira);
                            if (value != null)
                            {
                                summary[$"CalledSira.{propName}"] = value;
                            }
                        }
                    }
                }

                if (summary.Count == 0)
                {
                    // Hiçbir önemli alan bulunamadı, tip adını döndür
                    return type.Name;
                }

                var json = JsonSerializer.Serialize(summary, _jsonOptions);
                return json.Length > 2000 ? json[..2000] : json;
            }
            catch
            {
                return payload.GetType().Name;
            }
        }
    }
}

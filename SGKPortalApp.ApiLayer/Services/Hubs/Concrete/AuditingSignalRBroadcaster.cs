using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SignalR;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;

namespace SGKPortalApp.ApiLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// Auditing SignalR Broadcaster - Decorator Pattern
    /// Mevcut ISignalRBroadcaster'ı wrap ederek audit log ekler
    /// Ana akışı yavaşlatmamak için fire-and-forget logging kullanır
    /// </summary>
    public class AuditingSignalRBroadcaster : ISignalRBroadcaster
    {
        private readonly IHubContext<SiramatikHub> _hubContext;
        private readonly ISignalRAuditService _auditService;
        private readonly ILogger<AuditingSignalRBroadcaster> _logger;

        // Event name -> EventType mapping
        private static readonly Dictionary<string, SignalREventType> EventTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "siraListUpdate", SignalREventType.PanelUpdate },
            { "TvSiraGuncellendi", SignalREventType.TvUpdate },
            { "receiveSiraUpdate", SignalREventType.TvUpdate },
            { "siraCalled", SignalREventType.SiraCalled },
            { "siraCompleted", SignalREventType.SiraCompleted },
            { "siraCancelled", SignalREventType.SiraCancelled },
            { "siraRedirected", SignalREventType.SiraRedirected },
            { "siraCreated", SignalREventType.NewSira },
            { "bankoModeActivated", SignalREventType.BankoModeActivated },
            { "bankoModeDeactivated", SignalREventType.BankoModeDeactivated },
            { "forceLogout", SignalREventType.ForceLogout },
            { "receiveDuyuruUpdate", SignalREventType.Announcement },
            { "BankoPanelSiraGuncellemesi", SignalREventType.PanelUpdate }
        };

        public AuditingSignalRBroadcaster(
            IHubContext<SiramatikHub> hubContext,
            ISignalRAuditService auditService,
            ILogger<AuditingSignalRBroadcaster> logger)
        {
            _hubContext = hubContext;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task SendToConnectionsAsync(IEnumerable<string> connectionIds, string eventName, object payload)
        {
            var connectionList = connectionIds.ToList();
            if (!connectionList.Any())
            {
                // Hedef yok - log kaydı oluştur
                _ = LogEventFireAndForget(new SignalREventLogRequest
                {
                    EventType = GetEventType(eventName),
                    EventName = eventName,
                    TargetType = SignalRTargetType.Connections,
                    TargetCount = 0,
                    Payload = payload
                }, SignalRDeliveryStatus.NoTarget, null, 0);

                _logger.LogDebug("SendToConnectionsAsync: Hedef connection yok, atlanıyor");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            SignalRDeliveryStatus status = SignalRDeliveryStatus.Sent;
            string? errorMessage = null;

            try
            {
                await _hubContext.Clients.Clients(connectionList).SendAsync(eventName, payload);
                _logger.LogDebug("📤 {EventName} gönderildi: {Count} connection", eventName, connectionList.Count);
            }
            catch (Exception ex)
            {
                status = SignalRDeliveryStatus.Failed;
                errorMessage = ex.Message;
                _logger.LogError(ex, "❌ SendToConnectionsAsync hatası: {EventName}", eventName);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Fire-and-forget logging (ana akışı bloklamaz)
                _ = LogEventFireAndForget(new SignalREventLogRequest
                {
                    EventType = GetEventType(eventName),
                    EventName = eventName,
                    TargetType = SignalRTargetType.Connections,
                    TargetId = connectionList.Count <= 5 ? string.Join(",", connectionList) : $"{connectionList.Count} connections",
                    TargetCount = connectionList.Count,
                    Payload = payload
                }, status, errorMessage, (int)stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task SendToGroupAsync(string groupName, string eventName, object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            SignalRDeliveryStatus status = SignalRDeliveryStatus.Sent;
            string? errorMessage = null;

            try
            {
                await _hubContext.Clients.Group(groupName).SendAsync(eventName, payload);
                _logger.LogDebug("📤 {EventName} gruba gönderildi: {GroupName}", eventName, groupName);
            }
            catch (Exception ex)
            {
                status = SignalRDeliveryStatus.Failed;
                errorMessage = ex.Message;
                _logger.LogError(ex, "❌ SendToGroupAsync hatası: {GroupName} -> {EventName}", groupName, eventName);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Grup tipini belirle
                var targetType = SignalRTargetType.Group;
                int? tvId = null;
                int? bankoId = null;

                if (groupName.StartsWith("TV_", StringComparison.OrdinalIgnoreCase))
                {
                    targetType = SignalRTargetType.Tv;
                    if (int.TryParse(groupName[3..], out var id))
                        tvId = id;
                }
                else if (groupName.StartsWith("BANKO_", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(groupName[6..], out var id))
                        bankoId = id;
                }

                _ = LogEventFireAndForget(new SignalREventLogRequest
                {
                    EventType = GetEventType(eventName),
                    EventName = eventName,
                    TargetType = targetType,
                    TargetId = groupName,
                    TargetCount = 1, // Grup sayısı bilinmiyor
                    TvId = tvId,
                    BankoId = bankoId,
                    Payload = payload
                }, status, errorMessage, (int)stopwatch.ElapsedMilliseconds);
            }
        }

        public async Task BroadcastAllAsync(string eventName, object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            SignalRDeliveryStatus status = SignalRDeliveryStatus.Sent;
            string? errorMessage = null;

            try
            {
                await _hubContext.Clients.All.SendAsync(eventName, payload);
                _logger.LogDebug("📢 {EventName} broadcast edildi", eventName);
            }
            catch (Exception ex)
            {
                status = SignalRDeliveryStatus.Failed;
                errorMessage = ex.Message;
                _logger.LogError(ex, "❌ BroadcastAllAsync hatası: {EventName}", eventName);
                throw;
            }
            finally
            {
                stopwatch.Stop();

                _ = LogEventFireAndForget(new SignalREventLogRequest
                {
                    EventType = GetEventType(eventName),
                    EventName = eventName,
                    TargetType = SignalRTargetType.All,
                    TargetId = "ALL",
                    TargetCount = -1, // Bilinmiyor
                    Payload = payload
                }, status, errorMessage, (int)stopwatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Fire-and-forget event logging
        /// Ana akışı bloklamaz, hata olursa sadece log yazar
        /// </summary>
        private async Task LogEventFireAndForget(SignalREventLogRequest request, SignalRDeliveryStatus status, string? errorMessage, int durationMs)
        {
            try
            {
                // Payload'dan SiraId, SiraNo, BankoId çıkarmaya çalış
                ExtractPayloadInfo(request);

                var eventLog = await _auditService.LogEventAsync(request);
                
                // Status'u güncelle
                await _auditService.UpdateEventStatusAsync(eventLog.EventId, status, errorMessage, durationMs);
            }
            catch (Exception ex)
            {
                // Audit hatası ana akışı etkilememeli
                _logger.LogWarning(ex, "⚠️ SignalR audit log hatası (görmezden geliniyor)");
            }
        }

        /// <summary>
        /// Payload'dan SiraId, SiraNo, BankoId gibi bilgileri çıkarır
        /// </summary>
        private void ExtractPayloadInfo(SignalREventLogRequest request)
        {
            if (request.Payload == null) return;

            try
            {
                var type = request.Payload.GetType();

                // SiraId
                ExtractIntProperty(type, request.Payload, new[] { "SiraId", "siraId" }, val => request.SiraId = val);

                // SiraNo
                ExtractIntProperty(type, request.Payload, new[] { "SiraNo", "siraNo" }, val => request.SiraNo = val);

                // BankoId (nullable int desteği)
                ExtractNullableIntProperty(type, request.Payload, new[] { "BankoId", "bankoId" }, val => request.BankoId = val);

                // TvId (nullable int desteği)
                ExtractNullableIntProperty(type, request.Payload, new[] { "TvId", "tvId" }, val => request.TvId = val);

                // PersonelTc
                ExtractStringProperty(type, request.Payload, new[] { "PersonelTc", "personelTc" }, val => request.PersonelTc = val);

                // Nested Sira object
                var siraProp = type.GetProperty("Sira") ?? type.GetProperty("sira");
                var sira = siraProp?.GetValue(request.Payload);
                if (sira != null)
                {
                    var siraType = sira.GetType();
                    
                    if (request.SiraId == null)
                        ExtractIntProperty(siraType, sira, new[] { "SiraId", "siraId" }, val => request.SiraId = val);

                    if (request.SiraNo == null)
                        ExtractIntProperty(siraType, sira, new[] { "SiraNo", "siraNo" }, val => request.SiraNo = val);

                    if (request.BankoId == null)
                        ExtractNullableIntProperty(siraType, sira, new[] { "BankoId", "bankoId" }, val => request.BankoId = val);

                    if (request.PersonelTc == null)
                        ExtractStringProperty(siraType, sira, new[] { "PersonelTc", "personelTc" }, val => request.PersonelTc = val);
                }

                // CalledSira nested object (TvSiraCalledDto için)
                var calledSiraProp = type.GetProperty("CalledSira") ?? type.GetProperty("calledSira");
                var calledSira = calledSiraProp?.GetValue(request.Payload);
                if (calledSira != null)
                {
                    var calledSiraType = calledSira.GetType();
                    
                    if (request.SiraId == null)
                        ExtractIntProperty(calledSiraType, calledSira, new[] { "SiraId", "siraId" }, val => request.SiraId = val);

                    if (request.SiraNo == null)
                        ExtractIntProperty(calledSiraType, calledSira, new[] { "SiraNo", "siraNo" }, val => request.SiraNo = val);

                    if (request.BankoId == null)
                        ExtractNullableIntProperty(calledSiraType, calledSira, new[] { "BankoId", "bankoId" }, val => request.BankoId = val);
                }
            }
            catch
            {
                // Reflection hatası görmezden gel
            }
        }

        private static void ExtractIntProperty(Type type, object obj, string[] propNames, Action<int> setter)
        {
            foreach (var propName in propNames)
            {
                var prop = type.GetProperty(propName);
                if (prop?.GetValue(obj) is int val)
                {
                    setter(val);
                    return;
                }
            }
        }

        private static void ExtractNullableIntProperty(Type type, object obj, string[] propNames, Action<int> setter)
        {
            foreach (var propName in propNames)
            {
                var prop = type.GetProperty(propName);
                var value = prop?.GetValue(obj);
                if (value == null) continue;
                
                if (value is int intVal)
                {
                    setter(intVal);
                    return;
                }
                
                // Nullable<int> için
                var valueType = value.GetType();
                if (valueType == typeof(int?) || Nullable.GetUnderlyingType(valueType) == typeof(int))
                {
                    var nullableVal = (int?)value;
                    if (nullableVal.HasValue)
                    {
                        setter(nullableVal.Value);
                        return;
                    }
                }
            }
        }

        private static void ExtractStringProperty(Type type, object obj, string[] propNames, Action<string> setter)
        {
            foreach (var propName in propNames)
            {
                var prop = type.GetProperty(propName);
                if (prop?.GetValue(obj) is string val && !string.IsNullOrEmpty(val))
                {
                    setter(val);
                    return;
                }
            }
        }

        private static SignalREventType GetEventType(string eventName)
        {
            return EventTypeMap.TryGetValue(eventName, out var eventType) 
                ? eventType 
                : SignalREventType.Other;
        }
    }
}

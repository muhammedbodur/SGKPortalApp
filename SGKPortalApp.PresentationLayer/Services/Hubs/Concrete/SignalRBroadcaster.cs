using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Concrete
{
    /// <summary>
    /// SignalR mesaj yayınlama implementasyonu
    /// Business katmanından gelen istekleri SiramatikHub üzerinden iletir
    /// </summary>
    public class SignalRBroadcaster : ISignalRBroadcaster
    {
        private readonly IHubContext<SiramatikHub> _hubContext;
        private readonly ILogger<SignalRBroadcaster> _logger;

        public SignalRBroadcaster(
            IHubContext<SiramatikHub> hubContext,
            ILogger<SignalRBroadcaster> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendToConnectionsAsync(IEnumerable<string> connectionIds, string eventName, object payload)
        {
            var connectionList = connectionIds.ToList();
            if (!connectionList.Any())
            {
                _logger.LogDebug("SendToConnectionsAsync: Hedef connection yok, atlanıyor");
                return;
            }

            try
            {
                await _hubContext.Clients.Clients(connectionList).SendAsync(eventName, payload);
                _logger.LogDebug("📤 {EventName} gönderildi: {Count} connection", eventName, connectionList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SendToConnectionsAsync hatası: {EventName}", eventName);
                throw;
            }
        }

        public async Task SendToGroupAsync(string groupName, string eventName, object payload)
        {
            try
            {
                await _hubContext.Clients.Group(groupName).SendAsync(eventName, payload);
                _logger.LogDebug("📤 {EventName} gruba gönderildi: {GroupName}", eventName, groupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SendToGroupAsync hatası: {GroupName} -> {EventName}", groupName, eventName);
                throw;
            }
        }

        public async Task BroadcastAllAsync(string eventName, object payload)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync(eventName, payload);
                _logger.LogDebug("📢 {EventName} broadcast edildi", eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ BroadcastAllAsync hatası: {EventName}", eventName);
                throw;
            }
        }
    }
}

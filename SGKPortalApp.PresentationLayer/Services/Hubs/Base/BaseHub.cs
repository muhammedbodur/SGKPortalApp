using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Base
{
    /// <summary>
    /// Tüm Hub'lar için temel sınıf
    /// Ortak fonksiyonalite ve yardımcı metodlar içerir
    /// </summary>
    public abstract class BaseHub : Hub
    {
        protected readonly ILogger _logger;

        protected BaseHub(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Bağlantı kurulduğunda çağrılır
        /// NOT: HubConnection oluşturma işlemi türetilmiş sınıflarda yapılmalı
        /// (örn: SiramatikHub, NotificationHub)
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name ?? "Anonymous";
            var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
            
            _logger.LogInformation($"🟢 [{GetType().Name}] Yeni bağlantı: {connectionId} - Kullanıcı: {userId} (TC: {tcKimlikNo ?? "Yok"})");
            
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Bağlantı koptuğunda çağrılır
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name ?? "Anonymous";
            
            if (exception != null)
            {
                _logger.LogError(exception, $"🔴 [{GetType().Name}] Bağlantı koptu (hata): {connectionId} - Kullanıcı: {userId}");
            }
            else
            {
                _logger.LogInformation($"🔴 [{GetType().Name}] Bağlantı koptu: {connectionId} - Kullanıcı: {userId}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Belirli bir gruba mesaj gönder
        /// </summary>
        protected async Task SendToGroupAsync(string groupName, string method, object data)
        {
            try
            {
                await Clients.Group(groupName).SendAsync(method, data);
                _logger.LogDebug($"📤 Grup mesajı gönderildi: {groupName} -> {method}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Grup mesajı gönderilemedi: {groupName} -> {method}");
                throw;
            }
        }

        /// <summary>
        /// Belirli bir kullanıcıya mesaj gönder
        /// </summary>
        protected async Task SendToUserAsync(string userId, string method, object data)
        {
            try
            {
                await Clients.User(userId).SendAsync(method, data);
                _logger.LogDebug($"📤 Kullanıcı mesajı gönderildi: {userId} -> {method}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Kullanıcı mesajı gönderilemedi: {userId} -> {method}");
                throw;
            }
        }

        /// <summary>
        /// Tüm bağlı istemcilere mesaj gönder
        /// </summary>
        protected async Task BroadcastAsync(string method, object data)
        {
            try
            {
                await Clients.All.SendAsync(method, data);
                _logger.LogDebug($"📢 Broadcast mesajı gönderildi: {method}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Broadcast mesajı gönderilemedi: {method}");
                throw;
            }
        }

        /// <summary>
        /// Çağıran istemciye mesaj gönder
        /// </summary>
        protected async Task SendToCallerAsync(string method, object data)
        {
            try
            {
                await Clients.Caller.SendAsync(method, data);
                _logger.LogDebug($"📤 Caller mesajı gönderildi: {method}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Caller mesajı gönderilemedi: {method}");
                throw;
            }
        }

        /// <summary>
        /// Çağıran istemci hariç tüm istemcilere mesaj gönder
        /// </summary>
        protected async Task SendToOthersAsync(string method, object data)
        {
            try
            {
                await Clients.Others.SendAsync(method, data);
                _logger.LogDebug($"📤 Others mesajı gönderildi: {method}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Others mesajı gönderilemedi: {method}");
                throw;
            }
        }

        /// <summary>
        /// Gruba katıl
        /// </summary>
        protected async Task JoinGroupAsync(string groupName)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation($"➕ Gruba katıldı: {Context.ConnectionId} -> {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Gruba katılma hatası: {groupName}");
                throw;
            }
        }

        /// <summary>
        /// Gruptan ayrıl
        /// </summary>
        protected async Task LeaveGroupAsync(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation($"➖ Gruptan ayrıldı: {Context.ConnectionId} -> {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Gruptan ayrılma hatası: {groupName}");
                throw;
            }
        }

        /// <summary>
        /// Bağlantı bilgilerini al
        /// </summary>
        protected ConnectionInfoDto GetConnectionInfo()
        {
            return new ConnectionInfoDto
            {
                ConnectionId = Context.ConnectionId,
                UserId = Context.User?.Identity?.Name,
                UserAgent = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString(),
                IpAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString(),
                ConnectedAt = DateTime.Now
            };
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.Hubs.Base;
using SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.Hubs
{
    /// <summary>
    /// Sıramatik sistemi için SignalR Hub
    /// TV ekranları, banko yönetimi ve sıra çağırma işlemlerini yönetir
    /// </summary>
    public class SiramatikHub : BaseHub
    {
        private readonly IHubConnectionService _connectionService;

        public SiramatikHub(
            ILogger<SiramatikHub> logger,
            IHubConnectionService connectionService) : base(logger)
        {
            _connectionService = connectionService;
        }

        #region Connection Management

        /// <summary>
        /// Bağlantı kurulduğunda çağrılır
        /// Yeni yapı: Eski bağlantıları kontrol eder, yeni bağlantı oluşturur
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            var info = GetConnectionInfo();
            var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
            var userType = Context.User?.FindFirst("UserType")?.Value;
            
            if (!string.IsNullOrEmpty(tcKimlikNo))
            {
                try
                {
                    // 1. Bu kullanıcının aktif bağlantılarını kontrol et
                    var existingConnections = await _connectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
                    
                    if (existingConnections.Any())
                    {
                        // 2. Eski bağlantıları zorla kapat (Yeni login = Eski logout)
                        foreach (var oldConnection in existingConnections)
                        {
                            await Clients.Client(oldConnection.ConnectionId)
                                .SendAsync("ForceLogout", "Başka bir cihazdan giriş yapıldı. Oturumunuz sonlandırıldı.");
                            
                            await _connectionService.DisconnectAsync(oldConnection.ConnectionId);
                            
                            _logger.LogWarning($"⚠️ Eski bağlantı kapatıldı: {oldConnection.ConnectionId} (Yeni login: {info.ConnectionId})");
                        }
                    }
                    
                    // 3. Yeni bağlantı oluştur (ConnectionType = MainLayout)
                    var success = await _connectionService.RegisterUserConnectionAsync(
                        info.ConnectionId, 
                        tcKimlikNo, 
                        "MainLayout"
                    );
                    
                    if (success)
                    {
                        _logger.LogInformation($"✅ Yeni bağlantı oluşturuldu: {info.ConnectionId} | TC: {tcKimlikNo} | Type: MainLayout | IP: {info.IpAddress}");
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ Bağlantı oluşturulamadı: {info.ConnectionId} | TC: {tcKimlikNo}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ OnConnectedAsync hatası: {tcKimlikNo}");
                }
            }
            else
            {
                _logger.LogInformation($"🟢 Anonymous bağlantı: {info.ConnectionId} | IP: {info.IpAddress}");
            }
        }

        /// <summary>
        /// Bağlantı koptuğunda çağrılır
        /// Yeni yapı: ConnectionType'a göre temizlik yapar
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
            
            try
            {
                // 1. HubConnection'ı bul
                var hubConnection = await _connectionService.GetByConnectionIdAsync(connectionId);
                
                if (hubConnection != null)
                {
                    // 2. ConnectionType'a göre temizlik
                    switch (hubConnection.ConnectionType)
                    {
                        case "BankoMode":
                            // Banko modundan çıkış
                            var bankoConnection = await _connectionService.GetBankoConnectionByHubConnectionIdAsync(hubConnection.HubConnectionId);
                            if (bankoConnection != null)
                            {
                                await _connectionService.DeactivateBankoConnectionAsync(hubConnection.TcKimlikNo);
                                await Groups.RemoveFromGroupAsync(connectionId, $"BANKO_{bankoConnection.BankoId}");
                                _logger.LogWarning($"⚠️ Banko#{bankoConnection.BankoId} bağlantısı koptu: {hubConnection.TcKimlikNo}");
                            }
                            break;
                            
                        case "TvDisplay":
                            // TV Display'den çıkış
                            var tvConnection = await _connectionService.GetTvConnectionByHubConnectionIdAsync(hubConnection.HubConnectionId);
                            if (tvConnection != null)
                            {
                                await Groups.RemoveFromGroupAsync(connectionId, $"TV_{tvConnection.TvId}");
                                _logger.LogInformation($"ℹ️ TV#{tvConnection.TvId} bağlantısı koptu");
                            }
                            break;
                            
                        case "MainLayout":
                            // Normal personel bağlantısı koptu
                            _logger.LogInformation($"ℹ️ Personel bağlantısı koptu: {hubConnection.TcKimlikNo}");
                            break;
                    }
                    
                    // 3. Bağlantıyı kapat
                    await _connectionService.DisconnectAsync(connectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ OnDisconnectedAsync hatası: {connectionId}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region TV Group Management

        /// <summary>
        /// TV'yi kendi grubuna ekle (Yeni yapı - birden fazla kullanıcı destekli)
        /// </summary>
        /// <param name="tvId">TV ID</param>
        public async Task JoinTvGroup(int tvId)
        {
            try
            {
                var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
                var userType = Context.User?.FindFirst("UserType")?.Value;
                var connectionId = Context.ConnectionId;
                
                if (userType == "TvUser")
                {
                    // 1. TV User sadece kendi TV'sini görebilir
                    var tvIdFromUser = int.Parse(tcKimlikNo!.Substring(2)); // "TV0000001" -> 1
                    if (tvIdFromUser != tvId)
                    {
                        throw new HubException("Bu TV'yi görüntüleme yetkiniz yok!");
                    }
                    
                    // 2. Bu TV User'ın başka aktif bağlantısı var mı? (Sadece 1 tab)
                    var existingConnections = await _connectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo!);
                    if (existingConnections.Any(c => c.ConnectionId != connectionId))
                    {
                        // Eski tab'ı kapat
                        foreach (var old in existingConnections.Where(c => c.ConnectionId != connectionId))
                        {
                            await Clients.Client(old.ConnectionId)
                                .SendAsync("ForceLogout", "Başka bir sekmede TV açıldı. Bu sekme kapatılıyor.");
                            
                            await _connectionService.DisconnectAsync(old.ConnectionId);
                            
                            _logger.LogWarning($"⚠️ TV User {tcKimlikNo} yeni tab açtı. Eski tab kapatıldı.");
                        }
                    }
                    
                    // 3. Bu TV başka bir TV User tarafından kullanılıyor mu?
                    var tvInUse = await _connectionService.IsTvInUseByTvUserAsync(tvId);
                    if (tvInUse)
                    {
                        throw new HubException($"TV#{tvId} zaten başka bir TV kullanıcısı tarafından kullanılıyor!");
                    }
                }
                // Personel için kontrol yok, istediği TV'yi izleyebilir
                
                // 4. ConnectionType'ı güncelle
                await _connectionService.UpdateConnectionTypeAsync(connectionId, "TvDisplay");
                
                // 5. HubTvConnection oluştur
                var success = await _connectionService.RegisterTvConnectionAsync(tvId, connectionId);
                
                if (success)
                {
                    // 6. SignalR grubuna katıl
                    var groupName = $"TV_{tvId}";
                    await JoinGroupAsync(groupName);
                    
                    _logger.LogInformation($"✅ {tcKimlikNo} ({userType}) -> TV#{tvId} grubuna katıldı");
                    await SendToCallerAsync("ConnectionConfirmed", new { tvId, status = "connected" });
                }
                else
                {
                    _logger.LogWarning($"⚠️ TV bağlantısı kaydedilemedi: TV#{tvId}");
                    await SendToCallerAsync("ConnectionError", new { tvId, error = "Bağlantı kaydedilemedi" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ TV gruba katılma hatası: TV#{tvId}");
                await SendToCallerAsync("ConnectionError", new { tvId, error = ex.Message });
                throw;
            }
        }

        /// <summary>
        /// TV grubundan ayrıl
        /// </summary>
        /// <param name="tvId">TV ID</param>
        public async Task LeaveTvGroup(int tvId)
        {
            try
            {
                var groupName = $"TV_{tvId}";
                await LeaveGroupAsync(groupName);
                
                // Bağlantıyı kaldır
                await _connectionService.UnregisterTvConnectionAsync(tvId, Context.ConnectionId);
                
                _logger.LogInformation($"➖ TV gruptan ayrıldı: TV#{tvId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ TV gruptan ayrılma hatası: TV#{tvId}");
            }
        }

        #endregion

        #region Banko Mode Management

        /// <summary>
        /// Banko moduna geç (Fiziksel bankoya geçiş)
        /// </summary>
        /// <param name="bankoId">Banko ID</param>
        public async Task EnterBankoMode(int bankoId)
        {
            try
            {
                var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
                var connectionId = Context.ConnectionId;
                
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    throw new HubException("Kullanıcı bilgisi bulunamadı!");
                }
                
                // 1. Bu banko başka personel tarafından kullanılıyor mu?
                var bankoInUse = await _connectionService.IsBankoInUseAsync(bankoId);
                if (bankoInUse)
                {
                    var activePerson = await _connectionService.GetBankoActivePersonelAsync(bankoId);
                    throw new HubException($"Banko#{bankoId} şu anda {activePerson?.Personel.AdSoyad ?? "başka bir personel"} tarafından kullanılıyor!");
                }
                
                // 2. Bu personel başka bankoda mı?
                var existingBanko = await _connectionService.GetPersonelActiveBankoAsync(tcKimlikNo);
                if (existingBanko != null && existingBanko.BankoId != bankoId)
                {
                    throw new HubException($"Zaten Banko#{existingBanko.BankoId}'de aktifsiniz!");
                }
                
                // 3. Bu personelin başka tab'ı açık mı? (Banko modunda sadece 1 tab)
                var activeConnections = await _connectionService.GetActiveConnectionsByTcKimlikNoAsync(tcKimlikNo);
                if (activeConnections.Count() > 1)
                {
                    // Diğer tab'ları kapat
                    foreach (var conn in activeConnections.Where(c => c.ConnectionId != connectionId))
                    {
                        await Clients.Client(conn.ConnectionId)
                            .SendAsync("ForceLogout", "Banko moduna geçildi. Diğer sekmeler kapatılıyor.");
                        
                        await _connectionService.DisconnectAsync(conn.ConnectionId);
                        
                        _logger.LogWarning($"⚠️ Banko moduna geçiş: Diğer tab kapatıldı ({conn.ConnectionId})");
                    }
                }
                
                // 4. ConnectionType'ı güncelle
                await _connectionService.UpdateConnectionTypeAsync(connectionId, "BankoMode");
                
                // 5. HubBankoConnection oluştur (Fiziksel oturum)
                var success = await _connectionService.RegisterBankoConnectionAsync(bankoId, connectionId, tcKimlikNo);
                
                if (success)
                {
                    // 6. SignalR grubuna katıl
                    var groupName = $"BANKO_{bankoId}";
                    await JoinGroupAsync(groupName);
                    
                    _logger.LogInformation($"✅ {tcKimlikNo} -> Banko#{bankoId} moduna girdi (Fiziksel)");
                    await SendToCallerAsync("BankoModeActivated", new { bankoId });
                }
                else
                {
                    throw new HubException("Banko oturumu oluşturulamadı!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko moduna giriş hatası: Banko#{bankoId}");
                await SendToCallerAsync("BankoModeError", new { bankoId, error = ex.Message });
                throw;
            }
        }

        /// <summary>
        /// Banko modundan çık
        /// </summary>
        public async Task ExitBankoMode()
        {
            try
            {
                var tcKimlikNo = Context.User?.FindFirst("TcKimlikNo")?.Value;
                var connectionId = Context.ConnectionId;
                
                if (string.IsNullOrEmpty(tcKimlikNo))
                {
                    throw new HubException("Kullanıcı bilgisi bulunamadı!");
                }
                
                // 1. Personelin aktif banko oturumunu bul
                var bankoConnection = await _connectionService.GetPersonelActiveBankoAsync(tcKimlikNo);
                if (bankoConnection != null)
                {
                    // 2. Banko oturumunu kapat
                    await _connectionService.DeactivateBankoConnectionAsync(tcKimlikNo);
                    
                    // 3. SignalR grubundan çıkar
                    var groupName = $"BANKO_{bankoConnection.BankoId}";
                    await LeaveGroupAsync(groupName);
                    
                    _logger.LogInformation($"✅ {tcKimlikNo} -> Banko#{bankoConnection.BankoId} modundan çıktı");
                }
                
                // 4. ConnectionType'ı geri MainLayout yap
                await _connectionService.UpdateConnectionTypeAsync(connectionId, "MainLayout");
                
                await SendToCallerAsync("BankoModeDeactivated", new { });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Banko modundan çıkış hatası");
                await SendToCallerAsync("BankoModeError", new { error = ex.Message });
                throw;
            }
        }

        /// <summary>
        /// Banko'nun bağlantı durumunu kontrol et
        /// </summary>
        /// <param name="bankoId">Banko ID</param>
        public async Task<bool> CheckBankoConnection(int bankoId)
        {
            try
            {
                return await _connectionService.IsBankoConnectedAsync(bankoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko bağlantı kontrolü hatası: Banko#{bankoId}");
                return false;
            }
        }

        #endregion

        #region Sıra Çağırma

        /// <summary>
        /// Belirli bir TV'ye sıra çağırma bildirimi gönder
        /// </summary>
        /// <param name="tvId">TV ID</param>
        /// <param name="siraData">Sıra bilgisi</param>
        public async Task SendSiraUpdateToTv(int tvId, object siraData)
        {
            try
            {
                var groupName = $"TV_{tvId}";
                await SendToGroupAsync(groupName, "ReceiveSiraUpdate", siraData);
                
                _logger.LogInformation($"📤 Sıra güncellemesi gönderildi: TV#{tvId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Sıra güncellemesi gönderilemedi: TV#{tvId}");
                throw;
            }
        }

        /// <summary>
        /// Tüm TV'lere sıra çağırma bildirimi gönder
        /// </summary>
        /// <param name="siraData">Sıra bilgisi</param>
        public async Task BroadcastSiraUpdate(object siraData)
        {
            try
            {
                await BroadcastAsync("ReceiveSiraUpdate", siraData);
                _logger.LogInformation("📢 Sıra güncellemesi broadcast edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Sıra güncellemesi broadcast edilemedi");
                throw;
            }
        }

        #endregion

        #region Duyuru Yönetimi

        /// <summary>
        /// Belirli bir TV'ye duyuru gönder
        /// </summary>
        /// <param name="tvId">TV ID</param>
        /// <param name="duyuru">Duyuru metni</param>
        public async Task SendDuyuruToTv(int tvId, string duyuru)
        {
            try
            {
                var groupName = $"TV_{tvId}";
                await SendToGroupAsync(groupName, "ReceiveDuyuruUpdate", duyuru);
                
                _logger.LogInformation($"📤 Duyuru gönderildi: TV#{tvId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Duyuru gönderilemedi: TV#{tvId}");
                throw;
            }
        }

        /// <summary>
        /// Tüm TV'lere duyuru gönder
        /// </summary>
        /// <param name="duyuru">Duyuru metni</param>
        public async Task BroadcastDuyuru(string duyuru)
        {
            try
            {
                await BroadcastAsync("ReceiveDuyuruUpdate", duyuru);
                _logger.LogInformation("📢 Duyuru broadcast edildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Duyuru broadcast edilemedi");
                throw;
            }
        }

        #endregion

        #region Banko Yönetimi

        /// <summary>
        /// Banko durumu değişikliğini TV'ye bildir
        /// </summary>
        /// <param name="tvId">TV ID</param>
        /// <param name="bankoData">Banko bilgisi</param>
        public async Task SendBankoUpdateToTv(int tvId, object bankoData)
        {
            try
            {
                var groupName = $"TV_{tvId}";
                await SendToGroupAsync(groupName, "ReceiveBankoUpdate", bankoData);
                
                _logger.LogInformation($"📤 Banko güncellemesi TV'ye gönderildi: TV#{tvId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Banko güncellemesi TV'ye gönderilemedi: TV#{tvId}");
                throw;
            }
        }

        /// <summary>
        /// Belirli bir Banko'ya mesaj gönder
        /// </summary>
        /// <param name="bankoId">Banko ID</param>
        /// <param name="message">Mesaj</param>
        /// <param name="data">Veri</param>
        public async Task SendMessageToBanko(int bankoId, string message, object? data = null)
        {
            try
            {
                var groupName = $"Banko_{bankoId}";
                await SendToGroupAsync(groupName, message, data ?? new { });
                
                _logger.LogInformation($"📤 Mesaj Banko'ya gönderildi: Banko#{bankoId} -> {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Mesaj Banko'ya gönderilemedi: Banko#{bankoId}");
                throw;
            }
        }

        /// <summary>
        /// Banko'ya sıra bildirimi gönder
        /// </summary>
        /// <param name="bankoId">Banko ID</param>
        /// <param name="siraData">Sıra bilgisi</param>
        public async Task SendSiraToBanko(int bankoId, object siraData)
        {
            try
            {
                var groupName = $"Banko_{bankoId}";
                await SendToGroupAsync(groupName, "ReceiveSiraNotification", siraData);
                
                _logger.LogInformation($"📤 Sıra bildirimi Banko'ya gönderildi: Banko#{bankoId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Sıra bildirimi Banko'ya gönderilemedi: Banko#{bankoId}");
                throw;
            }
        }

        /// <summary>
        /// Tüm Bankolara broadcast mesaj gönder
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="data">Veri</param>
        public async Task BroadcastToBankolar(string message, object? data = null)
        {
            try
            {
                await BroadcastAsync(message, data ?? new { });
                _logger.LogInformation($"📢 Broadcast mesajı tüm Bankolara gönderildi: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Broadcast mesajı Bankolara gönderilemedi");
                throw;
            }
        }

        #endregion

        #region Ping/Pong - Bağlantı Kontrolü

        /// <summary>
        /// İstemciden ping al, pong gönder
        /// Bağlantı canlılığını kontrol etmek için kullanılır
        /// </summary>
        public async Task Ping()
        {
            try
            {
                await SendToCallerAsync("Pong", new { timestamp = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ping/Pong hatası");
            }
        }

        /// <summary>
        /// TV'nin bağlantı durumunu kontrol et
        /// </summary>
        /// <param name="tvId">TV ID</param>
        public async Task<bool> CheckTvConnection(int tvId)
        {
            try
            {
                return await _connectionService.IsTvConnectedAsync(tvId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ TV bağlantı kontrolü hatası: TV#{tvId}");
                return false;
            }
        }

        #endregion

        #region Admin Notifications

        /// <summary>
        /// Admin paneline bildirim gönder
        /// </summary>
        /// <param name="message">Bildirim mesajı</param>
        /// <param name="type">Bildirim tipi (success, warning, error, info)</param>
        public async Task SendAdminNotification(string message, string type = "info")
        {
            try
            {
                await SendToGroupAsync("Admins", "ReceiveAdminNotification", new { message, type, timestamp = DateTime.Now });
                _logger.LogInformation($"📤 Admin bildirimi gönderildi: {type} - {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Admin bildirimi gönderilemedi");
            }
        }

        /// <summary>
        /// Admin grubuna katıl
        /// </summary>
        public async Task JoinAdminGroup()
        {
            try
            {
                await JoinGroupAsync("Admins");
                _logger.LogInformation($"✅ Admin grubuna katıldı: {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Admin grubuna katılma hatası");
            }
        }

        /// <summary>
        /// Admin grubundan ayrıl
        /// </summary>
        public async Task LeaveAdminGroup()
        {
            try
            {
                await LeaveGroupAsync("Admins");
                _logger.LogInformation($"➖ Admin grubundan ayrıldı: {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Admin grubundan ayrılma hatası");
            }
        }

        #endregion
    }
}

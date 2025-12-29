using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGKPortalApp.DataAccessLayer.Context;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.ApiLayer.Services.BackgroundServices
{
    /// <summary>
    /// Orphan/stale session ve connection'ları temizleyen background service
    /// Her 10 dakikada bir çalışır
    /// </summary>
    public class SessionCleanupService : BackgroundService
    {
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(8); // 8 saat session timeout
        private readonly TimeSpan _hubConnectionTimeout = TimeSpan.FromMinutes(15); // 15 dakika hub timeout

        public SessionCleanupService(
            ILogger<SessionCleanupService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🧹 SessionCleanupService başlatıldı - Temizlik aralığı: {Interval}", _cleanupInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_cleanupInterval, stoppingToken);
                    await CleanupOrphanSessionsAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("🛑 SessionCleanupService durduruldu");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ SessionCleanupService hatası");
                }
            }
        }

        private async Task CleanupOrphanSessionsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SGKDbContext>();

            try
            {
                var now = DateTime.Now;
                var sessionTimeoutThreshold = now.Add(-_sessionTimeout);
                var hubTimeoutThreshold = now.Add(-_hubConnectionTimeout);

                // 1️⃣ Orphan LoginLogoutLog kayıtlarını temizle (LogoutTime null ve çok eski)
                var orphanSessions = await context.LoginLogoutLogs
                    .Where(l => !l.LogoutTime.HasValue && l.LoginTime < sessionTimeoutThreshold)
                    .ToListAsync(cancellationToken);

                if (orphanSessions.Any())
                {
                    _logger.LogWarning("🧹 {Count} orphan session bulundu, temizleniyor...", orphanSessions.Count);

                    foreach (var session in orphanSessions)
                    {
                        // Timeout nedeniyle otomatik logout
                        session.LogoutTime = session.LoginTime.Add(_sessionTimeout);
                        _logger.LogInformation("🧹 Orphan session kapatıldı: SessionID={SessionID}, TC={TcKimlikNo}, LoginTime={LoginTime}",
                            session.SessionID, session.TcKimlikNo, session.LoginTime);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("✅ {Count} orphan session temizlendi", orphanSessions.Count);
                }

                // 2️⃣ Stale HubConnection kayıtlarını pasifleştir (AktifMi=true ama çok eski)
                var staleConnections = await context.HubConnections
                    .Where(h => h.AktifMi && h.ConnectedAt < hubTimeoutThreshold)
                    .ToListAsync(cancellationToken);

                if (staleConnections.Any())
                {
                    _logger.LogWarning("🧹 {Count} stale HubConnection bulundu, pasifleştiriliyor...", staleConnections.Count);

                    foreach (var conn in staleConnections)
                    {
                        conn.AktifMi = false;
                        conn.DisconnectedAt = now;
                        _logger.LogInformation("🧹 Stale HubConnection pasifleştirildi: ConnectionId={ConnectionId}, TC={TcKimlikNo}, ConnectedAt={ConnectedAt}",
                            conn.ConnectionId, conn.TcKimlikNo, conn.ConnectedAt);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("✅ {Count} stale HubConnection pasifleştirildi", staleConnections.Count);
                }

                // 3️⃣ Pasif olmayan ama bağlı HubConnection olmayan LoginLogoutLog kayıtlarını temizle
                // (SignalR disconnect çalıştı ama OnDisconnectedAsync logout kaydını güncelleyemedi)
                var disconnectedButNotLoggedOut = await context.LoginLogoutLogs
                    .Where(l => !l.LogoutTime.HasValue && l.SessionID != null)
                    .Where(l => !context.HubConnections.Any(h =>
                        h.AktifMi &&
                        context.Users.Any(u => u.SessionID == l.SessionID && u.TcKimlikNo == h.TcKimlikNo)))
                    .Where(l => l.LoginTime < hubTimeoutThreshold)
                    .ToListAsync(cancellationToken);

                if (disconnectedButNotLoggedOut.Any())
                {
                    _logger.LogWarning("🧹 {Count} disconnected-but-not-logged-out session bulundu", disconnectedButNotLoggedOut.Count);

                    foreach (var session in disconnectedButNotLoggedOut)
                    {
                        // Hub connection yok ama logout kaydı yok → timeout yap
                        session.LogoutTime = now;
                        _logger.LogInformation("🧹 Disconnected session kapatıldı: SessionID={SessionID}, TC={TcKimlikNo}",
                            session.SessionID, session.TcKimlikNo);
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("✅ {Count} disconnected session temizlendi", disconnectedButNotLoggedOut.Count);
                }

                if (!orphanSessions.Any() && !staleConnections.Any() && !disconnectedButNotLoggedOut.Any())
                {
                    _logger.LogDebug("✨ Session cleanup tamamlandı - temizlenecek kayıt yok");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CleanupOrphanSessionsAsync hatası");
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;

namespace SGKPortalApp.BusinessLogicLayer.Services.SignalR
{
    /// <summary>
    /// Sıramatik SignalR yayın servisi implementasyonu
    /// Business katmanından hub'a mesaj göndermek için kullanılır
    /// 
    /// NOT: Bu servis ISignalRBroadcaster aracılığıyla Presentation katmanındaki
    /// hub'a mesaj gönderir. Doğrudan IHubContext kullanmaz.
    /// </summary>
    public class SiramatikHubService : ISiramatikHubService
    {
        private readonly ISignalRBroadcaster _broadcaster;
        private readonly IHubConnectionRepository _hubConnectionRepository;
        private readonly ISiramatikQueryRepository _siramatikQueryRepository;
        private readonly ILogger<SiramatikHubService> _logger;

        // SignalR Event sabitleri (SignalREvents.cs ile senkron tutulmalı)
        private const string SiraListUpdate = "siraListUpdate";
        private const string ReceiveSiraUpdate = "receiveSiraUpdate";

        public SiramatikHubService(
            ISignalRBroadcaster broadcaster,
            IHubConnectionRepository hubConnectionRepository,
            ISiramatikQueryRepository siramatikQueryRepository,
            ILogger<SiramatikHubService> logger)
        {
            _broadcaster = broadcaster;
            _hubConnectionRepository = hubConnectionRepository;
            _siramatikQueryRepository = siramatikQueryRepository;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════
        // SIRA ÇAĞIRMA PANELİ BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task BroadcastSiraCalledAsync(
            SiraCagirmaResponseDto sira,
            int callerBankoId,
            string bankoNo,
            string callerPersonelTc)
        {
            try
            {
                // 1. Etkilenen personelleri bul (aynı KanalAltIslem'e atanmış ve banko modunda olanlar)
                var affectedPersonels = await _siramatikQueryRepository.GetSiraEtkilenenPersonellerAsync(sira.SiraId);
                affectedPersonels = affectedPersonels.Where(tc => tc != callerPersonelTc).ToList();

                // 2. Diğer banko panellerine REMOVE bildirimi gönder
                if (affectedPersonels.Any())
                {
                    var panelPayload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = sira,
                        BankoId = callerBankoId,
                        PersonelTc = callerPersonelTc,
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, panelPayload);
                    _logger.LogInformation("📤 SiraCalled panel broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        sira.SiraId, affectedPersonels.Count);
                }

                // 3. TV ekranlarına bildirim gönder
                if (!string.IsNullOrEmpty(bankoNo) && sira.HizmetBinasiId > 0)
                {
                    await BroadcastSiraToTvAsync(sira, bankoNo, sira.HizmetBinasiId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCalled broadcast hatası. SiraId: {SiraId}", sira.SiraId);
            }
        }

        public async Task BroadcastSiraCompletedAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId)
        {
            try
            {
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(hizmetBinasiId, kanalAltIslemId);

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = new SiraCagirmaResponseDto { SiraId = siraId },
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 SiraCompleted broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        siraId, affectedPersonels.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCompleted broadcast hatası. SiraId: {SiraId}", siraId);
            }
        }

        public async Task BroadcastSiraCancelledAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId)
        {
            try
            {
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(hizmetBinasiId, kanalAltIslemId);

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = new SiraCagirmaResponseDto { SiraId = siraId },
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 SiraCancelled broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        siraId, affectedPersonels.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCancelled broadcast hatası. SiraId: {SiraId}", siraId);
            }
        }

        public async Task BroadcastSiraRedirectedAsync(
            SiraCagirmaResponseDto sira,
            int sourceBankoId,
            int targetBankoId,
            string sourcePersonelTc)
        {
            try
            {
                // Kaynak personele REMOVE gönder
                var removePayload = new SiraUpdatePayloadDto
                {
                    UpdateType = SiraUpdateType.Remove,
                    Sira = sira,
                    BankoId = sourceBankoId,
                    PersonelTc = sourcePersonelTc,
                    Aciklama = "Sıra yönlendirildi",
                    Timestamp = DateTime.Now
                };

                await SendToPersonelAsync(sourcePersonelTc, SiraListUpdate, removePayload);

                // Hedef bankodaki personellere INSERT gönder
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(sira.HizmetBinasiId, sira.KanalAltIslemId);
                var targetPersonels = affectedPersonels.Where(tc => tc != sourcePersonelTc).ToList();

                if (targetPersonels.Any())
                {
                    var insertPayload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Insert,
                        Sira = sira,
                        BankoId = targetBankoId,
                        Position = 0, // En başa ekle (yönlendirilen sıralar öncelikli)
                        Aciklama = "Yönlendirilmiş sıra",
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(targetPersonels, SiraListUpdate, insertPayload);
                }

                _logger.LogInformation("📤 SiraRedirected broadcast edildi. SiraId: {SiraId}, Kaynak: {SourceBanko}, Hedef: {TargetBanko}",
                    sira.SiraId, sourceBankoId, targetBankoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraRedirected broadcast hatası. SiraId: {SiraId}", sira.SiraId);
            }
        }

        // ═══════════════════════════════════════════════════════
        // KIOSK / YENİ SIRA BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task BroadcastNewSiraAsync(
            SiraCagirmaResponseDto sira,
            int hizmetBinasiId,
            int kanalAltIslemId)
        {
            try
            {
                _logger.LogInformation("🔍 BroadcastNewSiraAsync başladı. SiraNo: {SiraNo}, HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                    sira.SiraNo, hizmetBinasiId, kanalAltIslemId);

                // Bu KanalAltIslem'e atanmış ve banko modunda olan personelleri bul
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(hizmetBinasiId, kanalAltIslemId);

                _logger.LogInformation("🔍 Etkilenen personeller: {Count} kişi, TC'ler: [{TcList}]",
                    affectedPersonels.Count, string.Join(", ", affectedPersonels));

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Append, // Listenin sonuna ekle
                        Sira = sira,
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 NewSira (Kiosk) broadcast edildi. SiraId: {SiraId}, SiraNo: {SiraNo}, Etkilenen: {Count} personel",
                        sira.SiraId, sira.SiraNo, affectedPersonels.Count);
                }
                else
                {
                    _logger.LogWarning("⚠️ BroadcastNewSiraAsync: Etkilenen personel bulunamadı! HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                        hizmetBinasiId, kanalAltIslemId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ NewSira broadcast hatası. SiraId: {SiraId}", sira.SiraId);
            }
        }

        // ═══════════════════════════════════════════════════════
        // TV EKRANI BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        public async Task BroadcastSiraToTvAsync(
            SiraCagirmaResponseDto sira,
            string bankoNo,
            int hizmetBinasiId)
        {
            try
            {
                var tvPayload = new
                {
                    siraNo = sira.SiraNo,
                    bankoNo = bankoNo,
                    kanalAltAdi = sira.KanalAltAdi,
                    timestamp = DateTime.Now
                };

                // Hizmet binasındaki tüm TV'lere gönder
                var groupName = $"HIZMETBINASI_{hizmetBinasiId}";
                await _broadcaster.SendToGroupAsync(groupName, ReceiveSiraUpdate, tvPayload);

                _logger.LogInformation("📺 TV broadcast edildi. SiraNo: {SiraNo}, BankoNo: {BankoNo}, HizmetBinasi: {HizmetBinasiId}",
                    sira.SiraNo, bankoNo, hizmetBinasiId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TV broadcast hatası. SiraNo: {SiraNo}", sira.SiraNo);
            }
        }

        #region Private Helper Methods

        private async Task SendToPersonelsAsync(List<string> personelTcs, string eventName, object payload)
        {
            if (!personelTcs.Any()) return;

            var connectionIds = new List<string>();
            foreach (var tc in personelTcs)
            {
                var connections = (await _hubConnectionRepository.GetActiveConnectionsByTcKimlikNoAsync(tc)).ToList();
                var typesList = string.Join(", ", connections.Select(c => $"{c.ConnectionType}:{c.ConnectionId}"));
                _logger.LogInformation("🔍 TC: {Tc} için {Count} aktif bağlantı bulundu. Tipler: {Types}",
                    tc, connections.Count, typesList);
                
                connectionIds.AddRange(connections
                    .Where(c => c.ConnectionType == "BankoMode")
                    .Select(c => c.ConnectionId));
            }

            var idsString = string.Join(", ", connectionIds);
            _logger.LogInformation("🔍 BankoMode connection sayısı: {Count}, IDs: {Ids}",
                connectionIds.Count, idsString);

            if (connectionIds.Any())
            {
                await _broadcaster.SendToConnectionsAsync(connectionIds, eventName, payload);
                _logger.LogInformation("📤 {EventName} gönderildi: {Count} connection'a", eventName, connectionIds.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ {EventName} gönderilemedi: BankoMode connection bulunamadı!", eventName);
            }
        }

        private async Task SendToPersonelAsync(string personelTc, string eventName, object payload)
        {
            var connections = await _hubConnectionRepository.GetActiveConnectionsByTcKimlikNoAsync(personelTc);
            var connectionIds = connections
                .Where(c => c.ConnectionType == "BankoMode")
                .Select(c => c.ConnectionId)
                .ToList();

            if (connectionIds.Any())
            {
                await _broadcaster.SendToConnectionsAsync(connectionIds, eventName, payload);
                _logger.LogDebug("📤 {EventName} gönderildi: {PersonelTc}", eventName, personelTc);
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════
        // ⭐ INCREMENTAL UPDATE
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// ⭐ Sıra alındığında/yönlendirildiğinde etkilenen personellere güncel listeyi gönder
        /// ConnectionId ile direkt mesaj gönderilir
        /// </summary>
        public async Task BroadcastBankoPanelGuncellemesiAsync(int siraId)
        {
            try
            {
                _logger.LogInformation("🔍 BankoPanelGuncellemesi başladı. SiraId: {SiraId}", siraId);

                // Repository'den tüm satırları al (PersonelTc + ConnectionId içeren)
                var rawData = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(siraId);

                if (!rawData.Any())
                {
                    _logger.LogWarning("⚠️ SiraId: {SiraId} için etkilenen personel bulunamadı!", siraId);
                    return;
                }

                // PersonelTc ve ConnectionId'ye göre grupla
                var personelGroups = rawData
                    .GroupBy(x => new { x.PersonelTc, x.ConnectionId })
                    .Select(g => new
                    {
                        PersonelTc = g.Key.PersonelTc!,
                        ConnectionId = g.Key.ConnectionId!,
                        Siralar = g.OrderBy(s => s.SiraAlisZamani).ThenBy(s => s.SiraNo).ToList()
                    })
                    .ToList();

                _logger.LogInformation("🔍 {Count} personele mesaj gönderilecek", personelGroups.Count);

                // Her personele kendi ConnectionId üzerinden direkt mesaj gönder
                foreach (var group in personelGroups)
                {
                    var payload = new
                    {
                        siraId = siraId,
                        personelTc = group.PersonelTc,
                        siralar = group.Siralar,
                        timestamp = DateTime.Now
                    };

                    await _broadcaster.SendToConnectionAsync(group.ConnectionId, "BankoPanelSiraGuncellemesi", payload);

                    _logger.LogInformation("📤 BankoPanelGuncellemesi gönderildi. TC: {PersonelTc}, ConnectionId: {ConnectionId}, Sıra sayısı: {Count}",
                        group.PersonelTc, group.ConnectionId, group.Siralar.Count);
                }

                _logger.LogInformation("✅ BankoPanelGuncellemesi tamamlandı. SiraId: {SiraId}, Etkilenen: {Count} personel",
                    siraId, personelGroups.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ BankoPanelGuncellemesi hatası. SiraId: {SiraId}", siraId);
            }
        }

        #endregion
    }
}

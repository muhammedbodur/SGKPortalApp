using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
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
                _logger.LogInformation("🔄 SiraRedirected broadcast başladı. SiraId: {SiraId}, YonlendirmeTipi: {YonlendirmeTipi}, Kaynak: {SourceBanko}, Hedef: {TargetBanko}",
                    sira.SiraId, sira.YonlendirmeTipi, sourceBankoId, targetBankoId);

                // 1. Kaynak personele REMOVE gönder
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
                _logger.LogInformation("📤 Kaynak personele REMOVE gönderildi. PersonelTc: {PersonelTc}", sourcePersonelTc);

                // 2. Hedef personelleri yönlendirme tipine göre akıllı şekilde bul
                List<string> targetPersonels = new List<string>();
                string aciklama = "";

                switch (sira.YonlendirmeTipi)
                {
                    case BusinessObjectLayer.Enums.SiramatikIslemleri.YonlendirmeTipi.BaskaBanko:
                        // Sadece hedef bankodaki personele gönder
                        targetPersonels = await _siramatikQueryRepository.GetBankodakiAktifPersonellerAsync(targetBankoId);
                        aciklama = "Başka bankoya yönlendirilmiş sıra";
                        _logger.LogInformation("🎯 BaskaBanko yönlendirme. HedefBankoId: {TargetBankoId}, Hedef personel sayısı: {Count}",
                            targetBankoId, targetPersonels.Count);
                        break;

                    case BusinessObjectLayer.Enums.SiramatikIslemleri.YonlendirmeTipi.Sef:
                        // Sadece Şef yetkisine sahip personellere gönder
                        targetPersonels = await _siramatikQueryRepository.GetBankoModundakiSefPersonellerAsync(sira.HizmetBinasiId, sira.KanalAltIslemId);
                        aciklama = "Şef'e yönlendirilmiş sıra";
                        _logger.LogInformation("🎯 Şef yönlendirme. Şef personel sayısı: {Count}", targetPersonels.Count);
                        break;

                    case BusinessObjectLayer.Enums.SiramatikIslemleri.YonlendirmeTipi.UzmanPersonel:
                        // Sadece Uzman yetkisine sahip personellere gönder
                        targetPersonels = await _siramatikQueryRepository.GetBankoModundakiUzmanPersonellerAsync(sira.HizmetBinasiId, sira.KanalAltIslemId);
                        aciklama = "Uzman personele yönlendirilmiş sıra";
                        _logger.LogInformation("🎯 Uzman yönlendirme. Uzman personel sayısı: {Count}", targetPersonels.Count);
                        break;

                    default:
                        _logger.LogWarning("⚠️ Bilinmeyen YonlendirmeTipi: {YonlendirmeTipi}. Tüm personellere gönderiliyor.", sira.YonlendirmeTipi);
                        targetPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(sira.HizmetBinasiId, sira.KanalAltIslemId);
                        aciklama = "Yönlendirilmiş sıra";
                        break;
                }

                // Kaynak personeli hedef listesinden çıkar
                targetPersonels = targetPersonels.Where(tc => tc != sourcePersonelTc).ToList();

                // 3. Hedef personellere INSERT gönder
                if (targetPersonels.Any())
                {
                    var insertPayload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Insert,
                        Sira = sira,
                        BankoId = targetBankoId,
                        Position = 0, // En başa ekle (yönlendirilen sıralar öncelikli)
                        Aciklama = aciklama,
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(targetPersonels, SiraListUpdate, insertPayload);
                    _logger.LogInformation("📤 Hedef personellere INSERT gönderildi. Personel sayısı: {Count}, TC'ler: [{TcList}]",
                        targetPersonels.Count, string.Join(", ", targetPersonels));
                }
                else
                {
                    _logger.LogWarning("⚠️ Hedef personel bulunamadı! YonlendirmeTipi: {YonlendirmeTipi}, SiraId: {SiraId}",
                        sira.YonlendirmeTipi, sira.SiraId);
                }

                _logger.LogInformation("✅ SiraRedirected broadcast tamamlandı. SiraId: {SiraId}, YonlendirmeTipi: {YonlendirmeTipi}",
                    sira.SiraId, sira.YonlendirmeTipi);
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
    }
}

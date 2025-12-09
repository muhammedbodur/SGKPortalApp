using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Interfaces.SignalR;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;

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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SiramatikHubService> _logger;

        // SignalR Event sabitleri (SignalREvents.cs ile senkron tutulmalı)
        private const string SiraListUpdate = "siraListUpdate";
        private const string ReceiveSiraUpdate = "receiveSiraUpdate";

        public SiramatikHubService(
            ISignalRBroadcaster broadcaster,
            IHubConnectionRepository hubConnectionRepository,
            ISiramatikQueryRepository siramatikQueryRepository,
            IUnitOfWork unitOfWork,
            ILogger<SiramatikHubService> logger)
        {
            _broadcaster = broadcaster;
            _hubConnectionRepository = hubConnectionRepository;
            _siramatikQueryRepository = siramatikQueryRepository;
            _unitOfWork = unitOfWork;
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
            int? targetBankoId,
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

                // Hedef veya müsait tüm personellere INSERT gönder - her personel için komşu sıraları hesapla
                var targetPersonelSiralar = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(sira.SiraId);
                
                if (targetBankoId.HasValue)
                {
                    targetPersonelSiralar = targetPersonelSiralar
                        .Where(x => x.BankoId == targetBankoId.Value)
                        .ToList();
                }

                if (targetPersonelSiralar.Any())
                {
                    // PersonelTc'ye göre grupla
                    var personelGroups = targetPersonelSiralar
                        .Where(x => x.PersonelTc != sourcePersonelTc) // Kaynak personeli hariç tut
                        .GroupBy(x => x.PersonelTc)
                        .ToList();

                    foreach (var group in personelGroups)
                    {
                        var personelTc = group.Key;
                        // ⭐ Backend'den gelen sıralama zaten doğru, değiştirme
                        var siralar = group.ToList();
                        
                        // Yönlendirilen sıranın pozisyonunu bul
                        var siraIndex = siralar.FindIndex(s => s.SiraId == sira.SiraId);
                        
                        // ⭐ Backend'den gelen güncel sıra bilgisini kullan (YonlendirmeAciklamasi dahil)
                        var guncelSira = siraIndex >= 0 ? siralar[siraIndex] : sira;
                        
                        int? previousSiraId = null;
                        int? nextSiraId = null;
                        
                        if (siraIndex > 0)
                        {
                            previousSiraId = siralar[siraIndex - 1].SiraId;
                        }
                        if (siraIndex >= 0 && siraIndex < siralar.Count - 1)
                        {
                            nextSiraId = siralar[siraIndex + 1].SiraId;
                        }

                        var hedefBanko = targetBankoId ?? siralar.FirstOrDefault()?.BankoId ?? 0;

                        var insertPayload = new SiraUpdatePayloadDto
                        {
                            UpdateType = SiraUpdateType.Insert,
                            Sira = guncelSira, // ⭐ Güncel sıra bilgisi (YonlendirmeAciklamasi dahil)
                            BankoId = hedefBanko,
                            Position = siraIndex >= 0 ? siraIndex : 0,
                            PreviousSiraId = previousSiraId,
                            NextSiraId = nextSiraId,
                            Aciklama = "Yönlendirilmiş sıra",
                            Timestamp = DateTime.Now
                        };

                        await SendToPersonelAsync(personelTc!, SiraListUpdate, insertPayload);
                        
                        _logger.LogInformation("📤 INSERT gönderildi: TC={PersonelTc}, SiraId={SiraId}, BankoId={BankoId}, Prev={Prev}, Next={Next}",
                            personelTc, sira.SiraId, hedefBanko, previousSiraId, nextSiraId);
                    }
                }
                else if (!targetBankoId.HasValue)
                {
                    _logger.LogInformation("ℹ️ Şef/Uzman yönlendirmesinde aktif uzman bulunamadı, sadece kaynak personel bilgilendirildi. SiraId: {SiraId}",
                        sira.SiraId);
                }

                _logger.LogInformation("📤 SiraRedirected broadcast edildi. SiraId: {SiraId}, Kaynak: {SourceBanko}, Hedef: {TargetBanko}",
                    sira.SiraId, sourceBankoId, targetBankoId.HasValue ? targetBankoId.Value.ToString() : "Yok");
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
                // ⭐ Profesyonel DTO yapısı
                var tvPayload = new TvSiraUpdateDto
                {
                    SiraNo = sira.SiraNo,
                    BankoNo = bankoNo,
                    KanalAltAdi = sira.KanalAltAdi,
                    Timestamp = DateTime.Now
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

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// HubTvConnection tablosu üzerinden aktif TV bağlantılarına mesaj gönderir
        /// Sıra çağırma paneli gibi tüm güncel listeyi gönderir
        /// </summary>
        public async Task BroadcastSiraCalledToTvAsync(SiraCagirmaResponseDto sira, int bankoId, string bankoNo)
        {
            try
            {
                // Banko bilgilerini al (katTipi, bankoNo ve bankoTipi için)
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var banko = await bankoRepo.GetByIdAsync(bankoId);

                string katTipi = banko?.KatTipi.GetDisplayName() ?? "";
                string bankoTipi = banko?.BankoTipi.GetDisplayName() ?? "BANKO";
                // bankoNo parametresi boş gelebilir, veritabanından al
                string actualBankoNo = !string.IsNullOrEmpty(bankoNo) ? bankoNo : (banko?.BankoNo.ToString() ?? "");

                // Bu bankoya bağlı TV'leri bul (TvBanko tablosundan)
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvBankolar = await tvRepo.GetTvBankolarByBankoIdAsync(bankoId);

                if (tvBankolar == null || !tvBankolar.Any())
                {
                    _logger.LogDebug("ℹ️ Banko#{BankoId} için bağlı TV bulunamadı", bankoId);
                    return;
                }

                // HubTvConnection tablosundan aktif TV bağlantılarını al
                var hubTvConnectionRepo = _unitOfWork.GetRepository<IHubTvConnectionRepository>();
                var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();

                foreach (var tvBanko in tvBankolar)
                {
                    // Bu TV'nin aktif bağlantılarını bul
                    var tvConnections = await hubTvConnectionRepo.GetByTvAsync(tvBanko.TvId);
                    var connectionIds = tvConnections
                        .Where(tc => tc.HubConnection != null && !string.IsNullOrEmpty(tc.HubConnection.ConnectionId))
                        .Select(tc => tc.HubConnection!.ConnectionId)
                        .ToList();

                    if (!connectionIds.Any())
                    {
                        _logger.LogDebug("ℹ️ TV#{TvId} için aktif bağlantı bulunamadı", tvBanko.TvId);
                        continue;
                    }

                    // Bu TV'ye bağlı tüm bankoların ID'lerini al
                    var tvninBankolari = await tvRepo.GetTvBankolarAsync(tvBanko.TvId);
                    var bankoIds = tvninBankolari.Select(tb => tb.BankoId).ToList();

                    // Tüm bankolardaki güncel sıraları al (sıra çağırma paneli mantığı)
                    var aktifHareketler = await bankoHareketRepo.GetAktifSiralarByBankoIdsAsync(bankoIds);
                    var siralar = aktifHareketler.Select(bh => new TvSiraItemDto
                    {
                        BankoId = bh.BankoId,
                        BankoNo = bh.Banko?.BankoNo ?? 0,
                        KatTipi = bh.Banko?.KatTipi.GetDisplayName() ?? "",
                        SiraNo = bh.SiraNo
                    }).ToList();

                    // ⭐ Profesyonel DTO yapısı (Request/Command Pattern)
                    var tvPayload = new TvSiraCalledDto
                    {
                        SiraNo = sira.SiraNo,
                        BankoNo = actualBankoNo,
                        BankoId = bankoId,
                        BankoTipi = bankoTipi,
                        KatTipi = katTipi,
                        KanalAltAdi = sira.KanalAltAdi,
                        UpdateType = "SiraCalled",
                        Siralar = siralar,
                        Timestamp = DateTime.Now
                    };

                    await _broadcaster.SendToConnectionsAsync(connectionIds, "TvSiraGuncellendi", tvPayload);
                    _logger.LogInformation("📺 TV#{TvId}'ye sıra bildirimi gönderildi: Sıra#{SiraNo}, Liste: {Count} sıra, {ConnCount} bağlantı",
                        tvBanko.TvId, sira.SiraNo, siralar.Count, connectionIds.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TV sıra çağırma broadcast hatası. SiraId: {SiraId}, BankoId: {BankoId}", sira.SiraId, bankoId);
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
        /// Sıra alındığında/yönlendirildiğinde etkilenen personellere güncelleme gönderir
        /// </summary>
        public async Task BroadcastBankoPanelGuncellemesiAsync(int siraId)
        {
            try
            {
                _logger.LogInformation("🔍 BankoPanelGuncellemesi başladı. SiraId: {SiraId}", siraId);

                // Sıra bilgisini al
                var siraRepo = _unitOfWork.GetRepository<ISiraRepository>();
                var sira = await siraRepo.GetByIdAsync(siraId);
                if (sira == null)
                {
                    _logger.LogWarning("⚠️ SiraId: {SiraId} bulunamadı!", siraId);
                    return;
                }

                _logger.LogInformation("🔍 Sıra bulundu. KanalAltIslemId: {KanalAltIslemId}, HizmetBinasiId: {HizmetBinasiId}",
                    sira.KanalAltIslemId, sira.HizmetBinasiId);

                // Repository'den tüm satırları al (PersonelTc + ConnectionId içeren)
                var rawData = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(siraId);

                _logger.LogInformation("🔍 GetBankoPanelBekleyenSiralarBySiraIdAsync sonucu: {Count} satır", rawData.Count);

                if (!rawData.Any())
                {
                    _logger.LogWarning("⚠️ SiraId: {SiraId} için etkilenen personel bulunamadı! HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                        siraId, sira.HizmetBinasiId, sira.KanalAltIslemId);
                    return;
                }

                // PersonelTc ve ConnectionId'ye göre grupla
                // ⭐ Backend'den gelen sıralama zaten doğru (GetBankoPanelBekleyenSiralarBySiraIdAsync)
                var personelGroups = rawData
                    .GroupBy(x => new { x.PersonelTc, x.ConnectionId })
                    .Select(g => new
                    {
                        PersonelTc = g.Key.PersonelTc!,
                        ConnectionId = g.Key.ConnectionId!,
                        Siralar = g.ToList() // Sıralama zaten doğru
                    })
                    .ToList();

                _logger.LogInformation("🔍 {Count} personele mesaj gönderilecek", personelGroups.Count);

                // Her personele kendi ConnectionId üzerinden direkt mesaj gönder
                foreach (var group in personelGroups)
                {
                    // ⭐ Sadece tetikleyen sırayı bul ve pozisyonunu hesapla
                    var tetikleyenSira = group.Siralar.FirstOrDefault(s => s.SiraId == siraId);
                    var pozisyon = tetikleyenSira != null ? group.Siralar.IndexOf(tetikleyenSira) : -1;

                    // ⭐ Profesyonel DTO yapısı (Request/Command Pattern)
                    var payload = new BankoPanelSiraGuncellemesiDto
                    {
                        SiraId = siraId,
                        PersonelTc = group.PersonelTc,
                        Sira = tetikleyenSira,
                        Pozisyon = pozisyon,
                        ToplamSiraSayisi = group.Siralar.Count,
                        Timestamp = DateTime.Now
                    };

                    await _broadcaster.SendToConnectionsAsync(new[] { group.ConnectionId }, "BankoPanelSiraGuncellemesi", payload);

                    _logger.LogInformation("📤 BankoPanelGuncellemesi gönderildi. TC: {PersonelTc}, SiraNo: {SiraNo}, Pozisyon: {Pozisyon}/{Toplam}",
                        group.PersonelTc, tetikleyenSira?.SiraNo ?? 0, pozisyon, group.Siralar.Count);
                }

                _logger.LogInformation("✅ BankoPanelGuncellemesi tamamlandı. SiraId: {SiraId}, Etkilenen: {Count} personel",
                    siraId, personelGroups.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ BankoPanelGuncellemesi hatası. SiraId: {SiraId}", siraId);
            }
        }
    }
}

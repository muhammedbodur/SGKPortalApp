using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
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

        /// <summary>
        /// Sıra çağrıldığında ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraCalledAsync(BroadcastSiraCalledRequest request)
        {
            try
            {
                // 1. Etkilenen personelleri bul (aynı KanalAltIslem'e atanmış ve banko modunda olanlar)
                var affectedPersonels = await _siramatikQueryRepository.GetSiraEtkilenenPersonellerAsync(request.Sira.SiraId);
                affectedPersonels = affectedPersonels.Where(tc => tc != request.CallerPersonelTc).ToList();

                // 2. Diğer banko panellerine REMOVE bildirimi gönder
                if (affectedPersonels.Any())
                {
                    var panelPayload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = request.Sira,
                        BankoId = request.CallerBankoId,
                        PersonelTc = request.CallerPersonelTc,
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, panelPayload);
                    _logger.LogInformation("📤 SiraCalled panel broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        request.Sira.SiraId, affectedPersonels.Count);
                }

                // 3. TV ekranlarına bildirim gönder
                if (!string.IsNullOrEmpty(request.BankoNo) && request.Sira.HizmetBinasiId > 0)
                {
                    await BroadcastSiraToTvAsync(new BroadcastSiraToTvRequest
                    {
                        Sira = request.Sira,
                        BankoNo = request.BankoNo,
                        HizmetBinasiId = request.Sira.HizmetBinasiId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCalled broadcast hatası. SiraId: {SiraId}", request.Sira.SiraId);
            }
        }

        /// <summary>
        /// Sıra tamamlandığında ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraCompletedAsync(BroadcastSiraCompletedRequest request)
        {
            try
            {
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(
                    request.HizmetBinasiId, request.KanalAltIslemId);

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = new SiraCagirmaResponseDto { SiraId = request.SiraId },
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 SiraCompleted broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        request.SiraId, affectedPersonels.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCompleted broadcast hatası. SiraId: {SiraId}", request.SiraId);
            }
        }

        /// <summary>
        /// Sıra iptal edildiğinde ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraCancelledAsync(BroadcastSiraCancelledRequest request)
        {
            try
            {
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(
                    request.HizmetBinasiId, request.KanalAltIslemId);

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Remove,
                        Sira = new SiraCagirmaResponseDto { SiraId = request.SiraId },
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 SiraCancelled broadcast edildi. SiraId: {SiraId}, Etkilenen: {Count} personel",
                        request.SiraId, affectedPersonels.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraCancelled broadcast hatası. SiraId: {SiraId}", request.SiraId);
            }
        }

        /// <summary>
        /// Sıra yönlendirildiğinde kaynak ve hedef banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraRedirectedAsync(BroadcastSiraRedirectedRequest request)
        {
            try
            {
                // Kaynak personele REMOVE gönder
                var removePayload = new SiraUpdatePayloadDto
                {
                    UpdateType = SiraUpdateType.Remove,
                    Sira = request.Sira,
                    BankoId = request.SourceBankoId,
                    PersonelTc = request.SourcePersonelTc,
                    Aciklama = "Sıra yönlendirildi",
                    Timestamp = DateTime.Now
                };

                await SendToPersonelAsync(request.SourcePersonelTc, SiraListUpdate, removePayload);

                // Hedef veya müsait tüm personellere INSERT gönder - her personel için komşu sıraları hesapla
                var targetPersonelSiralar = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(request.Sira.SiraId);

                _logger.LogInformation("🔍 Yönlendirme hedef sorgusu: SiraId={SiraId}, Bulunan={Count} kayıt, TargetBankoId={TargetBankoId}",
                    request.Sira.SiraId, targetPersonelSiralar.Count, request.TargetBankoId);

                if (request.TargetBankoId.HasValue)
                {
                    var beforeFilter = targetPersonelSiralar.Count;
                    targetPersonelSiralar = targetPersonelSiralar
                        .Where(x => x.BankoId == request.TargetBankoId.Value)
                        .ToList();

                    _logger.LogInformation("🔍 Banko filtresi uygulandı: {Before} -> {After} kayıt (BankoId={BankoId})",
                        beforeFilter, targetPersonelSiralar.Count, request.TargetBankoId.Value);
                }

                if (targetPersonelSiralar.Any())
                {
                    // PersonelTc'ye göre grupla
                    var personelGroups = targetPersonelSiralar
                        .Where(x => x.PersonelTc != request.SourcePersonelTc) // Kaynak personeli hariç tut
                        .GroupBy(x => x.PersonelTc)
                        .ToList();

                    _logger.LogInformation("🔍 Hedef personel grupları: {Count} grup (Kaynak personel hariç: {SourceTc})",
                        personelGroups.Count, request.SourcePersonelTc);

                    if (!personelGroups.Any())
                    {
                        _logger.LogWarning("⚠️ Hedef personel bulunamadı! Tüm personeller kaynak personel ({SourceTc}) veya filtrelerden elendi.",
                            request.SourcePersonelTc);
                    }

                    foreach (var group in personelGroups)
                    {
                        var personelTc = group.Key;
                        // ⭐ Backend'den gelen sıralama zaten doğru, değiştirme
                        var siralar = group.ToList();

                        // Yönlendirilen sıranın pozisyonunu bul
                        var siraIndex = siralar.FindIndex(s => s.SiraId == request.Sira.SiraId);

                        // ⭐ Backend'den gelen güncel sıra bilgisini kullan (YonlendirmeAciklamasi dahil)
                        var guncelSira = siraIndex >= 0 ? siralar[siraIndex] : request.Sira;

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

                        var hedefBanko = request.TargetBankoId ?? siralar.FirstOrDefault()?.BankoId ?? 0;

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
                            personelTc, request.Sira.SiraId, hedefBanko, previousSiraId, nextSiraId);
                    }
                }
                else
                {
                    // Liste boş - hedef personel bulunamadı
                    if (request.TargetBankoId.HasValue)
                    {
                        _logger.LogWarning("⚠️ BAŞKA BANKO yönlendirmesi - Hedef bankoda (BankoId={BankoId}) aktif personel bulunamadı! " +
                            "Olası sebepler: 1) Personel offline, 2) BankoModuAktif=false, 3) Başka binada. SiraId: {SiraId}",
                            request.TargetBankoId.Value, request.Sira.SiraId);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ ŞEF/UZMAN yönlendirmesi - Aktif şef/uzman personel bulunamadı! " +
                            "Olası sebepler: 1) Personel offline, 2) BankoModuAktif=false, 3) Başka binada, 4) Uzmanlık seviyesi uyumsuz. " +
                            "YonlendirmeTipi={YonlendirmeTipi}, SiraId: {SiraId}",
                            request.Sira.YonlendirmeTipi, request.Sira.SiraId);
                    }
                }

                _logger.LogInformation("📤 SiraRedirected panel broadcast edildi. SiraId: {SiraId}, Kaynak: {SourceBanko}, Hedef: {TargetBanko}",
                    request.Sira.SiraId, request.SourceBankoId, request.TargetBankoId.HasValue ? request.TargetBankoId.Value.ToString() : "Yok");

                // ⭐ TV'lere bildirim gönder - Yönlendirilen sıra TV listesinden kalkacak (overlay yok)
                await BroadcastSiraRedirectedToTvAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ SiraRedirected broadcast hatası. SiraId: {SiraId}", request.Sira.SiraId);
            }
        }

        /// <summary>
        /// Sıra yönlendirildiğinde TV'lere bildirim gönderir - Overlay göstermeden sadece liste güncellenir
        /// ⭐ Request/Command Pattern
        /// </summary>
        private async Task BroadcastSiraRedirectedToTvAsync(BroadcastSiraRedirectedRequest request)
        {
            try
            {
                // Kaynak banko bilgilerini al
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var banko = await bankoRepo.GetByIdAsync(request.SourceBankoId);

                if (banko == null)
                {
                    _logger.LogWarning("⚠️ TV redirect broadcast: Kaynak banko bulunamadı. BankoId: {BankoId}", request.SourceBankoId);
                    return;
                }

                // Bu bankoya bağlı TV'leri bul (TvBanko tablosundan)
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvBankolar = await tvRepo.GetTvBankolarByBankoIdAsync(request.SourceBankoId);

                if (tvBankolar == null || !tvBankolar.Any())
                {
                    _logger.LogDebug("ℹ️ Banko#{BankoId} için bağlı TV bulunamadı (redirect)", request.SourceBankoId);
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
                        _logger.LogDebug("ℹ️ TV#{TvId} için aktif bağlantı bulunamadı (redirect)", tvBanko.TvId);
                        continue;
                    }

                    // Bu TV'ye bağlı tüm bankoların ID'lerini al
                    var tvninBankolari = await tvRepo.GetTvBankolarAsync(tvBanko.TvId);
                    var bankoIds = tvninBankolari.Select(tb => tb.BankoId).ToList();

                    // Tüm bankolardaki güncel sıraları al (yönlendirilen sıra artık listede olmayacak)
                    var aktifHareketler = await bankoHareketRepo.GetAktifSiralarByBankoIdsAsync(bankoIds);
                    var siralar = aktifHareketler.Select(bh => new TvSiraItemDto
                    {
                        BankoId = bh.BankoId,
                        BankoNo = bh.Banko?.BankoNo ?? 0,
                        KatTipi = bh.Banko?.KatTipi.GetDisplayName() ?? "",
                        SiraNo = bh.SiraNo
                    }).ToList();

                    // ⭐ Yönlendirme için özel payload - ShowOverlay = false
                    var tvPayload = new TvSiraCalledDto
                    {
                        SiraNo = request.Sira.SiraNo,
                        BankoNo = banko.BankoNo.ToString(),
                        BankoId = request.SourceBankoId,
                        BankoTipi = banko.BankoTipi.GetDisplayName(),
                        KatTipi = banko.KatTipi.GetDisplayName(),
                        KanalAltAdi = request.Sira.KanalAltAdi,
                        UpdateType = "SiraRedirected", // ⭐ Farklı update type
                        ShowOverlay = false, // ⭐ Overlay gösterme
                        Siralar = siralar,
                        Timestamp = DateTime.Now
                    };

                    await _broadcaster.SendToConnectionsAsync(connectionIds, "TvSiraGuncellendi", tvPayload);
                    _logger.LogInformation("📺 TV#{TvId}'ye yönlendirme bildirimi gönderildi: Sıra#{SiraNo} listeden kaldırıldı, Liste: {Count} sıra, {ConnCount} bağlantı",
                        tvBanko.TvId, request.Sira.SiraNo, siralar.Count, connectionIds.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TV redirect broadcast hatası. SiraId: {SiraId}", request.Sira.SiraId);
            }
        }

        // ═══════════════════════════════════════════════════════
        // KIOSK / YENİ SIRA BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Yeni sıra alındığında (Kiosk'tan) ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastNewSiraAsync(BroadcastNewSiraRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 BroadcastNewSiraAsync başladı. SiraNo: {SiraNo}, HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                    request.Sira.SiraNo, request.HizmetBinasiId, request.KanalAltIslemId);

                // Bu KanalAltIslem'e atanmış ve banko modunda olan personelleri bul
                var affectedPersonels = await _siramatikQueryRepository.GetBankoModundakiPersonellerAsync(
                    request.HizmetBinasiId, request.KanalAltIslemId);

                _logger.LogInformation("🔍 Etkilenen personeller: {Count} kişi, TC'ler: [{TcList}]",
                    affectedPersonels.Count, string.Join(", ", affectedPersonels));

                if (affectedPersonels.Any())
                {
                    var payload = new SiraUpdatePayloadDto
                    {
                        UpdateType = SiraUpdateType.Append, // Listenin sonuna ekle
                        Sira = request.Sira,
                        Timestamp = DateTime.Now
                    };

                    await SendToPersonelsAsync(affectedPersonels, SiraListUpdate, payload);
                    _logger.LogInformation("📤 NewSira (Kiosk) broadcast edildi. SiraId: {SiraId}, SiraNo: {SiraNo}, Etkilenen: {Count} personel",
                        request.Sira.SiraId, request.Sira.SiraNo, affectedPersonels.Count);
                }
                else
                {
                    _logger.LogWarning("⚠️ BroadcastNewSiraAsync: Etkilenen personel bulunamadı! HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                        request.HizmetBinasiId, request.KanalAltIslemId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ NewSira broadcast hatası. SiraId: {SiraId}", request.Sira.SiraId);
            }
        }

        // ═══════════════════════════════════════════════════════
        // TV EKRANI BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra çağrıldığında TV ekranlarına bildirim gönder (eski yapı)
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraToTvAsync(BroadcastSiraToTvRequest request)
        {
            try
            {
                // ⭐ Profesyonel DTO yapısı
                var tvPayload = new TvSiraUpdateDto
                {
                    SiraNo = request.Sira.SiraNo,
                    BankoNo = request.BankoNo,
                    KanalAltAdi = request.Sira.KanalAltAdi,
                    Timestamp = DateTime.Now
                };

                // Hizmet binasındaki tüm TV'lere gönder
                var groupName = $"HIZMETBINASI_{request.HizmetBinasiId}";
                await _broadcaster.SendToGroupAsync(groupName, ReceiveSiraUpdate, tvPayload);

                _logger.LogInformation("📺 TV broadcast edildi. SiraNo: {SiraNo}, BankoNo: {BankoNo}, HizmetBinasi: {HizmetBinasiId}",
                    request.Sira.SiraNo, request.BankoNo, request.HizmetBinasiId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TV broadcast hatası. SiraNo: {SiraNo}", request.Sira.SiraNo);
            }
        }

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// HubTvConnection tablosu üzerinden aktif TV bağlantılarına mesaj gönderir
        /// Sıra çağırma paneli gibi tüm güncel listeyi gönderir
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastSiraCalledToTvAsync(BroadcastSiraCalledToTvRequest request)
        {
            try
            {
                // Banko bilgilerini al (katTipi, bankoNo ve bankoTipi için)
                var bankoRepo = _unitOfWork.GetRepository<IBankoRepository>();
                var banko = await bankoRepo.GetByIdAsync(request.BankoId);

                string katTipi = banko?.KatTipi.GetDisplayName() ?? "";
                string bankoTipi = banko?.BankoTipi.GetDisplayName() ?? "BANKO";
                // bankoNo parametresi boş gelebilir, veritabanından al
                string actualBankoNo = !string.IsNullOrEmpty(request.BankoNo) ? request.BankoNo : (banko?.BankoNo.ToString() ?? "");

                // Bu bankoya bağlı TV'leri bul (TvBanko tablosundan)
                var tvRepo = _unitOfWork.GetRepository<ITvRepository>();
                var tvBankolar = await tvRepo.GetTvBankolarByBankoIdAsync(request.BankoId);

                if (tvBankolar == null || !tvBankolar.Any())
                {
                    _logger.LogDebug("ℹ️ Banko#{BankoId} için bağlı TV bulunamadı", request.BankoId);
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
                        SiraNo = request.Sira.SiraNo,
                        BankoNo = actualBankoNo,
                        BankoId = request.BankoId,
                        BankoTipi = bankoTipi,
                        KatTipi = katTipi,
                        KanalAltAdi = request.Sira.KanalAltAdi,
                        UpdateType = "SiraCalled",
                        Siralar = siralar,
                        Timestamp = DateTime.Now
                    };

                    await _broadcaster.SendToConnectionsAsync(connectionIds, "TvSiraGuncellendi", tvPayload);
                    _logger.LogInformation("📺 TV#{TvId}'ye sıra bildirimi gönderildi: Sıra#{SiraNo}, Liste: {Count} sıra, {ConnCount} bağlantı",
                        tvBanko.TvId, request.Sira.SiraNo, siralar.Count, connectionIds.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ TV sıra çağırma broadcast hatası. SiraId: {SiraId}, BankoId: {BankoId}", request.Sira.SiraId, request.BankoId);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Birden fazla personele SignalR mesajı gönderir
        /// ⭐ HubBankoConnection, HubConnection ve User ile entegre
        /// Repository zaten tüm filtreleri yapıyor (BankoMode, BankoModuAktif, User.BankoModuAktif, vb.)
        /// </summary>
        private async Task SendToPersonelsAsync(List<string> personelTcs, string eventName, object payload)
        {
            if (!personelTcs.Any()) return;

            var connectionIds = new List<string>();
            foreach (var tc in personelTcs)
            {
                // Repository'den gelen bağlantılar zaten filtrelenmiş (BankoMode, aktif, banko modu vb.)
                var connections = (await _hubConnectionRepository.GetActiveConnectionsByTcKimlikNoAsync(tc)).ToList();

                if (connections.Any())
                {
                    var typesList = string.Join(", ", connections.Select(c => $"{c.ConnectionType}:{c.ConnectionId}"));
                    _logger.LogInformation("🔍 TC: {Tc} için {Count} aktif banko bağlantısı bulundu. Bağlantılar: {Types}",
                        tc, connections.Count, typesList);

                    connectionIds.AddRange(connections.Select(c => c.ConnectionId));
                }
                else
                {
                    _logger.LogDebug("ℹ️ TC: {Tc} için aktif banko bağlantısı yok (offline, banko modu kapalı veya filtrelere uymayan)", tc);
                }
            }

            var idsString = string.Join(", ", connectionIds);
            _logger.LogInformation("🔍 Toplam aktif banko bağlantısı: {Count}, IDs: {Ids}",
                connectionIds.Count, idsString);

            if (connectionIds.Any())
            {
                await _broadcaster.SendToConnectionsAsync(connectionIds, eventName, payload);
                _logger.LogInformation("📤 {EventName} gönderildi: {Count} bağlantıya", eventName, connectionIds.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ {EventName} gönderilemedi: Hiçbir personel için aktif banko bağlantısı bulunamadı! Kontrol edilecekler: BankoMode, User.BankoModuAktif, HubBankoConnection.BankoModuAktif", eventName);
            }
        }

        /// <summary>
        /// Tek bir personele SignalR mesajı gönderir
        /// ⭐ HubBankoConnection, HubConnection ve User ile entegre
        /// Repository zaten tüm filtreleri yapıyor (BankoMode, BankoModuAktif, User.BankoModuAktif, vb.)
        /// </summary>
        private async Task SendToPersonelAsync(string personelTc, string eventName, object payload)
        {
            // Repository'den gelen bağlantılar zaten filtrelenmiş (BankoMode, aktif, banko modu vb.)
            var connections = await _hubConnectionRepository.GetActiveConnectionsByTcKimlikNoAsync(personelTc);
            var connectionIds = connections
                .Select(c => c.ConnectionId)
                .ToList();

            if (connectionIds.Any())
            {
                await _broadcaster.SendToConnectionsAsync(connectionIds, eventName, payload);
                _logger.LogDebug("📤 {EventName} gönderildi: {PersonelTc}, {Count} bağlantı", eventName, personelTc, connectionIds.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ {EventName} gönderilemedi: {PersonelTc} için aktif banko bağlantısı bulunamadı!", eventName, personelTc);
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════
        // ⭐ INCREMENTAL UPDATE
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra alındığında/yönlendirildiğinde etkilenen personellere güncelleme gönderir
        /// ⭐ Request/Command Pattern
        /// </summary>
        public async Task BroadcastBankoPanelGuncellemesiAsync(BroadcastBankoPanelGuncellemesiRequest request)
        {
            try
            {
                _logger.LogInformation("🔍 BankoPanelGuncellemesi başladı. SiraId: {SiraId}", request.SiraId);

                // Sıra bilgisini al
                var siraRepo = _unitOfWork.GetRepository<ISiraRepository>();
                var sira = await siraRepo.GetByIdAsync(request.SiraId);
                if (sira == null)
                {
                    _logger.LogWarning("⚠️ SiraId: {SiraId} bulunamadı!", request.SiraId);
                    return;
                }

                _logger.LogInformation("🔍 Sıra bulundu. KanalAltIslemId: {KanalAltIslemId}, HizmetBinasiId: {HizmetBinasiId}",
                    sira.KanalAltIslemId, sira.HizmetBinasiId);

                // Repository'den tüm satırları al (PersonelTc + ConnectionId içeren)
                var rawData = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(request.SiraId);

                _logger.LogInformation("🔍 GetBankoPanelBekleyenSiralarBySiraIdAsync sonucu: {Count} satır", rawData.Count);

                if (!rawData.Any())
                {
                    _logger.LogWarning("⚠️ SiraId: {SiraId} için etkilenen personel bulunamadı! HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                        request.SiraId, sira.HizmetBinasiId, sira.KanalAltIslemId);
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
                // ⭐ siraRepo zaten yukarıda tanımlı - aktif çağrılmış sıra kontrolü için kullanılacak
                foreach (var group in personelGroups)
                {
                    // ⭐ Sadece tetikleyen sırayı bul ve pozisyonunu hesapla
                    var tetikleyenSira = group.Siralar.FirstOrDefault(s => s.SiraId == request.SiraId);
                    var pozisyon = tetikleyenSira != null ? group.Siralar.IndexOf(tetikleyenSira) : -1;

                    // ⭐ Frontend'de çağrılan sıra listenin en üstünde tutuluyor
                    // Backend listesi sadece Beklemede/Yönlendirildi sıraları içeriyor
                    // Eğer personelin aktif çağrılmış sırası varsa, pozisyonu +1 artır
                    if (pozisyon >= 0)
                    {
                        var aktifCagrilanSira = await siraRepo.GetCalledByPersonelAsync(group.PersonelTc);
                        if (aktifCagrilanSira != null)
                        {
                            pozisyon += 1; // Çağrılan sıra en üstte olduğu için +1
                            _logger.LogDebug("📍 Pozisyon düzeltildi: Personel {PersonelTc} için aktif çağrılan sıra var, yeni pozisyon: {Pozisyon}", 
                                group.PersonelTc, pozisyon);
                        }
                    }

                    // ⭐ Profesyonel DTO yapısı (Request/Command Pattern)
                    var payload = new BankoPanelSiraGuncellemesiDto
                    {
                        SiraId = request.SiraId,
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
                    request.SiraId, personelGroups.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ BankoPanelGuncellemesi hatası. SiraId: {SiraId}", request.SiraId);
            }
        }

    }
}

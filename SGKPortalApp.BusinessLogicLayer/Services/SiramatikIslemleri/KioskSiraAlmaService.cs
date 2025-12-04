using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk Sıra Alma Servisi
    /// Masaüstü kiosk uygulamasını simüle eder
    /// 
    /// Vatandaş Akışı:
    /// 1. Kiosk Menüleri → GetKioskMenulerAsync
    /// 2. Alt Kanal İşlemleri → GetKioskMenuAltIslemleriAsync  
    /// 3. Sıra Al → SiraAlAsync
    /// </summary>
    public class KioskSiraAlmaService : IKioskSiraAlmaService
    {
        private readonly ISiraRepository _siraRepository;
        private readonly IKioskMenuRepository _kioskMenuRepository;
        private readonly IKioskMenuIslemRepository _kioskMenuIslemRepository;
        private readonly IKanalAltIslemRepository _kanalAltIslemRepository;
        private readonly ISiramatikQueryRepository _siramatikQueryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISiramatikHubService _hubService;
        private readonly ILogger<KioskSiraAlmaService> _logger;

        public KioskSiraAlmaService(
            ISiraRepository siraRepository,
            IKioskMenuRepository kioskMenuRepository,
            IKioskMenuIslemRepository kioskMenuIslemRepository,
            IKanalAltIslemRepository kanalAltIslemRepository,
            ISiramatikQueryRepository siramatikQueryRepository,
            IUnitOfWork unitOfWork,
            ISiramatikHubService hubService,
            ILogger<KioskSiraAlmaService> logger)
        {
            _siraRepository = siraRepository;
            _kioskMenuRepository = kioskMenuRepository;
            _kioskMenuIslemRepository = kioskMenuIslemRepository;
            _kanalAltIslemRepository = kanalAltIslemRepository;
            _siramatikQueryRepository = siramatikQueryRepository;
            _unitOfWork = unitOfWork;
            _hubService = hubService;
            _logger = logger;
        }

        /// <summary>
        /// Kiosk'tan sıra al
        /// </summary>
        public async Task<ApiResponseDto<KioskSiraAlResponseDto>> SiraAlAsync(KioskSiraAlRequestDto request)
        {
            try
            {
                _logger.LogInformation("🎫 Kiosk sıra alma başladı. KioskMenuIslemId: {KioskMenuIslemId}, HizmetBinasiId: {HizmetBinasiId}",
                    request.KioskMenuIslemId, request.HizmetBinasiId);

                // 1. KioskMenuIslem'i bul ve KanalAltId'yi al
                var kioskMenuIslem = await _kioskMenuIslemRepository.GetWithDetailsAsync(request.KioskMenuIslemId);
                
                int kanalAltId;
                string kanalAltAdi;

                if (kioskMenuIslem != null)
                {
                    // KioskMenuIslem bulundu - normal akış
                    if (kioskMenuIslem.Aktiflik != Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                            "İşlem aktif değil",
                            "Bu işlem şu anda aktif değil.");
                    }

                    kanalAltId = kioskMenuIslem.KanalAltId;
                    kanalAltAdi = kioskMenuIslem.KanalAlt?.KanalAltAdi ?? "Bilinmiyor";
                }
                else
                {
                    // KioskMenuIslem bulunamadı - direkt KanalAltIslemId olarak dene (test/simülasyon için)
                    var direktKanalAltIslem = await _kanalAltIslemRepository.GetWithDetailsAsync(request.KioskMenuIslemId);
                    if (direktKanalAltIslem == null)
                    {
                        return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                            "Geçersiz işlem",
                            "Belirtilen işlem bulunamadı.");
                    }

                    if (direktKanalAltIslem.Aktiflik != Aktiflik.Aktif)
                    {
                        return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                            "İşlem aktif değil",
                            "Bu işlem şu anda aktif değil.");
                    }

                    // Direkt KanalAltIslem kullanılıyor
                    kanalAltId = direktKanalAltIslem.KanalAltId;
                    kanalAltAdi = direktKanalAltIslem.KanalAlt?.KanalAltAdi ?? "Bilinmiyor";
                }

                // 2. HizmetBinasi + KanalAlt kombinasyonundan KanalAltIslem'i bul
                var kanalAltIslemler = await _kanalAltIslemRepository.GetByKanalAltAsync(kanalAltId);
                var kanalAltIslem = kanalAltIslemler
                    .FirstOrDefault(kai => kai.HizmetBinasiId == request.HizmetBinasiId && kai.Aktiflik == Aktiflik.Aktif);

                if (kanalAltIslem == null)
                {
                    return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                        "İşlem bulunamadı",
                        "Bu hizmet binasında belirtilen işlem tanımlı değil.");
                }

                // 3. Bu KanalAltIslem'e atanmış ve banko modunda aktif personel var mı kontrol et
                var aktifPersonelVar = await HasAktifPersonelAsync(kanalAltIslem.KanalAltIslemId);
                if (!aktifPersonelVar)
                {
                    _logger.LogWarning("⚠️ Kiosk sıra alma: Aktif personel yok. HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                        request.HizmetBinasiId, kanalAltIslem.KanalAltIslemId);
                    
                    return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                        "Şu anda hizmet verilemiyor",
                        "Bu işlem için şu anda aktif personel bulunmamaktadır. Lütfen daha sonra tekrar deneyiniz.");
                }

                // 4. Bugün için yeni sıra numarası üret
                var yeniSiraNo = await GetNextSiraNoAsync(request.HizmetBinasiId, kanalAltIslem.KanalAltIslemId);

                // 5. Yeni Sira entity'si oluştur
                var yeniSira = new Sira
                {
                    SiraNo = yeniSiraNo,
                    KanalAltIslemId = kanalAltIslem.KanalAltIslemId,
                    KanalAltIslem = kanalAltIslem,
                    KanalAltAdi = kanalAltAdi,
                    HizmetBinasiId = request.HizmetBinasiId,
                    HizmetBinasi = kanalAltIslem.HizmetBinasi,
                    SiraAlisZamani = DateTime.Now,
                    BeklemeDurum = BeklemeDurum.Beklemede
                };

                // 6. Veritabanına kaydet
                await _siraRepository.AddAsync(yeniSira);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("✅ Kiosk sıra oluşturuldu. SiraId: {SiraId}, SiraNo: {SiraNo}, KanalAltIslemId: {KanalAltIslemId}",
                    yeniSira.SiraId, yeniSira.SiraNo, kanalAltIslem.KanalAltIslemId);

                // 7. Bekleyen sıra sayısını hesapla
                var bekleyenSiraSayisi = await GetBekleyenSiraSayisiAsync(request.HizmetBinasiId, kanalAltIslem.KanalAltIslemId);

                // 8. SignalR ile banko panellerine bildirim gönder
                var siraDto = new SiraCagirmaResponseDto
                {
                    SiraId = yeniSira.SiraId,
                    SiraNo = yeniSira.SiraNo,
                    KanalAltAdi = kanalAltAdi,
                    BeklemeDurum = BeklemeDurum.Beklemede,
                    SiraAlisZamani = yeniSira.SiraAlisZamani,
                    HizmetBinasiId = request.HizmetBinasiId,
                    HizmetBinasiAdi = kanalAltIslem.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor",
                    KanalAltIslemId = kanalAltIslem.KanalAltIslemId
                };

                _ = _hubService.BroadcastNewSiraAsync(siraDto, request.HizmetBinasiId, kanalAltIslem.KanalAltIslemId);

                // 9. Response oluştur
                var response = new KioskSiraAlResponseDto
                {
                    SiraId = yeniSira.SiraId,
                    SiraNo = yeniSira.SiraNo,
                    KanalAltAdi = kanalAltAdi,
                    HizmetBinasiId = request.HizmetBinasiId,
                    HizmetBinasiAdi = kanalAltIslem.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor",
                    KanalAltIslemId = kanalAltIslem.KanalAltIslemId,
                    SiraAlisZamani = yeniSira.SiraAlisZamani,
                    BekleyenSiraSayisi = bekleyenSiraSayisi - 1, // Kendisi hariç öndeki sayı
                    AktifPersonelVar = aktifPersonelVar,
                    TahminiBeklemeSuresi = (bekleyenSiraSayisi - 1) * 5, // Ortalama 5 dk/sıra varsayımı
                    FisMesaji = $"Sıra No: {yeniSira.SiraNo}\n{kanalAltAdi}\nTarih: {yeniSira.SiraAlisZamani:dd.MM.yyyy HH:mm}\nÖnünüzde {bekleyenSiraSayisi - 1} kişi bekliyor."
                };

                return ApiResponseDto<KioskSiraAlResponseDto>.SuccessResult(response, "Sıra başarıyla alındı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk sıra alma hatası. KioskMenuIslemId: {KioskMenuIslemId}", request.KioskMenuIslemId);
                return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                    "Sıra alınamadı",
                    "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        /// <summary>
        /// Bugün için yeni sıra numarası üret
        /// HizmetBinasi + KanalAltIslem bazında günlük sıra numarası
        /// </summary>
        private async Task<int> GetNextSiraNoAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Bugünkü en yüksek sıra numarasını bul
            var siralar = await _siraRepository.GetByKanalAltIslemAsync(kanalAltIslemId);
            var bugunSiralar = siralar.Where(s => s.HizmetBinasiId == hizmetBinasiId 
                                                && s.SiraAlisZamani >= today 
                                                && s.SiraAlisZamani < tomorrow);

            var maxSiraNo = bugunSiralar.Any() ? bugunSiralar.Max(s => s.SiraNo) : 0;
            return maxSiraNo + 1;
        }

        /// <summary>
        /// Belirli bir hizmet binası ve kanal alt işlem için bekleyen sıra sayısını döner
        /// </summary>
        public async Task<int> GetBekleyenSiraSayisiAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var siralar = await _siraRepository.GetByKanalAltIslemAsync(kanalAltIslemId);
            return siralar.Count(s => s.HizmetBinasiId == hizmetBinasiId
                                   && s.BeklemeDurum == BeklemeDurum.Beklemede
                                   && s.SiraAlisZamani >= today
                                   && s.SiraAlisZamani < tomorrow);
        }

        /// <summary>
        /// Belirli bir KanalAltIslem için banko modunda aktif personel (Yrd.Uzman+) var mı?
        /// KanalAltIslem zaten HizmetBinasiId bilgisini içerir, bu yüzden sadece kanalAltIslemId yeterli
        /// </summary>
        public async Task<bool> HasAktifPersonelAsync(int kanalAltIslemId)
        {
            // KanalAltIslem'i personelleri ile birlikte getir
            var kanalAltIslem = await _kanalAltIslemRepository.GetWithDetailsAsync(kanalAltIslemId);

            if (kanalAltIslem == null)
            {
                _logger.LogWarning("⚠️ HasAktifPersonelAsync: KanalAltIslem bulunamadı. KanalAltIslemId={KanalAltIslemId}",
                    kanalAltIslemId);
                return false;
            }

            // Aktif, yetkili (en az Yrd.Uzman) ve banko modunda olan personel var mı?
            var aktifPersonelVar = kanalAltIslem.KanalPersonelleri?.Any(kp =>
                kp.Aktiflik == Aktiflik.Aktif &&
                !kp.SilindiMi &&
                kp.Uzmanlik != PersonelUzmanlik.BilgisiYok &&
                kp.User != null &&
                kp.User.BankoModuAktif &&
                kp.User.AktifMi
            ) ?? false;

            _logger.LogDebug("🔍 HasAktifPersonelAsync: KanalAltIslemId={KanalAltIslemId}, AktifPersonelVar={AktifPersonelVar}",
                kanalAltIslemId, aktifPersonelVar);

            return aktifPersonelVar;
        }

        // ═══════════════════════════════════════════════════════
        // YENİ YAPILAR: KIOSK BAZLI İŞLEMLER (Complex Query)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Belirli bir Kiosk için menüleri detaylı olarak getirir (YENİ)
        /// Complex query kullanarak kiosk bazlı menü listesini döner
        /// </summary>
        public async Task<ApiResponseDto<List<KioskMenuDto>>> GetKioskMenulerByKioskIdAsync(int kioskId)
        {
            try
            {
                _logger.LogInformation("📋 Kiosk menüleri getiriliyor (Complex Query). KioskId: {KioskId}", kioskId);

                // Complex query ile menüleri getir
                var menuDetaylar = await _siramatikQueryRepository.GetKioskMenulerByKioskIdAsync(kioskId);

                // DTO dönüşümü
                var result = menuDetaylar.Select(m => new KioskMenuDto
                {
                    KioskMenuId = m.KioskMenuId,
                    MenuAdi = m.MenuAdi,
                    Aciklama = m.MenuAciklama,
                    MenuSira = m.MenuSiraNo,
                    AktifAltIslemSayisi = m.ToplamIslemSayisi,
                    ToplamBekleyenSiraSayisi = 0 // Complex query'de hesaplanmıyor, gerekirse ayrı sorgu
                }).ToList();

                _logger.LogInformation("✅ Kiosk menüleri getirildi. KioskId: {KioskId}, Menü sayısı: {Count}",
                    kioskId, result.Count);

                return ApiResponseDto<List<KioskMenuDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk menüleri getirme hatası (Complex Query). KioskId: {KioskId}", kioskId);
                return ApiResponseDto<List<KioskMenuDto>>.ErrorResult(
                    "Menüler getirilemedi",
                    "Beklenmeyen bir hata oluştu.");
            }
        }

        /// <summary>
        /// Belirli bir Kiosk'taki seçilen menü için alt kanal işlemlerini getirir (YENİ)
        /// Complex query kullanarak kiosk ve menü bazlı alt işlem listesini döner
        /// </summary>
        public async Task<ApiResponseDto<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriByKioskIdAsync(int kioskId, int kioskMenuId)
        {
            try
            {
                _logger.LogInformation("📋 Kiosk alt işlemleri getiriliyor (Complex Query). KioskId: {KioskId}, KioskMenuId: {KioskMenuId}",
                    kioskId, kioskMenuId);

                // Complex query ile alt işlemleri getir
                var altIslemler = await _siramatikQueryRepository.GetKioskMenuAltIslemleriByKioskIdAsync(kioskId, kioskMenuId);

                _logger.LogInformation("✅ Kiosk alt işlemleri getirildi. KioskMenuId: {KioskMenuId}, İşlem sayısı: {Count}",
                    kioskMenuId, altIslemler.Count);

                return ApiResponseDto<List<KioskAltIslemDto>>.SuccessResult(altIslemler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk alt işlemleri getirme hatası (Complex Query). KioskMenuId: {KioskMenuId}", kioskMenuId);
                return ApiResponseDto<List<KioskAltIslemDto>>.ErrorResult(
                    "Alt işlemler getirilemedi",
                    "Beklenmeyen bir hata oluştu.");
            }
        }

        // ═══════════════════════════════════════════════════════
        // ESKİ YAPILAR: HİZMET BİNASI BAZLI İŞLEMLER (Geriye Uyumluluk)
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// [ESKİ] Hizmet binasındaki kiosk menülerini listeler
        /// Sadece en az bir alt işleminde aktif personel (Yrd.Uzman+) olan menüler döner
        /// </summary>
        public async Task<ApiResponseDto<List<KioskMenuDto>>> GetKioskMenulerAsync(int hizmetBinasiId)
        {
            try
            {
                _logger.LogInformation("📋 Kiosk menüleri getiriliyor. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);

                // Tüm aktif kiosk menülerini al
                var tumMenuler = await _kioskMenuRepository.GetActiveAsync();
                var result = new List<KioskMenuDto>();

                foreach (var menu in tumMenuler.OrderBy(m => m.MenuSira))
                {
                    // Bu menüdeki alt işlemleri al
                    var menuIslemler = await _kioskMenuIslemRepository.GetByKioskMenuAsync(menu.KioskMenuId);
                    var aktifAltIslemSayisi = 0;
                    var toplamBekleyenSiraSayisi = 0;

                    _logger.LogDebug("📂 Menü: {MenuAdi} (ID:{MenuId}), Alt işlem sayısı: {Count}", 
                        menu.MenuAdi, menu.KioskMenuId, menuIslemler.Count());

                    foreach (var islem in menuIslemler.Where(i => i.Aktiflik == Aktiflik.Aktif))
                    {
                        // Bu KanalAlt için aktif personel var mı? (KanalAltId üzerinden kontrol)
                        var aktifPersonelVar = await HasAktifPersonelAsync(hizmetBinasiId, islem.KanalAltId);
                        
                        _logger.LogDebug("  📄 KanalAltId: {KanalAltId}, AktifPersonelVar: {AktifPersonelVar}", 
                            islem.KanalAltId, aktifPersonelVar);
                        
                        if (aktifPersonelVar)
                        {
                            aktifAltIslemSayisi++;
                            // Bekleyen sıra sayısı için KanalAltIslem'i bul
                            var kanalAltIslemler = await _kanalAltIslemRepository.GetByKanalAltAsync(islem.KanalAltId);
                            var kanalAltIslem = kanalAltIslemler
                                .FirstOrDefault(kai => kai.HizmetBinasiId == hizmetBinasiId && kai.Aktiflik == Aktiflik.Aktif);
                            if (kanalAltIslem != null)
                            {
                                toplamBekleyenSiraSayisi += await GetBekleyenSiraSayisiAsync(hizmetBinasiId, kanalAltIslem.KanalAltIslemId);
                            }
                        }
                    }

                    // Sadece en az bir aktif alt işlemi olan menüleri ekle
                    if (aktifAltIslemSayisi > 0)
                    {
                        result.Add(new KioskMenuDto
                        {
                            KioskMenuId = menu.KioskMenuId,
                            MenuAdi = menu.MenuAdi,
                            Aciklama = menu.Aciklama,
                            MenuSira = menu.MenuSira,
                            ToplamBekleyenSiraSayisi = toplamBekleyenSiraSayisi,
                            AktifAltIslemSayisi = aktifAltIslemSayisi
                        });
                    }
                }

                _logger.LogInformation("✅ Kiosk menüleri getirildi. HizmetBinasiId: {HizmetBinasiId}, Menü sayısı: {Count}", 
                    hizmetBinasiId, result.Count);

                return ApiResponseDto<List<KioskMenuDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk menüleri getirme hatası. HizmetBinasiId: {HizmetBinasiId}", hizmetBinasiId);
                return ApiResponseDto<List<KioskMenuDto>>.ErrorResult(
                    "Menüler getirilemedi",
                    "Beklenmeyen bir hata oluştu.");
            }
        }

        // ═══════════════════════════════════════════════════════
        // ADIM 2: ALT KANAL İŞLEMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// [ESKİ] Seçilen kiosk menüsündeki alt kanal işlemlerini listeler
        /// Sadece aktif personel (Yrd.Uzman+) olan işlemler döner
        /// </summary>
        public async Task<ApiResponseDto<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriAsync(int hizmetBinasiId, int kioskMenuId)
        {
            try
            {
                _logger.LogInformation("📋 Kiosk alt işlemleri getiriliyor. HizmetBinasiId: {HizmetBinasiId}, KioskMenuId: {KioskMenuId}", 
                    hizmetBinasiId, kioskMenuId);

                // Menüdeki alt işlemleri al
                var menuIslemler = await _kioskMenuIslemRepository.GetByKioskMenuAsync(kioskMenuId);
                var result = new List<KioskAltIslemDto>();

                foreach (var islem in menuIslemler.Where(i => i.Aktiflik == Aktiflik.Aktif).OrderBy(i => i.MenuSira))
                {
                    // Bu alt işlem için HizmetBinasi'ndaki KanalAltIslem'i bul
                    var kanalAltIslemler = await _kanalAltIslemRepository.GetByKanalAltAsync(islem.KanalAltId);
                    var kanalAltIslem = kanalAltIslemler
                        .FirstOrDefault(kai => kai.HizmetBinasiId == hizmetBinasiId && kai.Aktiflik == Aktiflik.Aktif);

                    if (kanalAltIslem != null)
                    {
                        // Bu işlem için aktif personel var mı?
                        var aktifPersonelVar = await HasAktifPersonelAsync(hizmetBinasiId, kanalAltIslem.KanalAltIslemId);
                        
                        // Sadece aktif personeli olan işlemleri ekle
                        if (aktifPersonelVar)
                        {
                            var bekleyenSayisi = await GetBekleyenSiraSayisiAsync(hizmetBinasiId, kanalAltIslem.KanalAltIslemId);

                            result.Add(new KioskAltIslemDto
                            {
                                KioskMenuIslemId = islem.KioskMenuIslemId,
                                KanalAltId = islem.KanalAltId,
                                KanalAltAdi = islem.KanalAlt?.KanalAltAdi ?? "Bilinmiyor",
                                KanalAdi = islem.KanalAlt?.Kanal?.KanalAdi ?? "Bilinmiyor",
                                MenuSira = islem.MenuSira,
                                BekleyenSiraSayisi = bekleyenSayisi,
                                AktifPersonelVar = true,
                                TahminiBeklemeSuresi = bekleyenSayisi * 5 // Ortalama 5 dk/sıra varsayımı
                            });
                        }
                    }
                }

                _logger.LogInformation("✅ Kiosk alt işlemleri getirildi. KioskMenuId: {KioskMenuId}, İşlem sayısı: {Count}", 
                    kioskMenuId, result.Count);

                return ApiResponseDto<List<KioskAltIslemDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk alt işlemleri getirme hatası. KioskMenuId: {KioskMenuId}", kioskMenuId);
                return ApiResponseDto<List<KioskAltIslemDto>>.ErrorResult(
                    "Alt işlemler getirilemedi",
                    "Beklenmeyen bir hata oluştu.");
            }
        }
    }
}

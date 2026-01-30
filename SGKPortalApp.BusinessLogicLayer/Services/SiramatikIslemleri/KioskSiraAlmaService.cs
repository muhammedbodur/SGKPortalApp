using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.Common.Helpers;
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
        /// Kiosk'tan sıra al - Eski proje mantığı ile
        /// KanalAltIslemId üzerinden tek parametre ile çalışır
        /// </summary>
        public async Task<ApiResponseDto<KioskSiraAlResponseDto>> SiraAlAsync(KioskSiraAlRequestDto request)
        {
            try
            {
                _logger.LogInformation("🎫 Kiosk sıra alma başladı. KanalAltIslemId: {KanalAltIslemId}",
                    request.KanalAltIslemId);

                // Business validation
                if (request.KanalAltIslemId <= 0)
                {
                    _logger.LogWarning("GetSiraNoAsync failed: Invalid KanalAltIslemId: {KanalAltIslemId}", request.KanalAltIslemId);
                    return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                        "Geçersiz işlem",
                        "Geçersiz KanalAltIslemId!");
                }

                _logger.LogInformation("🔍 GetSiraNoAsync çağrılıyor. KanalAltIslemId: {KanalAltIslemId}", request.KanalAltIslemId);
                
                var siraNoBilgisi = await _siramatikQueryRepository.GetSiraNoAsync(request.KanalAltIslemId);

                _logger.LogInformation("🔍 GetSiraNoAsync sonucu: {Result}, SiraNo: {SiraNo}", 
                    siraNoBilgisi != null ? "Bulundu" : "NULL", 
                    siraNoBilgisi?.SiraNo ?? 0);

                if (siraNoBilgisi == null || siraNoBilgisi.SiraNo <= 0)
                {
                    _logger.LogWarning("⚠️ Kiosk sıra alma: Sıra numarası alınamadı. KanalAltIslemId: {KanalAltIslemId}, SiraNoBilgisi: {SiraNoBilgisi}",
                        request.KanalAltIslemId, siraNoBilgisi != null ? $"SiraNo={siraNoBilgisi.SiraNo}" : "NULL");
                    
                    return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                        "Şu anda hizmet verilemiyor",
                        "Bu işlem için şu anda aktif personel bulunmamaktadır. Lütfen daha sonra tekrar deneyiniz.");
                }

                // Yeni Sira entity'si oluştur
                // NOT: Navigation property'ler null! ile bypass ediliyor - EF Core FK üzerinden ilişki kuracak
                var yeniSira = new Sira
                {
                    SiraNo = siraNoBilgisi.SiraNo,
                    KanalAltIslemId = siraNoBilgisi.KanalAltIslemId,
                    KanalAltIslem = null!,  // FK üzerinden ilişki kurulacak
                    KanalAltAdi = siraNoBilgisi.KanalAltAdi,
                    DepartmanHizmetBinasiId = siraNoBilgisi.DepartmanHizmetBinasiId,
                    DepartmanHizmetBinasi = null!,   // FK üzerinden ilişki kurulacak
                    SiraAlisZamani = DateTimeHelper.Now,
                    BeklemeDurum = BeklemeDurum.Beklemede
                };

                // Veritabanına kaydet
                await _siraRepository.AddAsync(yeniSira);
                var insertedRows = await _unitOfWork.SaveChangesAsync();

                if (insertedRows <= 0)
                {
                    return ApiResponseDto<KioskSiraAlResponseDto>.ErrorResult(
                        "Sıra kaydedilemedi",
                        "Sıra numarası oluşturuldu ancak kaydedilemedi.");
                }

                _logger.LogInformation("✅ Kiosk sıra oluşturuldu. SiraId: {SiraId}, SiraNo: {SiraNo}, KanalAltIslemId: {KanalAltIslemId}",
                    yeniSira.SiraId, yeniSira.SiraNo, siraNoBilgisi.KanalAltIslemId);

                // Bekleyen sıra sayısını hesapla
                var bekleyenSiraSayisi = await GetBekleyenSiraSayisiAsync(siraNoBilgisi.DepartmanHizmetBinasiId, siraNoBilgisi.KanalAltIslemId);

                // SignalR ile banko panellerine bildirim gönder
                var siraDto = new SiraCagirmaResponseDto
                {
                    SiraId = yeniSira.SiraId,
                    SiraNo = yeniSira.SiraNo,
                    KanalAltAdi = siraNoBilgisi.KanalAltAdi,
                    BeklemeDurum = BeklemeDurum.Beklemede,
                    SiraAlisZamani = yeniSira.SiraAlisZamani,
                    HizmetBinasiId = siraNoBilgisi.HizmetBinasiId,
                    HizmetBinasiAdi = siraNoBilgisi.HizmetBinasiAdi,
                    KanalAltIslemId = siraNoBilgisi.KanalAltIslemId
                };

                _logger.LogInformation("📤 SignalR broadcast başlatılıyor. SiraNo: {SiraNo}, HizmetBinasiId: {HizmetBinasiId}, KanalAltIslemId: {KanalAltIslemId}",
                    yeniSira.SiraNo, siraNoBilgisi.HizmetBinasiId, siraNoBilgisi.KanalAltIslemId);

                // ⭐ INCREMENTAL UPDATE: Etkilenen personellere güncel listeyi gönder
                // ⭐ Request/Command Pattern
                await _hubService.BroadcastBankoPanelGuncellemesiAsync(new BroadcastBankoPanelGuncellemesiRequest
                {
                    SiraId = yeniSira.SiraId
                });

                // Response oluştur
                var response = new KioskSiraAlResponseDto
                {
                    SiraId = yeniSira.SiraId,
                    SiraNo = yeniSira.SiraNo,
                    KanalAltAdi = siraNoBilgisi.KanalAltAdi,
                    HizmetBinasiId = siraNoBilgisi.HizmetBinasiId,
                    HizmetBinasiAdi = siraNoBilgisi.HizmetBinasiAdi,
                    KanalAltIslemId = siraNoBilgisi.KanalAltIslemId,
                    SiraAlisZamani = yeniSira.SiraAlisZamani,
                    BekleyenSiraSayisi = bekleyenSiraSayisi - 1, // Kendisi hariç öndeki sayı
                    AktifPersonelVar = true,
                    TahminiBeklemeSuresi = (bekleyenSiraSayisi - 1) * 5, // Ortalama 5 dk/sıra varsayımı
                    FisMesaji = $"Sıra No: {yeniSira.SiraNo}\n{siraNoBilgisi.KanalAltAdi}\nTarih: {yeniSira.SiraAlisZamani:dd.MM.yyyy HH:mm}\nÖnünüzde {bekleyenSiraSayisi - 1} kişi bekliyor."
                };

                return ApiResponseDto<KioskSiraAlResponseDto>.SuccessResult(response, "Sıra başarıyla alındı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Kiosk sıra alma hatası. KanalAltIslemId: {KanalAltIslemId}", request.KanalAltIslemId);
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
            var bugunSiralar = siralar.Where(s => s.DepartmanHizmetBinasiId == hizmetBinasiId 
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
            return siralar.Count(s => s.DepartmanHizmetBinasiId == hizmetBinasiId
                                   && s.BeklemeDurum == BeklemeDurum.Beklemede
                                   && s.SiraAlisZamani >= today
                                   && s.SiraAlisZamani < tomorrow);
        }

        /// <summary>
        /// Belirli bir hizmet binası ve KanalAltIslem için banko modunda aktif personel (Yrd.Uzman+) var mı?
        /// NOT: kanalAltIslemId parametresi KanalAltIslem tablosundaki ID'dir!
        /// </summary>
        public async Task<bool> HasAktifPersonelAsync(int hizmetBinasiId, int kanalAltIslemId)
        {
            var aktifPersoneller = await _siramatikQueryRepository.GetBankoModundakiYetkiliPersonellerAsync(hizmetBinasiId, kanalAltIslemId);
            
            _logger.LogDebug("🔍 HasAktifPersonelAsync: HizmetBinasiId={HizmetBinasiId}, KanalAltIslemId={KanalAltIslemId}, AktifPersonelSayisi={Count}, Personeller={Personeller}",
                hizmetBinasiId, kanalAltIslemId, aktifPersoneller.Count, string.Join(",", aktifPersoneller));
            
            return aktifPersoneller.Any();
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
                                .FirstOrDefault(kai => kai.DepartmanHizmetBinasiId == hizmetBinasiId && kai.Aktiflik == Aktiflik.Aktif);
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
                        .FirstOrDefault(kai => kai.DepartmanHizmetBinasiId == hizmetBinasiId && kai.Aktiflik == Aktiflik.Aktif);

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

        /// <summary>
        /// [DEBUG] Sıra numarası bilgisini test et
        /// </summary>
        public async Task<object> TestGetSiraNoAsync(int kanalAltIslemId)
        {
            var siraNoBilgisi = await _siramatikQueryRepository.GetSiraNoAsync(kanalAltIslemId);
            
            return new
            {
                KanalAltIslemId = kanalAltIslemId,
                Sonuc = siraNoBilgisi != null ? "Bulundu" : "NULL",
                SiraNoBilgisi = siraNoBilgisi
            };
        }
    }
}

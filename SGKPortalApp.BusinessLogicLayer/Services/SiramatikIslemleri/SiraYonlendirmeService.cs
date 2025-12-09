using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Yönlendirme Servisi - Business Logic
    /// </summary>
    public class SiraYonlendirmeService : ISiraYonlendirmeService
    {
        private readonly ISiraRepository _siraRepository;
        private readonly IBankoRepository _bankoRepository;
        private readonly IKanalPersonelRepository _kanalPersonelRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBankoKullaniciRepository _bankoKullaniciRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISiramatikHubService _hubService;
        private readonly ILogger<SiraYonlendirmeService> _logger;

        public SiraYonlendirmeService(
            ISiraRepository siraRepository,
            IBankoRepository bankoRepository,
            IKanalPersonelRepository kanalPersonelRepository,
            IUserRepository userRepository,
            IBankoKullaniciRepository bankoKullaniciRepository,
            IUnitOfWork unitOfWork,
            ISiramatikHubService hubService,
            ILogger<SiraYonlendirmeService> logger)
        {
            _siraRepository = siraRepository;
            _bankoRepository = bankoRepository;
            _kanalPersonelRepository = kanalPersonelRepository;
            _userRepository = userRepository;
            _bankoKullaniciRepository = bankoKullaniciRepository;
            _unitOfWork = unitOfWork;
            _hubService = hubService;
            _logger = logger;
        }

        public async Task<ApiResponseDto<bool>> YonlendirSiraAsync(SiraYonlendirmeDto request)
        {
            // Broadcast için gerekli bilgiler
            SiraCagirmaResponseDto? siraDto = null;
            int? hedefBankoIdForBroadcast = null;

            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                try
                {
                    // ═══════════════════════════════════════════════════════
                    // TEMEL VALIDASYONLAR
                    // ═══════════════════════════════════════════════════════
                    if (request.SiraId <= 0)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Geçersiz sıra ID");
                    }

                    if (string.IsNullOrWhiteSpace(request.YonlendirenPersonelTc) || request.YonlendirenPersonelTc.Length != 11)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Geçersiz personel TC Kimlik No");
                    }

                    // Sırayı getir
                    var sira = await _siraRepository.GetSiraForYonlendirmeAsync(request.SiraId);
                    if (sira == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Sıra bulunamadı");
                    }

                    // Sıra sadece Cagrildi durumundayken yönlendirilebilir
                    if (sira.BeklemeDurum != BeklemeDurum.Cagrildi)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Sadece çağrılmış sıralar yönlendirilebilir");
                    }

                    // Kaynak banko kontrolü
                    var kaynakBanko = await _bankoRepository.GetByIdAsync(request.YonlendirenBankoId);
                    if (kaynakBanko == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Kaynak banko bulunamadı");
                    }

                    // Yönlendiren personel bu KanalAltIslem'de kayıtlı mı kontrol et
                    var yonlendirenKanalPersoneller = await _kanalPersonelRepository.GetByPersonelAsync(request.YonlendirenPersonelTc);
                    var yonlendirenKayit = yonlendirenKanalPersoneller.FirstOrDefault(kp =>
                        kp.KanalAltIslemId == sira.KanalAltIslemId &&
                        kp.Aktiflik == Aktiflik.Aktif &&
                        !kp.SilindiMi);

                    if (yonlendirenKayit == null)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Bu işlem için yetkiniz yok (Kanal personeli ataması bulunamadı)");
                    }

                    // ═══════════════════════════════════════════════════════
                    // YÖNLENDİRME TİPİNE GÖRE VALIDASYONLAR
                    // ═══════════════════════════════════════════════════════
                    int? hedefBankoId = null;

                    switch (request.YonlendirmeTipi)
                    {
                        case YonlendirmeTipi.BaskaBanko:
                            var baskaBankoId = await ValidateBaskaBankoYonlendirme(request);
                            if (baskaBankoId <= 0)
                            {
                                return ApiResponseDto<bool>.ErrorResult("Hedef banko geçerli değil");
                            }
                            hedefBankoId = baskaBankoId;
                            break;

                        case YonlendirmeTipi.Sef:
                            var sefResult = await ValidateSefYonlendirme(sira.KanalAltIslemId);
                            if (sefResult <= 0)
                            {
                                return ApiResponseDto<bool>.ErrorResult("Şu anda aktif şef personeli bulunmuyor");
                            }
                            hedefBankoId = null; // Şef yönlendirmelerinde hedef banko bilgisi saklanmaz
                            break;

                        case YonlendirmeTipi.UzmanPersonel:
                            var uzmanResult = await ValidateUzmanYonlendirme(sira.KanalAltIslemId);
                            if (uzmanResult <= 0)
                            {
                                return ApiResponseDto<bool>.ErrorResult("Şu anda aktif uzman personel bulunmuyor");
                            }
                            hedefBankoId = null; // Uzman yönlendirmelerinde hedef banko bilgisi saklanmaz
                            break;

                        default:
                            return ApiResponseDto<bool>.ErrorResult("Geçersiz yönlendirme tipi");
                    }

                    // ═══════════════════════════════════════════════════════
                    // YÖNLENDİRME İŞLEMİ
                    // ═══════════════════════════════════════════════════════
                    var yonlendirmeResult = await _siraRepository.YonlendirSiraAsync(
                        request.SiraId,
                        request.YonlendirenBankoId,
                        hedefBankoId,
                        request.YonlendirenPersonelTc,
                        request.YonlendirmeTipi,
                        request.YonlendirmeNedeni
                    );

                    if (!yonlendirmeResult)
                    {
                        return ApiResponseDto<bool>.ErrorResult("Yönlendirme işlemi başarısız oldu");
                    }

                    // ⭐ Yönlendiren bankonun BankoHareket kaydını tamamla
                    var yonlendirmeBitisZamani = DateTime.Now;
                    var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();
                    var mevcutHareketler = await bankoHareketRepo.GetBySiraForUpdateAsync(request.SiraId);
                    var aktifHareket = mevcutHareketler.FirstOrDefault(bh => 
                        bh.BankoId == request.YonlendirenBankoId && bh.IslemBitisZamani == null);
                    
                    if (aktifHareket != null)
                    {
                        aktifHareket.IslemBitisZamani = yonlendirmeBitisZamani;
                        aktifHareket.IslemSuresiSaniye = (int)(yonlendirmeBitisZamani - aktifHareket.IslemBaslamaZamani).TotalSeconds;
                        bankoHareketRepo.Update(aktifHareket);
                        _logger.LogInformation("📝 BankoHareket yönlendirme ile tamamlandı. SiraId: {SiraId}, BankoId: {BankoId}, Süre: {Sure}sn", 
                            request.SiraId, request.YonlendirenBankoId, aktifHareket.IslemSuresiSaniye);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "Sıra yönlendirildi. SiraId: {SiraId}, SiraNo: {SiraNo}, Kaynak: {KaynakBankoId}, Hedef: {HedefBankoId}, Tip: {YonlendirmeTipi}",
                        sira.SiraId, sira.SiraNo, request.YonlendirenBankoId, hedefBankoId.HasValue ? hedefBankoId.Value.ToString() : "Yok", request.YonlendirmeTipi);

                    // Broadcast için bilgileri kaydet
                    siraDto = new SiraCagirmaResponseDto
                    {
                        SiraId = sira.SiraId,
                        SiraNo = sira.SiraNo,
                        KanalAltIslemId = sira.KanalAltIslemId,
                        KanalAltAdi = sira.KanalAltAdi,
                        HizmetBinasiId = sira.HizmetBinasiId,
                        SiraAlisZamani = sira.SiraAlisZamani,
                        BeklemeDurum = BeklemeDurum.Yonlendirildi,
                        YonlendirildiMi = true,
                        YonlendirmeTipi = request.YonlendirmeTipi
                    };
                    hedefBankoIdForBroadcast = hedefBankoId;

                    return ApiResponseDto<bool>.SuccessResult(true, "Sıra başarıyla yönlendirildi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sıra yönlendirilirken hata oluştu. SiraId: {SiraId}", request.SiraId);
                    return ApiResponseDto<bool>.ErrorResult("Sıra yönlendirilirken bir hata oluştu", ex.Message);
                }
            });

            // Transaction tamamlandıktan sonra SignalR broadcast yap
            // ⭐ Request/Command Pattern
            if (result.Success && siraDto != null)
            {
                // 1️⃣ Personel panellerine yönlendirme bildirimi gönder
                await _hubService.BroadcastSiraRedirectedAsync(new BroadcastSiraRedirectedRequest
                {
                    Sira = siraDto,
                    SourceBankoId = request.YonlendirenBankoId,
                    TargetBankoId = hedefBankoIdForBroadcast,
                    SourcePersonelTc = request.YonlendirenPersonelTc
                });

                // 2️⃣ Kaynak bankonun TV'lerine güncellenmiş sıra listesi gönder
                // (Yönlendirilen sıra TV'den kalkacak çünkü BankoHareket tamamlanmış)
                await _hubService.BroadcastSiraCalledToTvAsync(new BroadcastSiraCalledToTvRequest
                {
                    Sira = siraDto,
                    BankoId = request.YonlendirenBankoId,
                    BankoNo = string.Empty // GetByIdAsync ile bulunacak
                });
            }

            return result;
        }

        /// <summary>
        /// Başka bankoya yönlendirme validasyonu
        /// </summary>
        private async Task<int> ValidateBaskaBankoYonlendirme(SiraYonlendirmeDto request)
        {
            try
            {
                if (!request.HedefBankoId.HasValue || request.HedefBankoId.Value <= 0)
                {
                    _logger.LogWarning("Hedef banko ID boş veya geçersiz. HedefBankoId: {HedefBankoId}", request.HedefBankoId);
                    return -1;
                }

                // Hedef banko kontrolü
                var hedefBanko = await _bankoRepository.GetByIdAsync(request.HedefBankoId.Value);
                if (hedefBanko == null)
                {
                    _logger.LogWarning("Hedef banko bulunamadı. HedefBankoId: {HedefBankoId}", request.HedefBankoId);
                    return -1;
                }

                // Aynı bankoya yönlendirme kontrolü
                if (request.YonlendirenBankoId == request.HedefBankoId.Value)
                {
                    _logger.LogWarning("Aynı bankoya yönlendirme denemesi. BankoId: {BankoId}", request.HedefBankoId);
                    return -1;
                }

                // Hedef bankonun personeli var mı ve banko modunda mı?
                var hedefBankoKullanici = await _bankoKullaniciRepository.GetByBankoAsync(request.HedefBankoId.Value);
                if (hedefBankoKullanici == null)
                {
                    _logger.LogWarning("Hedef bankoda personel atanmamış. HedefBankoId: {HedefBankoId}", request.HedefBankoId);
                    return -1;
                }

                // Hedef banko personeli aktif mi? (BankoModuAktif=1 ve AktifBankoId>0)
                var hedefUser = await _userRepository.GetByTcKimlikNoAsync(hedefBankoKullanici.TcKimlikNo);
                if (hedefUser == null || !hedefUser.BankoModuAktif || hedefUser.AktifBankoId != request.HedefBankoId.Value)
                {
                    _logger.LogWarning(
                        "Hedef banko personeli aktif değil. PersonelTc: {TcKimlikNo}, BankoModuAktif: {BankoModuAktif}, AktifBankoId: {AktifBankoId}",
                        hedefBankoKullanici.TcKimlikNo, hedefUser?.BankoModuAktif, hedefUser?.AktifBankoId);
                    return -1;
                }

                return request.HedefBankoId.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Başka banko validasyon hatası");
                return -1;
            }
        }

        /// <summary>
        /// Şef yönlendirme validasyonu - İlk aktif şef personelin bankosunu döner
        /// </summary>
        private async Task<int> ValidateSefYonlendirme(int kanalAltIslemId)
        {
            try
            {
                // Bu KanalAltIslem için Şef uzmanlığına sahip personelleri getir
                var kanalPersoneller = await _kanalPersonelRepository.GetByKanalAltIslemAsync(kanalAltIslemId);
                var sefPersoneller = kanalPersoneller
                    .Where(kp => kp.Uzmanlik == PersonelUzmanlik.Sef &&
                                 kp.Aktiflik == Aktiflik.Aktif &&
                                 !kp.SilindiMi)
                    .ToList();

                if (!sefPersoneller.Any())
                {
                    _logger.LogWarning("KanalAltIslemId {KanalAltIslemId} için şef personel bulunamadı", kanalAltIslemId);
                    return -1;
                }

                // Aktif (Banko modunda) olan şef personelleri filtrele
                foreach (var sefPersonel in sefPersoneller)
                {
                    var user = await _userRepository.GetByTcKimlikNoAsync(sefPersonel.TcKimlikNo);
                    if (user != null && user.BankoModuAktif && user.AktifBankoId.HasValue && user.AktifBankoId.Value > 0)
                    {
                        _logger.LogInformation(
                            "Şef personel bulundu. TcKimlikNo: {TcKimlikNo}, AktifBankoId: {AktifBankoId}",
                            user.TcKimlikNo, user.AktifBankoId.Value);

                        return user.AktifBankoId.Value;
                    }
                }

                _logger.LogWarning("KanalAltIslemId {KanalAltIslemId} için aktif şef personel bulunamadı", kanalAltIslemId);
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şef validasyon hatası. KanalAltIslemId: {KanalAltIslemId}", kanalAltIslemId);
                return -1;
            }
        }

        /// <summary>
        /// Uzman personel yönlendirme validasyonu - İlk aktif uzman personelin bankosunu döner
        /// </summary>
        private async Task<int> ValidateUzmanYonlendirme(int kanalAltIslemId)
        {
            try
            {
                // Bu KanalAltIslem için Uzman seviyesine sahip personelleri getir
                var kanalPersoneller = await _kanalPersonelRepository.GetByKanalAltIslemAsync(kanalAltIslemId);
                var uzmanPersoneller = kanalPersoneller
                    .Where(kp => kp.Uzmanlik == PersonelUzmanlik.Uzman &&
                                 kp.Aktiflik == Aktiflik.Aktif &&
                                 !kp.SilindiMi)
                    .ToList();

                if (!uzmanPersoneller.Any())
                {
                    _logger.LogWarning("KanalAltIslemId {KanalAltIslemId} için uzman personel bulunamadı", kanalAltIslemId);
                    return -1;
                }

                // Aktif (Banko modunda) olan uzman personelleri filtrele
                foreach (var uzmanPersonel in uzmanPersoneller)
                {
                    var user = await _userRepository.GetByTcKimlikNoAsync(uzmanPersonel.TcKimlikNo);
                    if (user != null && user.BankoModuAktif && user.AktifBankoId.HasValue && user.AktifBankoId.Value > 0)
                    {
                        _logger.LogInformation(
                            "Uzman personel bulundu. TcKimlikNo: {TcKimlikNo}, AktifBankoId: {AktifBankoId}",
                            user.TcKimlikNo, user.AktifBankoId.Value);

                        return user.AktifBankoId.Value;
                    }
                }

                _logger.LogWarning("KanalAltIslemId {KanalAltIslemId} için aktif uzman personel bulunamadı", kanalAltIslemId);
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uzman validasyon hatası. KanalAltIslemId: {KanalAltIslemId}", kanalAltIslemId);
                return -1;
            }
        }

        public async Task<ApiResponseDto<int>> GetYonlendirilmisSiraCountAsync(int bankoId)
        {
            try
            {
                if (bankoId <= 0)
                {
                    return ApiResponseDto<int>.ErrorResult("Geçersiz banko ID");
                }

                var banko = await _bankoRepository.GetByIdAsync(bankoId);
                if (banko == null)
                {
                    return ApiResponseDto<int>.ErrorResult("Banko bulunamadı");
                }

                // Hedef banko olarak bu bankoya yönlendirilmiş sıraları say
                var siralar = await _siraRepository.GetAllWithDetailsAsync();
                var count = siralar.Count(s =>
                    s.HedefBankoId == bankoId &&
                    s.BeklemeDurum == BeklemeDurum.Yonlendirildi &&
                    !s.SilindiMi);

                return ApiResponseDto<int>.SuccessResult(count, $"{count} adet yönlendirilmiş sıra bulundu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yönlendirilmiş sıra sayısı getirilirken hata oluştu. BankoId: {BankoId}", bankoId);
                return ApiResponseDto<int>.ErrorResult("Yönlendirilmiş sıra sayısı getirilirken bir hata oluştu", ex.Message);
            }
        }

        public async Task<ApiResponseDto<YonlendirmeSecenekleriResponseDto>> GetYonlendirmeSecenekleriAsync(int siraId, int kaynakBankoId)
        {
            try
            {
                // Sırayı getir
                var sira = await _siraRepository.GetSiraForYonlendirmeAsync(siraId);
                if (sira == null)
                {
                    return ApiResponseDto<YonlendirmeSecenekleriResponseDto>.ErrorResult("Sıra bulunamadı");
                }

                var result = new YonlendirmeSecenekleriResponseDto();

                // ⭐ Yönlendiren personelin TC'sini bul (kaynak bankodan)
                var kaynakBankoKullanici = await _bankoKullaniciRepository.GetByBankoAsync(kaynakBankoId);
                var yonlendirenPersonelTc = kaynakBankoKullanici?.TcKimlikNo;

                // ═══════════════════════════════════════════════════════
                // ŞEF KONTROLÜ (kendisi hariç)
                // ═══════════════════════════════════════════════════════
                var kanalPersoneller = await _kanalPersonelRepository.GetByKanalAltIslemAsync(sira.KanalAltIslemId);
                var sefPersoneller = kanalPersoneller
                    .Where(kp => kp.Uzmanlik == PersonelUzmanlik.Sef &&
                                 kp.Aktiflik == Aktiflik.Aktif &&
                                 !kp.SilindiMi &&
                                 kp.TcKimlikNo != yonlendirenPersonelTc) // ⭐ Kendisi hariç
                    .ToList();

                int aktifSefCount = 0;
                foreach (var sefPersonel in sefPersoneller)
                {
                    var user = await _userRepository.GetByTcKimlikNoAsync(sefPersonel.TcKimlikNo);
                    if (user != null && user.BankoModuAktif && user.AktifBankoId.HasValue && user.AktifBankoId.Value > 0)
                    {
                        aktifSefCount++;
                    }
                }

                if (aktifSefCount > 0)
                {
                    result.AvailableTypes.Add(YonlendirmeTipi.Sef);
                }
                result.SefPersonelCount = aktifSefCount;

                // ═══════════════════════════════════════════════════════
                // UZMAN KONTROLÜ (kendisi hariç)
                // ═══════════════════════════════════════════════════════
                var uzmanPersoneller = kanalPersoneller
                    .Where(kp => kp.Uzmanlik == PersonelUzmanlik.Uzman &&
                                 kp.Aktiflik == Aktiflik.Aktif &&
                                 !kp.SilindiMi &&
                                 kp.TcKimlikNo != yonlendirenPersonelTc) // ⭐ Kendisi hariç
                    .ToList();

                int aktifUzmanCount = 0;
                foreach (var uzmanPersonel in uzmanPersoneller)
                {
                    var user = await _userRepository.GetByTcKimlikNoAsync(uzmanPersonel.TcKimlikNo);
                    if (user != null && user.BankoModuAktif && user.AktifBankoId.HasValue && user.AktifBankoId.Value > 0)
                    {
                        aktifUzmanCount++;
                    }
                }

                if (aktifUzmanCount > 0)
                {
                    result.AvailableTypes.Add(YonlendirmeTipi.UzmanPersonel);
                }
                result.UzmanPersonelCount = aktifUzmanCount;

                // ═══════════════════════════════════════════════════════
                // AKTİF BANKOLAR (BaskaBanko için - sadece Uzman personeller)
                // ═══════════════════════════════════════════════════════
                var tumBankolar = await _bankoRepository.GetAllAsync();
                var aktifBankolar = new List<BankoOptionDto>();

                foreach (var banko in tumBankolar.Where(b => !b.SilindiMi && b.BankoAktiflik == Aktiflik.Aktif))
                {
                    // Kaynak banko hariç (aynı bankoya yönlendirme yapılmaz)
                    if (banko.BankoId == kaynakBankoId)
                        continue;

                    // Bankonun personeli var mı?
                    var bankoKullanici = await _bankoKullaniciRepository.GetByBankoAsync(banko.BankoId);
                    if (bankoKullanici == null)
                        continue;

                    // Personel aktif mi?
                    var user = await _userRepository.GetByTcKimlikNoAsync(bankoKullanici.TcKimlikNo);
                    if (user == null || !user.BankoModuAktif || user.AktifBankoId != banko.BankoId)
                        continue;

                    // ⭐ Personelin uzmanlığı Uzman mı? (Şef'ler BaskaBanko listesinde görünmemeli)
                    var personelKanalAtama = kanalPersoneller.FirstOrDefault(kp => kp.TcKimlikNo == bankoKullanici.TcKimlikNo);
                    if (personelKanalAtama == null || personelKanalAtama.Uzmanlik != PersonelUzmanlik.Uzman)
                        continue;

                    // Aktif banko bulundu (Uzman personel)
                    aktifBankolar.Add(new BankoOptionDto
                    {
                        BankoId = banko.BankoId,
                        BankoNo = banko.BankoNo,
                        PersonelAdi = user.Personel?.AdSoyad ?? string.Empty,
                        PersonelTc = bankoKullanici.TcKimlikNo,
                        KatAdi = banko.KatTipi.GetDisplayName()
                    });
                }

                result.Bankolar = aktifBankolar;

                // En az 1 aktif banko varsa BaskaBanko tipini ekle
                if (aktifBankolar.Any())
                {
                    result.AvailableTypes.Add(YonlendirmeTipi.BaskaBanko);
                }

                _logger.LogInformation(
                    "Yönlendirme seçenekleri hazırlandı. SiraId: {SiraId}, AvailableTypes: {Count}, Bankolar: {BankoCount}, Şef: {SefCount}, Uzman: {UzmanCount}",
                    siraId, result.AvailableTypes.Count, result.Bankolar.Count, aktifSefCount, aktifUzmanCount);

                return ApiResponseDto<YonlendirmeSecenekleriResponseDto>.SuccessResult(
                    result,
                    $"{result.AvailableTypes.Count} yönlendirme seçeneği mevcut");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yönlendirme seçenekleri getirilirken hata oluştu. SiraId: {SiraId}", siraId);
                return ApiResponseDto<YonlendirmeSecenekleriResponseDto>.ErrorResult(
                    "Yönlendirme seçenekleri getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}

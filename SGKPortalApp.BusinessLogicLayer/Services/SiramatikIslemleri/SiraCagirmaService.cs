using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;
using System.Linq;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Çağırma Servisi - Business Logic
    /// SignalR broadcast işlemleri bu katmanda yapılır (Layered Architecture)
    /// </summary>
    public class SiraCagirmaService : ISiraCagirmaService
    {
        private readonly ISiraRepository _siraRepository;
        private readonly ISiramatikQueryRepository _siramatikQueryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISiramatikHubService _hubService;
        private readonly ILogger<SiraCagirmaService> _logger;

        public SiraCagirmaService(
            ISiraRepository siraRepository,
            ISiramatikQueryRepository siramatikQueryRepository,
            IUnitOfWork unitOfWork,
            ISiramatikHubService hubService,
            ILogger<SiraCagirmaService> logger)
        {
            _siraRepository = siraRepository;
            _siramatikQueryRepository = siramatikQueryRepository;
            _unitOfWork = unitOfWork;
            _hubService = hubService;
            _logger = logger;
        }

        public async Task<List<SiraCagirmaResponseDto>> GetBekleyenSiralarAsync()
        {
            var siralar = await _siraRepository.GetWaitingAsync();

            return siralar.Select(s => new SiraCagirmaResponseDto
            {
                SiraId = s.SiraId,
                SiraNo = s.SiraNo,
                KanalAltAdi = s.KanalAltAdi,
                BeklemeDurum = s.BeklemeDurum,
                SiraAlisZamani = s.SiraAlisZamani,
                IslemBaslamaZamani = s.IslemBaslamaZamani,
                PersonelAdSoyad = s.Personel?.AdSoyad,
                HizmetBinasiId = s.HizmetBinasiId,
                HizmetBinasiAdi = s.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor"
            }).ToList();
        }

        public async Task<List<SiraCagirmaResponseDto>> GetPersonelBekleyenSiralarAsync(string tcKimlikNo)
        {
            var siralar = await _siraRepository.GetByPersonelAsync(tcKimlikNo);
            var bekleyenSiralar = siralar.Where(s => s.BeklemeDurum == BeklemeDurum.Beklemede);

            return bekleyenSiralar.Select(s => new SiraCagirmaResponseDto
            {
                SiraId = s.SiraId,
                SiraNo = s.SiraNo,
                KanalAltAdi = s.KanalAltAdi,
                BeklemeDurum = s.BeklemeDurum,
                SiraAlisZamani = s.SiraAlisZamani,
                IslemBaslamaZamani = s.IslemBaslamaZamani,
                PersonelAdSoyad = s.Personel?.AdSoyad,
                HizmetBinasiId = s.HizmetBinasiId,
                HizmetBinasiAdi = s.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor"
            }).ToList();
        }

        public async Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo, int? bankoId = null, string? bankoNo = null, int? firstCallableSiraId = null)
        {
            SiraCagirmaResponseDto? result = null;

            // ⭐ Transaction içinde atomik işlem - Race Condition koruması
            var transactionResult = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // İlk çağrılabilir sıra kontrolü
                if (firstCallableSiraId.HasValue)
                {
                    var actualFirstCallableId = await GetFirstCallableSiraIdAsync(personelTcKimlikNo);

                    if (!actualFirstCallableId.HasValue || actualFirstCallableId.Value != firstCallableSiraId.Value)
                    {
                        throw new InvalidOperationException("Sıra listesi güncellendi. Lütfen paneli yenileyip tekrar deneyin.");
                    }
                }

                // Önceki çağrılmış sırayı tamamla
                var onceCagrilanSira = await _siraRepository.GetCalledByPersonelAsync(personelTcKimlikNo);
                Sira? sira = null;

                if (onceCagrilanSira != null)
                {
                    if (onceCagrilanSira.SiraId == siraId)
                    {
                        // Aynı sırayı tekrar çağırıyor - zaten track edilen entity'yi kullan
                        sira = onceCagrilanSira;
                    }
                    else
                    {
                        // Farklı sıra - öncekini tamamla
                        var oncekiBitisZamani = DateTime.Now;
                        onceCagrilanSira.BeklemeDurum = BeklemeDurum.Bitti;
                        onceCagrilanSira.IslemBitisZamani = oncekiBitisZamani;
                        _siraRepository.Update(onceCagrilanSira);

                        // ⭐ Önceki sıranın BankoHareket kaydını güncelle
                        var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();
                        var oncekiHareketler = await bankoHareketRepo.GetBySiraForUpdateAsync(onceCagrilanSira.SiraId);
                        var oncekiAktifHareket = oncekiHareketler.FirstOrDefault(bh => bh.IslemBitisZamani == null);
                        if (oncekiAktifHareket != null)
                        {
                            oncekiAktifHareket.IslemBitisZamani = oncekiBitisZamani;
                            oncekiAktifHareket.IslemSuresiSaniye = (int)(oncekiBitisZamani - oncekiAktifHareket.IslemBaslamaZamani).TotalSeconds;
                            bankoHareketRepo.Update(oncekiAktifHareket);
                        }
                    }
                }

                // ⭐ Atomik kontrol ve güncelleme - Sıra hala çağrılabilir mi?
                // Eğer sira henüz alınmadıysa (önceki çağrılan yoksa veya farklı sıra ise) al
                if (sira == null)
                {
                    sira = await _siraRepository.GetByIdAsync(siraId);
                }
                if (sira == null)
                {
                    throw new InvalidOperationException("Sıra bulunamadı.");
                }

                // ⭐ Race Condition kontrolü: Sıra başka biri tarafından çağrılmış mı?
                // Aynı personel aynı sırayı tekrar çağırıyorsa (Cagrildi durumunda ve TcKimlikNo eşleşiyorsa) izin ver
                bool ayniPersonelTekrarCagiriyor = sira.BeklemeDurum == BeklemeDurum.Cagrildi && sira.TcKimlikNo == personelTcKimlikNo;
                
                if (!ayniPersonelTekrarCagiriyor && sira.BeklemeDurum != BeklemeDurum.Beklemede && sira.BeklemeDurum != BeklemeDurum.Yonlendirildi)
                {
                    var durum = sira.BeklemeDurum switch
                    {
                        BeklemeDurum.Cagrildi => "başka bir personel tarafından çağrıldı",
                        BeklemeDurum.Bitti => "işlemi tamamlandı",
                        _ => "artık çağrılamaz durumda"
                    };
                    throw new InvalidOperationException($"Bu sıra {durum}. Lütfen paneli yenileyip tekrar deneyin.");
                }

                // Sırayı çağır
                var islemBaslamaZamani = DateTime.Now;
                sira.BeklemeDurum = BeklemeDurum.Cagrildi;
                sira.IslemBaslamaZamani = islemBaslamaZamani;
                sira.TcKimlikNo = personelTcKimlikNo;

                _siraRepository.Update(sira);

                // ⭐ BankoHareket kaydı oluştur (TV ve raporlama için)
                // Aynı personel tekrar çağırıyorsa BankoHareket zaten var, yeni kayıt oluşturma
                if (bankoId.HasValue && bankoId.Value > 0 && !ayniPersonelTekrarCagiriyor)
                {
                    var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();
                    var kanalAltIslemRepo = _unitOfWork.GetRepository<IKanalAltIslemRepository>();
                    
                    // KanalIslemId'yi ayrı sorgu ile al (AsNoTracking ile)
                    var kanalAltIslem = await kanalAltIslemRepo.GetByIdNoTrackingAsync(sira.KanalAltIslemId);
                    int kanalIslemId = kanalAltIslem?.KanalIslemId ?? 0;
                    
                    if (kanalIslemId > 0)
                    {
                        var bankoHareket = new BankoHareket
                        {
                            BankoId = bankoId.Value,
                            PersonelTcKimlikNo = personelTcKimlikNo,
                            SiraId = sira.SiraId,
                            SiraNo = sira.SiraNo,
                            KanalIslemId = kanalIslemId,
                            KanalAltIslemId = sira.KanalAltIslemId,
                            IslemBaslamaZamani = islemBaslamaZamani,
                            IslemBitisZamani = null,
                            IslemSuresiSaniye = null
                        };
                        
                        await bankoHareketRepo.AddAsync(bankoHareket);
                        _logger.LogInformation("📝 BankoHareket kaydı oluşturuldu. BankoId: {BankoId}, SiraNo: {SiraNo}", 
                            bankoId.Value, sira.SiraNo);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // HizmetBinasi adını ayrı sorgu ile al (navigation property yok)
                string hizmetBinasiAdi = "Bilinmiyor";
                var hizmetBinasiRepo = _unitOfWork.GetRepository<IHizmetBinasiRepository>();
                var hizmetBinasi = await hizmetBinasiRepo.GetByIdAsync(sira.HizmetBinasiId);
                if (hizmetBinasi != null)
                {
                    hizmetBinasiAdi = hizmetBinasi.HizmetBinasiAdi;
                }

                result = new SiraCagirmaResponseDto
                {
                    SiraId = sira.SiraId,
                    SiraNo = sira.SiraNo,
                    KanalAltAdi = sira.KanalAltAdi,
                    BeklemeDurum = sira.BeklemeDurum,
                    SiraAlisZamani = sira.SiraAlisZamani,
                    IslemBaslamaZamani = sira.IslemBaslamaZamani,
                    PersonelAdSoyad = null, // Navigation property yok, gerekirse ayrı sorgu yapılabilir
                    HizmetBinasiId = sira.HizmetBinasiId,
                    HizmetBinasiAdi = hizmetBinasiAdi,
                    KanalAltIslemId = sira.KanalAltIslemId
                };

                _logger.LogInformation("✅ Sıra çağrıldı. SiraId: {SiraId}, SiraNo: {SiraNo}, Personel: {PersonelTc}",
                    sira.SiraId, sira.SiraNo, personelTcKimlikNo);

                return true;
            });

            if (!transactionResult || result == null)
            {
                return null;
            }

            // ═══════════════════════════════════════════════════════
            // SIGNALR BROADCAST - Transaction tamamlandıktan sonra
            // ═══════════════════════════════════════════════════════
            await _hubService.BroadcastSiraCalledAsync(result, bankoId ?? 0, bankoNo ?? "", personelTcKimlikNo);

            // ⭐ INCREMENTAL UPDATE: Etkilenen personellere güncel listeyi gönder
            await _hubService.BroadcastBankoPanelGuncellemesiAsync(siraId);

            // ⭐ TV'lere bildirim gönder
            if (bankoId.HasValue && bankoId.Value > 0)
            {
                await _hubService.BroadcastSiraCalledToTvAsync(result, bankoId.Value, bankoNo ?? "");
            }

            return result;
        }

        private async Task<int?> GetFirstCallableSiraIdAsync(string personelTcKimlikNo)
        {
            var siralar = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarAsync(personelTcKimlikNo);
            var firstCallable = siralar.FirstOrDefault(s => s.BeklemeDurum == BeklemeDurum.Yonlendirildi || s.BeklemeDurum == BeklemeDurum.Beklemede);
            return firstCallable?.SiraId;
        }

        public async Task<bool> SiraTamamlaAsync(int siraId)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null)
            {
                return false;
            }

            // Broadcast için bilgileri sakla
            var hizmetBinasiId = sira.HizmetBinasiId;
            var kanalAltIslemId = sira.KanalAltIslemId;

            var islemBitisZamani = DateTime.Now;
            sira.BeklemeDurum = BeklemeDurum.Bitti;
            sira.IslemBitisZamani = islemBitisZamani;

            _siraRepository.Update(sira);

            // ⭐ BankoHareket kaydını güncelle (işlem bitiş zamanı)
            var bankoHareketRepo = _unitOfWork.GetRepository<IBankoHareketRepository>();
            var bankoHareket = await bankoHareketRepo.GetBySiraForUpdateAsync(siraId);
            var aktifHareket = bankoHareket.FirstOrDefault(bh => bh.IslemBitisZamani == null);
            if (aktifHareket != null)
            {
                aktifHareket.IslemBitisZamani = islemBitisZamani;
                aktifHareket.IslemSuresiSaniye = (int)(islemBitisZamani - aktifHareket.IslemBaslamaZamani).TotalSeconds;
                bankoHareketRepo.Update(aktifHareket);
                _logger.LogInformation("📝 BankoHareket tamamlandı. SiraId: {SiraId}, Süre: {Sure}sn", 
                    siraId, aktifHareket.IslemSuresiSaniye);
            }

            await _unitOfWork.SaveChangesAsync();

            // SignalR broadcast - Business katmanında
            _ = _hubService.BroadcastSiraCompletedAsync(siraId, hizmetBinasiId, kanalAltIslemId);

            return true;
        }

        public async Task<bool> SiraIptalAsync(int siraId, string iptalNedeni)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null)
            {
                return false;
            }

            // Broadcast için bilgileri sakla
            var hizmetBinasiId = sira.HizmetBinasiId;
            var kanalAltIslemId = sira.KanalAltIslemId;

            // İptal için Bitti durumuna set ediyoruz (enum'da IptalEdildi yok)
            sira.BeklemeDurum = BeklemeDurum.Bitti;
            sira.IslemBitisZamani = DateTime.Now;

            _siraRepository.Update(sira);
            await _unitOfWork.SaveChangesAsync();

            // SignalR broadcast - Business katmanında
            _ = _hubService.BroadcastSiraCancelledAsync(siraId, hizmetBinasiId, kanalAltIslemId);

            return true;
        }

        public async Task<List<SiraCagirmaResponseDto>> GetBankoPanelSiralarAsync(string tcKimlikNo)
        {
            return await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarAsync(tcKimlikNo);
        }

        public async Task<SiraCagirmaResponseDto?> GetIlkCagrilabilirSiraAsync(string tcKimlikNo)
        {
            return await _siramatikQueryRepository.GetIlkCagrilabilirSiraAsync(tcKimlikNo);
        }

        public async Task<Dictionary<string, List<SiraCagirmaResponseDto>>> GetBankoPanelSiralarBySiraIdAsync(int siraId)
        {
            // Repository'den tüm satırları al (PersonelTc + ConnectionId içeren)
            var rawData = await _siramatikQueryRepository.GetBankoPanelBekleyenSiralarBySiraIdAsync(siraId);

            // PersonelTc'ye göre grupla
            var grouped = rawData
                .GroupBy(x => x.PersonelTc!)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.SiraAlisZamani).ThenBy(x => x.SiraNo).ToList()
                );

            return grouped;
        }
    }
}

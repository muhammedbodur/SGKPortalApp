using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR;
using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Complex;
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
            if (firstCallableSiraId.HasValue)
            {
                var actualFirstCallableId = await GetFirstCallableSiraIdAsync(personelTcKimlikNo);

                if (!actualFirstCallableId.HasValue || actualFirstCallableId.Value != firstCallableSiraId.Value)
                {
                    throw new InvalidOperationException("Sıra listesi güncellendi. Lütfen paneli yenileyip tekrar deneyin.");
                }
            }

            var onceCagrilanSira = await _siraRepository.GetCalledByPersonelAsync(personelTcKimlikNo);
            if (onceCagrilanSira != null && onceCagrilanSira.SiraId != siraId)
            {
                onceCagrilanSira.BeklemeDurum = BeklemeDurum.Bitti;
                onceCagrilanSira.IslemBitisZamani = DateTime.Now;
                _siraRepository.Update(onceCagrilanSira);
            }

            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null || (sira.BeklemeDurum != BeklemeDurum.Beklemede && sira.BeklemeDurum != BeklemeDurum.Yonlendirildi))
            {
                return null;
            }

            // Sırayı çağır
            sira.BeklemeDurum = BeklemeDurum.Cagrildi;
            sira.IslemBaslamaZamani = DateTime.Now;
            sira.TcKimlikNo = personelTcKimlikNo;

            _siraRepository.Update(sira);
            await _unitOfWork.SaveChangesAsync();

            var result = new SiraCagirmaResponseDto
            {
                SiraId = sira.SiraId,
                SiraNo = sira.SiraNo,
                KanalAltAdi = sira.KanalAltAdi,
                BeklemeDurum = sira.BeklemeDurum,
                SiraAlisZamani = sira.SiraAlisZamani,
                IslemBaslamaZamani = sira.IslemBaslamaZamani,
                PersonelAdSoyad = sira.Personel?.AdSoyad,
                HizmetBinasiId = sira.HizmetBinasiId,
                HizmetBinasiAdi = sira.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor",
                KanalAltIslemId = sira.KanalAltIslemId
            };

            // ═══════════════════════════════════════════════════════
            // SIGNALR BROADCAST - Business katmanında (Layered Architecture)
            // Masaüstü Kiosk, Web Kiosk, Mobil App - tüm client'lar için çalışır
            // ═══════════════════════════════════════════════════════
            _ = _hubService.BroadcastSiraCalledAsync(result, bankoId ?? 0, bankoNo ?? "", personelTcKimlikNo);

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

            sira.BeklemeDurum = BeklemeDurum.Bitti;
            sira.IslemBitisZamani = DateTime.Now;

            _siraRepository.Update(sira);
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
    }
}

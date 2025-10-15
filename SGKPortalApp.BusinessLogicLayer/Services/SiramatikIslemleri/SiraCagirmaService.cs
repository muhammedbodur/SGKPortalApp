using SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;

namespace SGKPortalApp.BusinessLogicLayer.Services.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Çağırma Servisi - Business Logic
    /// </summary>
    public class SiraCagirmaService : ISiraCagirmaService
    {
        private readonly ISiraRepository _siraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SiraCagirmaService(ISiraRepository siraRepository, IUnitOfWork unitOfWork)
        {
            _siraRepository = siraRepository;
            _unitOfWork = unitOfWork;
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

        public async Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null || sira.BeklemeDurum != BeklemeDurum.Beklemede)
            {
                return null;
            }

            // Sırayı çağır
            sira.BeklemeDurum = BeklemeDurum.Cagrildi;
            sira.IslemBaslamaZamani = DateTime.Now;
            sira.TcKimlikNo = personelTcKimlikNo;

            _siraRepository.Update(sira);
            await _unitOfWork.SaveChangesAsync();

            return new SiraCagirmaResponseDto
            {
                SiraId = sira.SiraId,
                SiraNo = sira.SiraNo,
                KanalAltAdi = sira.KanalAltAdi,
                BeklemeDurum = sira.BeklemeDurum,
                SiraAlisZamani = sira.SiraAlisZamani,
                IslemBaslamaZamani = sira.IslemBaslamaZamani,
                PersonelAdSoyad = sira.Personel?.AdSoyad,
                HizmetBinasiId = sira.HizmetBinasiId,
                HizmetBinasiAdi = sira.HizmetBinasi?.HizmetBinasiAdi ?? "Bilinmiyor"
            };
        }

        public async Task<bool> SiraTamamlaAsync(int siraId)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null)
            {
                return false;
            }

            sira.BeklemeDurum = BeklemeDurum.Bitti;
            sira.IslemBitisZamani = DateTime.Now;

            _siraRepository.Update(sira);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SiraIptalAsync(int siraId, string iptalNedeni)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null)
            {
                return false;
            }

            // İptal için Bitti durumuna set ediyoruz (enum'da IptalEdildi yok)
            sira.BeklemeDurum = BeklemeDurum.Bitti;
            sira.IslemBitisZamani = DateTime.Now;
            // Not: IptalNedeni ve IptalZamani property'leri entity'de yok

            _siraRepository.Update(sira);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}

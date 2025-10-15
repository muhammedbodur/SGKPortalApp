using SGKPortalApp.BusinessLayer.Services.Interfaces.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLayer.Services.Concrete.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Çağırma Servisi - Business Logic
    /// </summary>
    public class SiraCagirmaService : ISiraCagirmaService
    {
        private readonly ISiraRepository _siraRepository;

        public SiraCagirmaService(ISiraRepository siraRepository)
        {
            _siraRepository = siraRepository;
        }

        public async Task<List<SiraCagirmaResponseDto>> GetBekleyenSiralarAsync()
        {
            var siralar = await _siraRepository.GetWaitingAsync();

            return siralar.Select(s => new SiraCagirmaResponseDto
            {
                SiraId = s.Id,
                SiraNo = s.SiraNo,
                KanalAltAdi = s.KanalAltIslem?.KanalAltAdi ?? "Bilinmiyor",
                BeklemeDurum = s.BeklemeDurum,
                SiraAlisZamani = s.SiraAlisZamani,
                IslemBaslamaZamani = s.IslemBaslamaZamani,
                PersonelAdSoyad = s.Personel != null ? $"{s.Personel.Ad} {s.Personel.Soyad}" : null,
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
                SiraId = s.Id,
                SiraNo = s.SiraNo,
                KanalAltAdi = s.KanalAltIslem?.KanalAltAdi ?? "Bilinmiyor",
                BeklemeDurum = s.BeklemeDurum,
                SiraAlisZamani = s.SiraAlisZamani,
                IslemBaslamaZamani = s.IslemBaslamaZamani,
                PersonelAdSoyad = s.Personel != null ? $"{s.Personel.Ad} {s.Personel.Soyad}" : null,
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
            sira.PersonelTcKimlikNo = personelTcKimlikNo;

            await _siraRepository.UpdateAsync(sira);

            return new SiraCagirmaResponseDto
            {
                SiraId = sira.Id,
                SiraNo = sira.SiraNo,
                KanalAltAdi = sira.KanalAltIslem?.KanalAltAdi ?? "Bilinmiyor",
                BeklemeDurum = sira.BeklemeDurum,
                SiraAlisZamani = sira.SiraAlisZamani,
                IslemBaslamaZamani = sira.IslemBaslamaZamani,
                PersonelAdSoyad = sira.Personel != null ? $"{sira.Personel.Ad} {sira.Personel.Soyad}" : null,
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

            sira.BeklemeDurum = BeklemeDurum.Tamamlandi;
            sira.IslemBitisZamani = DateTime.Now;

            await _siraRepository.UpdateAsync(sira);
            return true;
        }

        public async Task<bool> SiraIptalAsync(int siraId, string iptalNedeni)
        {
            var sira = await _siraRepository.GetByIdAsync(siraId);
            if (sira == null)
            {
                return false;
            }

            sira.BeklemeDurum = BeklemeDurum.IptalEdildi;
            sira.IptalNedeni = iptalNedeni;
            sira.IptalZamani = DateTime.Now;

            await _siraRepository.UpdateAsync(sira);
            return true;
        }
    }
}

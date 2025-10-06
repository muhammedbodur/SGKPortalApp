using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices
{
    /// <summary>
    /// Departman API servisi - Mock implementation
    /// Gerçek API hazır olunca bu değiştirilecek
    /// </summary>
    public class DepartmanApiService : IDepartmanApiService
    {
        // Mock veri listesi
        private static readonly List<DepartmanResponseDto> _mockDepartmanlar = new()
        {
            new() { DepartmanId = 1, DepartmanAdi = "İnsan Kaynakları", DepartmanAktiflik = Aktiflik.Aktif, PersonelSayisi = 15, EklenmeTarihi = DateTime.Now.AddMonths(-12), DuzenlenmeTarihi = DateTime.Now.AddDays(-5) },
            new() { DepartmanId = 2, DepartmanAdi = "Bilgi İşlem", DepartmanAktiflik = Aktiflik.Aktif, PersonelSayisi = 8, EklenmeTarihi = DateTime.Now.AddMonths(-10), DuzenlenmeTarihi = DateTime.Now.AddDays(-2) },
            new() { DepartmanId = 3, DepartmanAdi = "Muhasebe", DepartmanAktiflik = Aktiflik.Aktif, PersonelSayisi = 12, EklenmeTarihi = DateTime.Now.AddMonths(-8), DuzenlenmeTarihi = DateTime.Now.AddDays(-10) },
            new() { DepartmanId = 4, DepartmanAdi = "Satın Alma", DepartmanAktiflik = Aktiflik.Aktif, PersonelSayisi = 5, EklenmeTarihi = DateTime.Now.AddMonths(-6), DuzenlenmeTarihi = DateTime.Now.AddDays(-15) },
            new() { DepartmanId = 5, DepartmanAdi = "Arşiv", DepartmanAktiflik = Aktiflik.Pasif, PersonelSayisi = 2, EklenmeTarihi = DateTime.Now.AddYears(-2), DuzenlenmeTarihi = DateTime.Now.AddMonths(-3) }
        };

        // Tüm departmanları getir
        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetAllAsync()
        {
            await Task.Delay(300); // API çağrısını simüle et
            var list = _mockDepartmanlar.OrderBy(d => d.DepartmanAdi).ToList();
            return ServiceResult<List<DepartmanResponseDto>>.Ok(list, "Departman listesi yüklendi");
        }

        // ID'ye göre departman getir
        public async Task<ServiceResult<DepartmanResponseDto>> GetByIdAsync(int id)
        {
            await Task.Delay(200);
            var departman = _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
            return departman != null
                ? ServiceResult<DepartmanResponseDto>.Ok(departman, "Departman bulundu")
                : ServiceResult<DepartmanResponseDto>.Fail("Departman bulunamadı");
        }

        // Yeni departman oluştur
        public async Task<ServiceResult<DepartmanResponseDto>> CreateAsync(DepartmanCreateRequestDto request)
        {
            await Task.Delay(400);

            var newDepartman = new DepartmanResponseDto
            {
                DepartmanId = _mockDepartmanlar.Max(d => d.DepartmanId) + 1,
                DepartmanAdi = request.DepartmanAdi,
                DepartmanAktiflik = Aktiflik.Aktif,
                PersonelSayisi = 0,
                EklenmeTarihi = DateTime.Now,
                DuzenlenmeTarihi = DateTime.Now
            };

            _mockDepartmanlar.Add(newDepartman);
            return ServiceResult<DepartmanResponseDto>.Ok(newDepartman, "Departman başarıyla eklendi");
        }

        // Departman güncelle
        public async Task<ServiceResult<DepartmanResponseDto>> UpdateAsync(int id, DepartmanUpdateRequestDto request)
        {
            await Task.Delay(400);

            var departman = _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
            if (departman == null)
                return ServiceResult<DepartmanResponseDto>.Fail("Departman bulunamadı");

            departman.DepartmanAdi = request.DepartmanAdi;
            departman.DepartmanAktiflik = request.DepartmanAktiflik;
            departman.DuzenlenmeTarihi = DateTime.Now;

            return ServiceResult<DepartmanResponseDto>.Ok(departman, "Departman başarıyla güncellendi");
        }

        // Departman sil
        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            await Task.Delay(300);

            var departman = _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
            if (departman == null)
                return ServiceResult<bool>.Fail("Departman bulunamadı");

            _mockDepartmanlar.Remove(departman);
            return ServiceResult<bool>.Ok(true, "Departman başarıyla silindi");
        }

        // Aktif departmanları getir
        public async Task<ServiceResult<List<DepartmanResponseDto>>> GetActiveAsync()
        {
            await Task.Delay(300);
            var activeList = _mockDepartmanlar
                .Where(d => d.DepartmanAktiflik == Aktiflik.Aktif)
                .OrderBy(d => d.DepartmanAdi)
                .ToList();

            return ServiceResult<List<DepartmanResponseDto>>.Ok(activeList, "Aktif departman listesi getirildi");
        }
    }
}

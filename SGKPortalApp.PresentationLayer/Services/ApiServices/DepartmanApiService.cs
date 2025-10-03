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
        // Mock veri
        private static List<DepartmanResponseDto> _mockDepartmanlar = new()
        {
            new DepartmanResponseDto
            {
                DepartmanId = 1,
                DepartmanAdi = "İnsan Kaynakları",
                DepartmanAktiflik = Aktiflik.Aktif,
                PersonelSayisi = 15,
                EklenmeTarihi = DateTime.Now.AddMonths(-12),
                DuzenlenmeTarihi = DateTime.Now.AddDays(-5)
            },
            new DepartmanResponseDto
            {
                DepartmanId = 2,
                DepartmanAdi = "Bilgi İşlem",
                DepartmanAktiflik = Aktiflik.Aktif,
                PersonelSayisi = 8,
                EklenmeTarihi = DateTime.Now.AddMonths(-10),
                DuzenlenmeTarihi = DateTime.Now.AddDays(-2)
            },
            new DepartmanResponseDto
            {
                DepartmanId = 3,
                DepartmanAdi = "Muhasebe",
                DepartmanAktiflik = Aktiflik.Aktif,
                PersonelSayisi = 12,
                EklenmeTarihi = DateTime.Now.AddMonths(-8),
                DuzenlenmeTarihi = DateTime.Now.AddDays(-10)
            },
            new DepartmanResponseDto
            {
                DepartmanId = 4,
                DepartmanAdi = "Satın Alma",
                DepartmanAktiflik = Aktiflik.Aktif,
                PersonelSayisi = 5,
                EklenmeTarihi = DateTime.Now.AddMonths(-6),
                DuzenlenmeTarihi = DateTime.Now.AddDays(-15)
            },
            new DepartmanResponseDto
            {
                DepartmanId = 5,
                DepartmanAdi = "Arşiv",
                DepartmanAktiflik = Aktiflik.Pasif,
                PersonelSayisi = 2,
                EklenmeTarihi = DateTime.Now.AddYears(-2),
                DuzenlenmeTarihi = DateTime.Now.AddMonths(-3)
            }
        };

        public async Task<List<DepartmanResponseDto>> GetAllAsync()
        {
            await Task.Delay(300); // API çağrısını simüle et
            return _mockDepartmanlar.OrderBy(d => d.DepartmanAdi).ToList();
        }

        public async Task<DepartmanResponseDto?> GetByIdAsync(int id)
        {
            await Task.Delay(200);
            return _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
        }

        public async Task<DepartmanResponseDto> CreateAsync(DepartmanCreateRequestDto request)
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
            return newDepartman;
        }

        public async Task<DepartmanResponseDto> UpdateAsync(int id, DepartmanUpdateRequestDto request)
        {
            await Task.Delay(400);

            var departman = _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
            if (departman != null)
            {
                departman.DepartmanAdi = request.DepartmanAdi;
                departman.DepartmanAktiflik = request.DepartmanAktiflik;
                departman.DuzenlenmeTarihi = DateTime.Now;
            }

            return departman!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await Task.Delay(300);

            var departman = _mockDepartmanlar.FirstOrDefault(d => d.DepartmanId == id);
            if (departman != null)
            {
                _mockDepartmanlar.Remove(departman);
                return true;
            }

            return false;
        }

        public async Task<List<DepartmanResponseDto>> GetActiveAsync()
        {
            await Task.Delay(300);
            return _mockDepartmanlar
                .Where(d => d.DepartmanAktiflik == Aktiflik.Aktif)
                .OrderBy(d => d.DepartmanAdi)
                .ToList();
        }
    }
}
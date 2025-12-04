using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    /// <summary>
    /// Kiosk Simülatör API Servisi Interface
    /// Masaüstü kiosk uygulamasını simüle eder
    /// </summary>
    public interface IKioskSimulatorApiService
    {
        /// <summary>
        /// Hizmet binasındaki kiosk menülerini listele
        /// Sadece aktif personeli (Yrd.Uzman+) olan menüler döner
        /// </summary>
        Task<ServiceResult<List<KioskMenuDto>>> GetKioskMenulerAsync(int hizmetBinasiId);

        /// <summary>
        /// Seçilen kiosk menüsündeki alt işlemleri listele
        /// Sadece aktif personeli (Yrd.Uzman+) olan işlemler döner
        /// </summary>
        Task<ServiceResult<List<KioskAltIslemDto>>> GetKioskMenuAltIslemleriAsync(int hizmetBinasiId, int kioskMenuId);

        /// <summary>
        /// Kiosk'tan sıra al
        /// </summary>
        Task<ServiceResult<KioskSiraAlResponseDto>> SiraAlAsync(KioskSiraAlRequestDto request);

        /// <summary>
        /// Belirli bir işlem için bekleyen sıra sayısını getir
        /// </summary>
        Task<ServiceResult<int>> GetBekleyenSayisiAsync(int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Belirli bir işlem için aktif personel var mı kontrol et
        /// </summary>
        Task<ServiceResult<bool>> HasAktifPersonelAsync(int hizmetBinasiId, int kanalAltIslemId);
    }
}

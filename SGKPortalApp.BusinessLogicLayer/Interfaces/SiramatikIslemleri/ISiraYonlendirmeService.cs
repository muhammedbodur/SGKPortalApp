using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface ISiraYonlendirmeService
    {
        /// <summary>
        /// Sırayı başka bir bankoya veya şefe yönlendirir
        /// </summary>
        Task<ApiResponseDto<bool>> YonlendirSiraAsync(SiraYonlendirmeDto request);

        /// <summary>
        /// Bankoya yönlendirilmiş sıraları getirir
        /// </summary>
        Task<ApiResponseDto<int>> GetYonlendirilmisSiraCountAsync(int bankoId);

        /// <summary>
        /// Sıra için kullanılabilir yönlendirme seçeneklerini getirir
        /// Sadece geçerli (aktif personel/banko olan) seçenekleri döner
        /// </summary>
        Task<ApiResponseDto<YonlendirmeSecenekleriResponseDto>> GetYonlendirmeSecenekleriAsync(int siraId, int kaynakBankoId);
    }
}

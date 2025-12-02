using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    /// <summary>
    /// Sıra Yönlendirme API Servisi - Presentation Layer
    /// </summary>
    public interface ISiraYonlendirmeApiService
    {
        /// <summary>
        /// Çağrılmış bir sırayı başka bankoya, şefe veya uzman personele yönlendirir
        /// </summary>
        Task<ApiResponseDto<bool>> YonlendirSiraAsync(SiraYonlendirmeDto request);

        /// <summary>
        /// Belirtilen bankoya yönlendirilmiş sıra sayısını getirir
        /// </summary>
        Task<ApiResponseDto<int>> GetYonlendirilmisSiraCountAsync(int bankoId);

        /// <summary>
        /// Sıra için kullanılabilir yönlendirme seçeneklerini getirir
        /// </summary>
        Task<ApiResponseDto<YonlendirmeSecenekleriResponseDto>> GetYonlendirmeSecenekleriAsync(int siraId, int kaynakBankoId);
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

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
    }
}

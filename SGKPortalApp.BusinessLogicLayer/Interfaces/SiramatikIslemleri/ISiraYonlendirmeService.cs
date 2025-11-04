using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

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
    }
}

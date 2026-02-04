using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IHaberService
    {
        /// <summary>
        /// Dashboard slider için haberleri getirir (vitrin resim dahil)
        /// </summary>
        Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5);

        /// <summary>
        /// Haberler liste sayfası: sayfalama + arama
        /// </summary>
        Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(
            int pageNumber = 1, int pageSize = 12, string? searchTerm = null);

        /// <summary>
        /// Tek haber detayı (tüm resimler dahil)
        /// </summary>
        Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId);
    }
}

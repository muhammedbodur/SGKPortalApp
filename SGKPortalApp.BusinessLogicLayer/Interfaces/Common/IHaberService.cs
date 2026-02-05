using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IHaberService
    {
        Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5);
        Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(
            int pageNumber = 1, int pageSize = 12, string? searchTerm = null);
        Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId);

        // ─── CRUD ───────────────────────────────────────────

        Task<ApiResponseDto<HaberResponseDto>> CreateHaberAsync(HaberCreateRequestDto request);
        Task<ApiResponseDto<HaberResponseDto>> UpdateHaberAsync(HaberUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteHaberAsync(int haberId);

        /// <summary>
        /// Admin liste: tarih/aktiflik filtresi olmaksızın tüm haberler
        /// </summary>
        Task<ApiResponseDto<HaberListeResponseDto>> GetAdminHaberListeAsync(
            int pageNumber = 1, int pageSize = 12, string? searchTerm = null);

        Task<ApiResponseDto<HaberResimResponseDto>> AddResimAsync(HaberResimCreateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteResimAsync(int haberResimId);
    }
}

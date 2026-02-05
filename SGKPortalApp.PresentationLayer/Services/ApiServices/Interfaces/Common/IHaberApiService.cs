using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IHaberApiService
    {
        Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5);
        Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(int page = 1, int pageSize = 12, string? search = null);
        Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId);
        Task<byte[]?> DownloadHaberWordAsync(int haberId);

        // ─── CRUD ───────────────────────────────────────────

        Task<ApiResponseDto<HaberListeResponseDto>> GetAdminHaberListeAsync(int page = 1, int pageSize = 12, string? search = null);
        Task<ApiResponseDto<HaberResponseDto>> CreateHaberAsync(HaberCreateRequestDto request);
        Task<ApiResponseDto<HaberResponseDto>> UpdateHaberAsync(int haberId, HaberUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteHaberAsync(int haberId);
        Task<ApiResponseDto<HaberResimResponseDto>> AddResimAsync(int haberId, HaberResimCreateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteResimAsync(int haberId, int resimId);
    }
}

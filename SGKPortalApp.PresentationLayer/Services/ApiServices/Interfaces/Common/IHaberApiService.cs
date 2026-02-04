using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IHaberApiService
    {
        Task<ApiResponseDto<List<HaberResponseDto>>> GetSliderHaberleriAsync(int count = 5);
        Task<ApiResponseDto<HaberListeResponseDto>> GetHaberListeAsync(int page = 1, int pageSize = 12, string? search = null);
        Task<ApiResponseDto<HaberResponseDto?>> GetHaberByIdAsync(int haberId);
        /// <summary>
        /// Word dosyası byte[] olarak getirir
        /// </summary>
        Task<byte[]?> DownloadHaberWordAsync(int haberId);
    }
}

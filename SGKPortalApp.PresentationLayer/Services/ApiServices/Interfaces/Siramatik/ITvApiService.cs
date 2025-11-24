using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface ITvApiService
    {
        Task<ApiResponseDto<TvResponseDto>> CreateAsync(TvCreateRequestDto request);
        Task<ApiResponseDto<TvResponseDto>> UpdateAsync(TvUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int tvId);
        Task<ApiResponseDto<TvResponseDto>> GetByIdAsync(int tvId);
        Task<ApiResponseDto<List<TvResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<TvResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<TvResponseDto>>> GetByKatTipiAsync(KatTipi katTipi);
        Task<ApiResponseDto<List<TvResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<TvResponseDto>> GetWithDetailsAsync(int tvId);
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByHizmetBinasiDropdownAsync(int hizmetBinasiId);
        
        // TV-Banko İlişkileri
        Task<ApiResponseDto<bool>> AddBankoToTvAsync(int tvId, int bankoId);
        Task<ApiResponseDto<bool>> RemoveBankoFromTvAsync(int tvId, int bankoId);
    }
}

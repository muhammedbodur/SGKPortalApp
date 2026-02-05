using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IOnemliLinkService
    {
        Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<OnemliLinkResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<OnemliLinkResponseDto>> CreateAsync(OnemliLinkCreateRequestDto request);
        Task<ApiResponseDto<OnemliLinkResponseDto>> UpdateAsync(int id, OnemliLinkUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<PagedResponseDto<OnemliLinkResponseDto>>> GetPagedAsync(OnemliLinkFilterRequestDto filter);
        Task<ApiResponseDto<List<OnemliLinkResponseDto>>> GetActiveAsync();
    }
}

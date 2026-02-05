using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IOnemliLinkApiService
    {
        Task<ServiceResult<List<OnemliLinkResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<OnemliLinkResponseDto>>> GetActiveAsync();
        Task<ServiceResult<OnemliLinkResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<OnemliLinkResponseDto>> CreateAsync(OnemliLinkCreateRequestDto request);
        Task<ServiceResult<OnemliLinkResponseDto>> UpdateAsync(int id, OnemliLinkUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<PagedResponseDto<OnemliLinkResponseDto>>> GetPagedAsync(OnemliLinkFilterRequestDto filter);
    }
}

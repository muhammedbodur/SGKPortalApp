using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IGununMenusuApiService
    {
        Task<ServiceResult<List<GununMenusuResponseDto>>> GetAllAsync();
        Task<ServiceResult<GununMenusuResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<GununMenusuResponseDto?>> GetByDateAsync(DateTime date);
        Task<ServiceResult<GununMenusuResponseDto>> CreateAsync(GununMenusuCreateRequestDto request);
        Task<ServiceResult<GununMenusuResponseDto>> UpdateAsync(int id, GununMenusuUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<PagedResponseDto<GununMenusuResponseDto>>> GetPagedAsync(GununMenusuFilterRequestDto filter);
    }
}

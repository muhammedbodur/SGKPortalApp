using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IIlService
    {
        Task<ApiResponseDto<List<IlResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<IlResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<IlResponseDto>> CreateAsync(IlCreateRequestDto request);
        Task<ApiResponseDto<IlResponseDto>> UpdateAsync(int id, IlUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<IlResponseDto>> GetByNameAsync(string ilAdi);
        Task<ApiResponseDto<PagedResponseDto<IlResponseDto>>> GetPagedAsync(IlFilterRequestDto filter);

        Task<ApiResponseDto<int>> GetIlceCountAsync(int ilId);
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
    }
}

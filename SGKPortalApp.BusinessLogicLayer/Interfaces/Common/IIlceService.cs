using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IIlceService
    {
        Task<ApiResponseDto<List<IlceResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<IlceResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<IlceResponseDto>> CreateAsync(IlceCreateRequestDto request);
        Task<ApiResponseDto<IlceResponseDto>> UpdateAsync(int id, IlceUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        Task<ApiResponseDto<List<IlceResponseDto>>> GetByIlAsync(int ilId);
        Task<ApiResponseDto<IlceResponseDto>> GetByNameAsync(string ilceAdi);
        Task<ApiResponseDto<PagedResponseDto<IlceResponseDto>>> GetPagedAsync(IlceFilterRequestDto filter);

        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetDropdownAsync();
        Task<ApiResponseDto<List<(int Id, string Ad)>>> GetByIlDropdownAsync(int ilId);
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IIlApiService
    {
        Task<ServiceResult<List<IlResponseDto>>> GetAllAsync();
        Task<ServiceResult<IlResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IlResponseDto>> CreateAsync(IlCreateRequestDto request);
        Task<ServiceResult<IlResponseDto>> UpdateAsync(int id, IlUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<int>> GetIlceCountAsync(int ilId);
        Task<ServiceResult<List<(int Id, string Ad)>>> GetDropdownAsync();
    }
}

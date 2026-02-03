using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IIlceApiService
    {
        Task<ServiceResult<List<IlceResponseDto>>> GetAllAsync();
        Task<ServiceResult<IlceResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<IlceResponseDto>> CreateAsync(IlceCreateRequestDto request);
        Task<ServiceResult<IlceResponseDto>> UpdateAsync(int id, IlceUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<IlceResponseDto>>> GetByIlAsync(int ilId);
        Task<ServiceResult<List<(int Id, string Ad)>>> GetDropdownAsync();
        Task<ServiceResult<List<(int Id, string Ad)>>> GetByIlDropdownAsync(int ilId);
    }
}

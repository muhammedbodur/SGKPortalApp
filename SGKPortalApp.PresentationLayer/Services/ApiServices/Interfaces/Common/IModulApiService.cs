using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IModulApiService
    {
        Task<ServiceResult<List<ModulResponseDto>>> GetAllAsync();
        Task<ServiceResult<ModulResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<ModulResponseDto>> CreateAsync(ModulCreateRequestDto request);
        Task<ServiceResult<ModulResponseDto>> UpdateAsync(int id, ModulUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync();
    }
}

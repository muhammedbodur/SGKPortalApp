using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IModulControllerIslemApiService
    {
        Task<ServiceResult<List<ModulControllerIslemResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<ModulControllerIslemResponseDto>>> GetByControllerIdAsync(int controllerId);
        Task<ServiceResult<ModulControllerIslemResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<ModulControllerIslemResponseDto>> CreateAsync(ModulControllerIslemCreateRequestDto request);
        Task<ServiceResult<ModulControllerIslemResponseDto>> UpdateAsync(int id, ModulControllerIslemUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync();
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownByControllerIdAsync(int controllerId);
        Task<ServiceResult<List<DropdownDto>>> GetActionTypesAsync();
    }
}

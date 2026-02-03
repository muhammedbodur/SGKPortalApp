using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IModulControllerApiService
    {
        Task<ServiceResult<List<ModulControllerResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<ModulControllerResponseDto>>> GetByModulIdAsync(int modulId);
        Task<ServiceResult<ModulControllerResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<ModulControllerResponseDto>> CreateAsync(ModulControllerCreateRequestDto request);
        Task<ServiceResult<ModulControllerResponseDto>> UpdateAsync(int id, ModulControllerUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync();
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownByModulIdAsync(int modulId);
    }
}

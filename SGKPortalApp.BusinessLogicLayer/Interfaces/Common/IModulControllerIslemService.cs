using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IModulControllerIslemService
    {
        Task<ApiResponseDto<List<ModulControllerIslemResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<ModulControllerIslemResponseDto>>> GetByControllerIdAsync(int controllerId);
        Task<ApiResponseDto<ModulControllerIslemResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<ModulControllerIslemResponseDto>> CreateAsync(ModulControllerIslemCreateRequestDto request);
        Task<ApiResponseDto<ModulControllerIslemResponseDto>> UpdateAsync(int id, ModulControllerIslemUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync();
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownByControllerIdAsync(int controllerId);
    }
}

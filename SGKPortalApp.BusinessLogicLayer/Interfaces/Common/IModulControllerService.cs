using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IModulControllerService
    {
        Task<ApiResponseDto<List<ModulControllerResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<ModulControllerResponseDto>>> GetByModulIdAsync(int modulId);
        Task<ApiResponseDto<ModulControllerResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<ModulControllerResponseDto>> CreateAsync(ModulControllerCreateRequestDto request);
        Task<ApiResponseDto<ModulControllerResponseDto>> UpdateAsync(int id, ModulControllerUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync();
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownByModulIdAsync(int modulId);
    }
}

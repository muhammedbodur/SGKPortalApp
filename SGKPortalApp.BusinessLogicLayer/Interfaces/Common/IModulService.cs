using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IModulService
    {
        Task<ApiResponseDto<List<ModulResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<ModulResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<ModulResponseDto>> CreateAsync(ModulCreateRequestDto request);
        Task<ApiResponseDto<ModulResponseDto>> UpdateAsync(int id, ModulUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync();
    }
}

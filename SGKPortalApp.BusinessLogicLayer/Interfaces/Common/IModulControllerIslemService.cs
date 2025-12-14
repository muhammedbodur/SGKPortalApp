using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IModulControllerIslemService
    {
        Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync();
    }
}

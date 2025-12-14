using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IModulControllerIslemApiService
    {
        Task<ServiceResult<List<DropdownItemDto>>> GetDropdownAsync();
    }
}

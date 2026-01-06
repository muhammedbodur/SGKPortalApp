using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.Entities.ZKTeco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface IZKTecoDeviceApiService
    {
        Task<ApiResponse<List<Device>>> GetAllAsync();
        Task<ApiResponse<List<Device>>> GetActiveAsync();
        Task<ApiResponse<Device>> GetByIdAsync(int id);
    }
}

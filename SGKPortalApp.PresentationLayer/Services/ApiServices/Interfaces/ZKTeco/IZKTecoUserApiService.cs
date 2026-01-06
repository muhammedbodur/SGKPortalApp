using SGKPortalApp.BusinessObjectLayer.DTOs.Response;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface IZKTecoUserApiService
    {
        Task<ApiResponse<List<ZKTecoUserDto>>> GetAllAsync();
        Task<ApiResponse<ZKTecoUserDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<ZKTecoUserDto>>> GetByDeviceIdAsync(int deviceId);
        Task<ApiResponse<List<ZKTecoApiUserDto>>> GetUsersFromDeviceAsync(int deviceId);
        Task<ApiResponse<bool>> SyncUsersFromDeviceToDbAsync(int deviceId);
    }
}

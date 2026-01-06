using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco
{
    public interface IZKTecoDeviceApiService
    {
        Task<ServiceResult<List<DeviceResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<DeviceResponseDto>>> GetActiveAsync();
        Task<ServiceResult<DeviceResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<DeviceStatusDto>> GetStatusAsync(int deviceId);
    }
}

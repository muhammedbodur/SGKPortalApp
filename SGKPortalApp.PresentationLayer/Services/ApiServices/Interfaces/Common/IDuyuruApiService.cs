using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Dashboard;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common
{
    public interface IDuyuruApiService
    {
        Task<ServiceResult<List<DuyuruResponseDto>>> GetAllAsync();
        Task<ServiceResult<DuyuruResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<DuyuruResponseDto>> CreateAsync(DuyuruCreateRequestDto request);
        Task<ServiceResult<DuyuruResponseDto>> UpdateAsync(int id, DuyuruUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<string>> UploadImageAsync(Stream fileStream, string fileName);
    }
}

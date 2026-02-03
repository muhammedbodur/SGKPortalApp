using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKanalApiService
    {
        Task<ServiceResult<List<KanalResponseDto>>> GetAllAsync();
        Task<ServiceResult<KanalResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<KanalResponseDto>> CreateAsync(KanalCreateRequestDto dto);
        Task<ServiceResult<KanalResponseDto>> UpdateAsync(int id, KanalUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<KanalResponseDto>>> GetActiveAsync();
    }
}

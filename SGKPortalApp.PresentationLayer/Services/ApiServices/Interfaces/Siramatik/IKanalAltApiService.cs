using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKanalAltApiService
    {
        Task<ServiceResult<List<KanalAltResponseDto>>> GetAllAsync();
        Task<ServiceResult<KanalAltResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<KanalAltResponseDto>> CreateAsync(KanalAltKanalCreateRequestDto dto);
        Task<ServiceResult<KanalAltResponseDto>> UpdateAsync(int id, KanalAltKanalUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<KanalAltResponseDto>>> GetActiveAsync();
        Task<ServiceResult<List<KanalAltResponseDto>>> GetByKanalIdAsync(int kanalId);
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKioskMenuApiService
    {
        Task<ServiceResult<List<KioskMenuResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<KioskMenuResponseDto>>> GetActiveAsync();
        Task<ServiceResult<KioskMenuResponseDto>> GetByIdAsync(int kioskMenuId);
        Task<ServiceResult<KioskMenuResponseDto>> CreateAsync(KioskMenuCreateRequestDto dto);
        Task<ServiceResult<KioskMenuResponseDto>> UpdateAsync(KioskMenuUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int kioskMenuId);
    }
}

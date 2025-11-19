using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuService
    {
        Task<ApiResponseDto<List<KioskMenuResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<KioskMenuResponseDto>> GetByIdAsync(int kioskMenuId);
        Task<ApiResponseDto<KioskMenuResponseDto>> CreateAsync(KioskMenuCreateRequestDto request);
        Task<ApiResponseDto<KioskMenuResponseDto>> UpdateAsync(KioskMenuUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuId);
        Task<ApiResponseDto<List<KioskMenuResponseDto>>> GetActiveAsync();
    }
}

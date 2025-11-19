using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuIslemService
    {
        Task<ApiResponseDto<List<KioskMenuIslemResponseDto>>> GetByKioskMenuAsync(int kioskMenuId);
        Task<ApiResponseDto<KioskMenuIslemResponseDto>> GetByIdAsync(int kioskMenuIslemId);
        Task<ApiResponseDto<KioskMenuIslemResponseDto>> CreateAsync(KioskMenuIslemCreateRequestDto request);
        Task<ApiResponseDto<KioskMenuIslemResponseDto>> UpdateAsync(KioskMenuIslemUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuIslemId);
        Task<ApiResponseDto<bool>> UpdateSiraAsync(int kioskMenuIslemId, int yeniSira);
    }
}

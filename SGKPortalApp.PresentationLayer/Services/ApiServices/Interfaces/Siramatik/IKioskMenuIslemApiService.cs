using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKioskMenuIslemApiService
    {
        Task<ServiceResult<List<KioskMenuIslemResponseDto>>> GetByKioskMenuAsync(int kioskMenuId);
        Task<ServiceResult<KioskMenuIslemResponseDto>> GetByIdAsync(int kioskMenuIslemId);
        Task<ServiceResult<KioskMenuIslemResponseDto>> CreateAsync(KioskMenuIslemCreateRequestDto request);
        Task<ServiceResult<KioskMenuIslemResponseDto>> UpdateAsync(KioskMenuIslemUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int kioskMenuIslemId);
        Task<ServiceResult<bool>> UpdateSiraAsync(int kioskMenuIslemId, int yeniSira);
    }
}

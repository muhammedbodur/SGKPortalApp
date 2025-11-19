using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKioskMenuAtamaApiService
    {
        Task<ServiceResult<List<KioskMenuAtamaResponseDto>>> GetAllAsync();
        Task<ServiceResult<List<KioskMenuAtamaResponseDto>>> GetByKioskAsync(int kioskId);
        Task<ServiceResult<KioskMenuAtamaResponseDto>> GetActiveByKioskAsync(int kioskId);
        Task<ServiceResult<KioskMenuAtamaResponseDto>> GetByIdAsync(int kioskMenuAtamaId);
        Task<ServiceResult<KioskMenuAtamaResponseDto>> CreateAsync(KioskMenuAtamaCreateRequestDto request);
        Task<ServiceResult<KioskMenuAtamaResponseDto>> UpdateAsync(KioskMenuAtamaUpdateRequestDto request);
        Task<ServiceResult<bool>> DeleteAsync(int kioskMenuAtamaId);
    }
}

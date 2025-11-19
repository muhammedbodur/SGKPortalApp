using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKioskApiService
    {
        Task<ServiceResult<List<KioskResponseDto>>> GetAllAsync();
        Task<ServiceResult<KioskResponseDto>> GetByIdAsync(int kioskId);
        Task<ServiceResult<KioskResponseDto>> GetWithMenuAsync(int kioskId);
        Task<ServiceResult<List<KioskResponseDto>>> GetActiveAsync();
        Task<ServiceResult<List<KioskResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ServiceResult<KioskResponseDto>> CreateAsync(KioskCreateRequestDto dto);
        Task<ServiceResult<KioskResponseDto>> UpdateAsync(KioskUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int kioskId);
    }
}

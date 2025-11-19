using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKioskService
    {
        Task<ApiResponseDto<List<KioskResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<KioskResponseDto>> GetByIdAsync(int kioskId);
        Task<ApiResponseDto<KioskResponseDto>> CreateAsync(KioskCreateRequestDto request);
        Task<ApiResponseDto<KioskResponseDto>> UpdateAsync(KioskUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kioskId);
        Task<ApiResponseDto<List<KioskResponseDto>>> GetByHizmetBinasiAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<KioskResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<KioskResponseDto>> GetWithMenuAsync(int kioskId);
    }
}

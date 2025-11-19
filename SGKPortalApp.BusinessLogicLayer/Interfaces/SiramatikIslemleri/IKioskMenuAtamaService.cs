using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKioskMenuAtamaService
    {
        Task<ApiResponseDto<List<KioskMenuAtamaResponseDto>>> GetByKioskAsync(int kioskId);
        Task<ApiResponseDto<KioskMenuAtamaResponseDto>> GetActiveByKioskAsync(int kioskId);
        Task<ApiResponseDto<KioskMenuAtamaResponseDto>> GetByIdAsync(int kioskMenuAtamaId);
        Task<ApiResponseDto<KioskMenuAtamaResponseDto>> CreateAsync(KioskMenuAtamaCreateRequestDto request);
        Task<ApiResponseDto<KioskMenuAtamaResponseDto>> UpdateAsync(KioskMenuAtamaUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kioskMenuAtamaId);
        Task<ApiResponseDto<List<KioskMenuAtamaResponseDto>>> GetAllAsync();
    }
}

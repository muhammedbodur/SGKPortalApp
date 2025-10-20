using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKanalService
    {
        // CRUD Operations
        Task<ApiResponseDto<List<KanalResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<KanalResponseDto>> GetByIdAsync(int kanalId);
        Task<ApiResponseDto<KanalResponseDto>> CreateAsync(KanalCreateRequestDto request);
        Task<ApiResponseDto<KanalResponseDto>> UpdateAsync(int kanalId, KanalUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kanalId);
        
        // Business Operations
        Task<ApiResponseDto<List<KanalResponseDto>>> GetActiveAsync();
    }
}

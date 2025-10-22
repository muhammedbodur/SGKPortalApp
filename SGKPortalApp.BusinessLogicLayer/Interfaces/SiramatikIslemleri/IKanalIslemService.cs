using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKanalIslemService
    {
        // CRUD Operations
        Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<KanalIslemResponseDto>> GetByIdAsync(int kanalIslemId);
        Task<ApiResponseDto<KanalIslemResponseDto>> CreateAsync(KanalIslemCreateRequestDto request);
        Task<ApiResponseDto<KanalIslemResponseDto>> UpdateAsync(int kanalIslemId, KanalIslemUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kanalIslemId);
        
        // Business Operations
        Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByKanalIdAsync(int kanalId);
        Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<KanalIslemResponseDto>>> GetActiveAsync();
    }
}

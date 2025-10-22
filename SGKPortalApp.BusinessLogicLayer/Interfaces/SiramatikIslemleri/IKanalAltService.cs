using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKanalAltService
    {
        // CRUD Operations
        Task<ApiResponseDto<List<KanalAltResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<KanalAltResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<KanalAltResponseDto>> CreateAsync(KanalAltKanalCreateRequestDto request);
        Task<ApiResponseDto<KanalAltResponseDto>> UpdateAsync(int id, KanalAltKanalUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);

        // Additional Operations
        Task<ApiResponseDto<List<KanalAltResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<List<KanalAltResponseDto>>> GetByKanalIdAsync(int kanalId);
        Task<ApiResponseDto<KanalAltResponseDto>> GetWithDetailsAsync(int id);
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKanalIslemApiService
    {
        Task<ServiceResult<List<KanalIslemResponseDto>>> GetAllAsync();
        Task<ServiceResult<KanalIslemResponseDto>> GetByIdAsync(int id);
        Task<ServiceResult<KanalIslemResponseDto>> CreateAsync(KanalIslemCreateRequestDto dto);
        Task<ServiceResult<KanalIslemResponseDto>> UpdateAsync(int id, KanalIslemUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
        Task<ServiceResult<List<KanalIslemResponseDto>>> GetActiveAsync();
        Task<ServiceResult<List<KanalIslemResponseDto>>> GetByKanalIdAsync(int kanalId);
        Task<ServiceResult<List<KanalIslemResponseDto>>> GetByDepartmanHizmetBinasiIdAsync(int departmanHizmetBinasiId);
    }
}

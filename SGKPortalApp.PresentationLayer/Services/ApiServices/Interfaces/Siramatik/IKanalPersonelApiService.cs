using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKanalPersonelApiService
    {
        Task<ServiceResult<List<KanalPersonelResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<ServiceResult<List<KanalPersonelResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo);
        Task<ServiceResult<List<KanalPersonelResponseDto>>> GetByKanalAltIslemIdAsync(int kanalAltIslemId);
        Task<ServiceResult<KanalPersonelResponseDto>> GetByIdAsync(int id);
        
        // CRUD Operations
        Task<ServiceResult<KanalPersonelResponseDto>> CreateAsync(KanalPersonelCreateRequestDto dto);
        Task<ServiceResult<KanalPersonelResponseDto>> UpdateAsync(int id, KanalPersonelUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}

using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKanalPersonelService
    {
        // CRUD Operations
        Task<ApiResponseDto<KanalPersonelResponseDto>> CreateAsync(KanalPersonelCreateRequestDto request);
        Task<ApiResponseDto<KanalPersonelResponseDto>> UpdateAsync(int kanalPersonelId, KanalPersonelUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int kanalPersonelId);
        Task<ApiResponseDto<KanalPersonelResponseDto>> GetByIdAsync(int kanalPersonelId);
        
        // Business Operations
        Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetPersonellerByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetByPersonelTcAsync(string tcKimlikNo);
        Task<ApiResponseDto<List<KanalPersonelResponseDto>>> GetByKanalAltIslemIdAsync(int kanalAltIslemId);
        
        // Personel Atama Matrix (Yeni Yapı)
        Task<ApiResponseDto<List<PersonelAtamaMatrixDto>>> GetPersonelAtamaMatrixAsync(int hizmetBinasiId);
    }
}

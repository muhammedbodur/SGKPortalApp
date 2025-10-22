using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    public interface IKanalAltIslemApiService
    {
        Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetAllAsync();
        Task<ServiceResult<KanalAltIslemResponseDto>> GetByIdWithDetailsAsync(int id);
        Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetByKanalIslemIdAsync(int kanalIslemId);
        Task<ServiceResult<Dictionary<int, int>>> GetPersonelSayilariAsync(int hizmetBinasiId);
        Task<ServiceResult<List<KanalAltIslemResponseDto>>> GetEslestirmeYapilmamisAsync(int hizmetBinasiId);
        
        // CRUD Operations
        Task<ServiceResult<KanalAltIslemResponseDto>> CreateAsync(KanalAltCreateRequestDto dto);
        Task<ServiceResult<KanalAltIslemResponseDto>> UpdateAsync(int id, KanalAltUpdateRequestDto dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}

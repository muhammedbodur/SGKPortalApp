using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    public interface IKanalAltIslemService
    {
        // Complex Query Operations (SiramatikQueryRepository kullanarak)
        Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetByHizmetBinasiIdAsync(int hizmetBinasiId);
        Task<ApiResponseDto<KanalAltIslemResponseDto>> GetByIdWithDetailsAsync(int kanalAltIslemId);
        Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetByKanalIslemIdAsync(int kanalIslemId);
        
        // İstatistik ve Dashboard
        Task<ApiResponseDto<Dictionary<int, int>>> GetPersonelSayilariAsync(int hizmetBinasiId);
        Task<ApiResponseDto<List<KanalAltIslemResponseDto>>> GetEslestirmeYapilmamisAsync(int hizmetBinasiId);
    }
}

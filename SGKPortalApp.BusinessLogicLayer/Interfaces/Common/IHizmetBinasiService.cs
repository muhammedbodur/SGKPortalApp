using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IHizmetBinasiService
    {
        Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetAllAsync();
        Task<ApiResponseDto<HizmetBinasiResponseDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<HizmetBinasiDetailResponseDto>> GetDetailByIdAsync(int id);
        Task<ApiResponseDto<HizmetBinasiResponseDto>> CreateAsync(HizmetBinasiCreateRequestDto request);
        Task<ApiResponseDto<HizmetBinasiResponseDto>> UpdateAsync(int id, HizmetBinasiUpdateRequestDto request);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetActiveAsync();
        Task<ApiResponseDto<List<HizmetBinasiResponseDto>>> GetByDepartmanAsync(int departmanId);
        Task<ApiResponseDto<int>> GetPersonelCountAsync(int hizmetBinasiId);
        Task<ApiResponseDto<bool>> ToggleStatusAsync(int id);
        Task<ApiResponseDto<List<ServisResponseDto>>> GetServislerByHizmetBinasiIdAsync(int hizmetBinasiId);
    }
}